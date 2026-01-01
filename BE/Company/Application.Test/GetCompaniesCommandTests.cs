using Application.Services;
using Database;
using Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

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
        public async Task GetAllCompaniesAsync_ShouldReturnCompaniesFromCache_WhenCacheHit()
        {
            var cachedCompanies = new List<Company>
            {
                new Company { Id = 1, Name = "Cached Company 1", Vat = "VAT001" },
                new Company { Id = 2, Name = "Cached Company 2", Vat = "VAT002" }
            };

            _daprCacheServiceMock.Setup(x => x.GetAllCompaniesAsync()).ReturnsAsync(cachedCompanies);

            var result = await _command.GetAllCompaniesAsync();

            Assert.NotNull(result);
            Assert.Equal(cachedCompanies.Count, result.Count);

            _repositoryMock.Verify(x => x.getAllCompaniesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllCompaniesAsync_ShouldReturnCompaniesFromDbAndCacheThem_WhenCacheMiss()
        {
            var dbCompanies = new List<Company>
            {
                new Company { Id = 1, Name = "DB Company 1", Vat = "VAT101" },
                new Company { Id = 2, Name = "DB Company 2", Vat = "VAT102" }
            };
            _daprCacheServiceMock.Setup(x => x.GetAllCompaniesAsync()).ReturnsAsync((List<Company>?)null);
            _repositoryMock.Setup(x => x.getAllCompaniesAsync()).ReturnsAsync(dbCompanies);
            var result = await _command.GetAllCompaniesAsync();
            Assert.NotNull(result);
            Assert.Equal(dbCompanies.Count, result.Count);
            _repositoryMock.Verify(x => x.getAllCompaniesAsync(), Times.Once);
            _daprCacheServiceMock.Verify(x => x.SetAllCompaniesAsync(It.IsAny<List<Company>>()), Times.Once);
        }

        [Fact]
        public async Task GetAllCompaniesAsync_ShouldReturnEmptyList_WhenNoCompaniesInCacheOrDb()
        {
            _daprCacheServiceMock.Setup(x => x.GetAllCompaniesAsync()).ReturnsAsync((List<Company>?)null);
            _repositoryMock.Setup(x => x.getAllCompaniesAsync()).ReturnsAsync(new List<Company>());
            var result = await _command.GetAllCompaniesAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(x => x.getAllCompaniesAsync(), Times.Once);
            _daprCacheServiceMock.Verify(x => x.SetAllCompaniesAsync(It.IsAny<List<Company>>()), Times.Once);
        }

        [Fact]
        public async Task GetAllCompaniesAsync_ShouldHandleException_WhenRepositoryThrows()
        {
            _daprCacheServiceMock.Setup(x => x.GetAllCompaniesAsync()).ReturnsAsync((List<Company>?)null);
            _repositoryMock.Setup(x => x.getAllCompaniesAsync()).ThrowsAsync(new Exception("Database error"));
            await Assert.ThrowsAsync<Exception>(async () => await _command.GetAllCompaniesAsync());
            _repositoryMock.Verify(x => x.getAllCompaniesAsync(), Times.Once);
        }
    }
}
