using Application.Metrics;
using Application.Services;
using Dapr.Client;
using Database;
using Domain;
using Domain.DTO;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.Metrics;

namespace Application.Test
{
    public class CreateCompanyCommandTests
    {
        private readonly Mock<ICompanyRepository<Company>> _repositoryMock;
        private readonly Mock<IValidator<CreateCompanyDto>> _validatorMock;
        private readonly Mock<DaprClient> _daprClientMock;
        private readonly Mock<IDaprCacheService> _daprCacheServiceMock;
        private readonly Mock<ILogger<CreateCompanyCommand>> _loggerMock;

        private readonly CompanyMetrics _metrics;

        private readonly CreateCompanyCommand _command;

        public CreateCompanyCommandTests()
        {
            _repositoryMock = new Mock<ICompanyRepository<Company>>();
            _validatorMock = new Mock<IValidator<CreateCompanyDto>>();
            _daprClientMock = new Mock<DaprClient>();
            _daprCacheServiceMock = new Mock<IDaprCacheService>();
            _loggerMock = new Mock<ILogger<CreateCompanyCommand>>();
            var meterFactoryMock = new Mock<IMeterFactory>();
            meterFactoryMock.Setup(x => x.Create(It.IsAny<MeterOptions>()))
                            .Returns(new Meter("TestMeter"));
            _metrics = new CompanyMetrics(meterFactoryMock.Object);
            _command = new CreateCompanyCommand(
                _repositoryMock.Object,
                _validatorMock.Object,
                _metrics,
                _daprClientMock.Object,
                _loggerMock.Object,
                _daprCacheServiceMock.Object);
        }

        [Fact]
        public async Task CreateCompanyAsync_ShouldReturnFail_WhenDtoIsNull()
        {
            var result = await _command.CreateCompanyAsync(null!);

            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task CreateCompanyAsync_ShouldReturnFail_WhenValidationFails()
        {
            var dto = new CreateCompanyDto { Name = "Test", Vat = "123" };
            _validatorMock.Setup(v => v.ValidateAsync(dto, default))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                          {
                              new FluentValidation.Results.ValidationFailure("Name", "Name is required")
                          }));
            var result = await _command.CreateCompanyAsync(dto);
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task CreateCompanyAsync_ShouldReturnOk_WhenCreatedSuccessfully()
        {
            var dto = new CreateCompanyDto { Name = "Test", Vat = "123" };

            _validatorMock.Setup(v => v.ValidateAsync(dto, default))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _repositoryMock.Setup(r => r.createAsync(It.IsAny<Company>()))
                     .Callback<Company>(c => c.Id = 123) // Simule la DB qui met l'ID
                     .Returns(Task.CompletedTask);

            var result = await _command.CreateCompanyAsync(dto);

            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(r => r.createAsync(It.IsAny<Company>()), Times.Once);

            // Vérifier que le cache a été invalidé
            _daprCacheServiceMock.Verify(c => c.InvalidateAllCompaniesAsync(), Times.Once);

            // Vérifier que l'événement Dapr a été publié
            _daprClientMock.Verify(d => d.PublishEventAsync(
                "rabbitmq-pubsub",
                "companycreated",
                It.IsAny<CreateCompanyDto>(),
                default), Times.Once);
        }
        [Fact]
        public async Task CreateCompanyAsync_ShouldReturnOk_EvenIfDaprPublishFails()
        {
            // Le but est de vérifier que si Dapr échoue, on ne plante pas toute la création
            // car il y a un try/catch autour du publish dans votre code

            // Arrange
            var dto = new CreateCompanyDto { Name = "DaprFail Corp", Vat = "BE000" };
            _validatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Dapr lance une exception
            _daprClientMock.Setup(d => d.PublishEventAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), default))
                           .ThrowsAsync(new Exception("Dapr is down"));

            // Act
            var result = await _command.CreateCompanyAsync(dto);

            // Assert
            Assert.True(result.IsSuccess); // Doit quand même réussir car la company est en DB

            // On vérifie que l'erreur a été loggée (optionnel mais bonne pratique)
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task CreateCompanyAsync_ShouldReturnFail_WhenRepositoryThrows()
        {
            // Arrange
            var dto = new CreateCompanyDto { Name = "DbFail Corp", Vat = "BE666" };
            _validatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _repositoryMock.Setup(r => r.createAsync(It.IsAny<Company>()))
                     .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _command.CreateCompanyAsync(dto);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Contains("Database error", result.Errors[0].Message);
        }
    }
}
