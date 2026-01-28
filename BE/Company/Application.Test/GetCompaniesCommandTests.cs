using Application.Services;
using Database;
using Domain;
using Domain.DTO; // Assurez-vous d'avoir ce using
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Application.Test
{
    public class GetCompaniesCommandTests
    {
        private readonly Mock<ICompanyRepository<Company>> _repositoryMock;
        private readonly Mock<IDaprCacheService> _daprCacheServiceMock;
        private readonly GetCompaniesCommand _command;

        public GetCompaniesCommandTests()
        {
            _repositoryMock = new Mock<ICompanyRepository<Company>>();
            _daprCacheServiceMock = new Mock<IDaprCacheService>();
            
            _command = new GetCompaniesCommand(
                _repositoryMock.Object,
                _daprCacheServiceMock.Object);
        }

        [Fact]
        public async Task GetCompaniesAsync_ShouldReturnPagedCompanies_FromDb()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            
            var dbCompanies = new List<Company>
            {
                new Company { Id = Guid.NewGuid(), Name = "Company 1", Vat = "VAT001" },
                new Company { Id = Guid.NewGuid(), Name = "Company 2", Vat = "VAT002" }
            };

            // On configure le mock pour répondre à la demande paginée
            _repositoryMock
                .Setup(x => x.GetAllCompaniesAsync(page, pageSize))
                .ReturnsAsync(dbCompanies);

            // Act
            // On appelle la méthode avec les paramètres de pagination
            var result = await _command.GetAllCompaniesAsync(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Company 1", result[0].Name);

            // Vérifie que le repository a bien été appelé avec les bons paramètres
            _repositoryMock.Verify(x => x.GetAllCompaniesAsync(page, pageSize), Times.Once);
        }

        [Fact]
        public async Task GetCompaniesAsync_ShouldReturnEmptyList_WhenNoDataFound()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;

            _repositoryMock
                .Setup(x => x.GetAllCompaniesAsync(page, pageSize))
                .ReturnsAsync(new List<Company>());

            // Act
            var result = await _command.GetAllCompaniesAsync(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            
            _repositoryMock.Verify(x => x.GetAllCompaniesAsync(page, pageSize), Times.Once);
        }

        [Fact]
        public async Task GetCompaniesAsync_ShouldHandleException_WhenRepositoryThrows()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;

            _repositoryMock
                .Setup(x => x.GetAllCompaniesAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
                await _command.GetAllCompaniesAsync(page, pageSize));
            
            _repositoryMock.Verify(x => x.GetAllCompaniesAsync(page, pageSize), Times.Once);
        }
    }
}