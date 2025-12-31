using Application;
using Application.Services;
using Database;
using Domain;
using Domain.DTO;
using Moq;
using Xunit;

namespace Application.Test
{
    public class GetCompanyByIdCommandTests
    {
        private readonly Mock<ICompanyRepository<Company>> _repositoryMock;
        private readonly Mock<IDaprCacheService> _daprCacheServiceMock;

        private readonly GetCompanyByIdCommand _command;

        public GetCompanyByIdCommandTests()
        {
            _repositoryMock = new Mock<ICompanyRepository<Company>>();
            _daprCacheServiceMock = new Mock<IDaprCacheService>();

            _command = new GetCompanyByIdCommand(
                _repositoryMock.Object,
                _daprCacheServiceMock.Object);
        }

        [Fact]
        public async Task GetCompanyByIdAsync_ShouldReturnCompany_WhenInCache()
        {
            var companyId = 1L;
            var cachedCompany = new Company
            {
                Id = companyId,
                Name = "Cached Company",
                Vat = "VAT123"
            };

            _daprCacheServiceMock.Setup(x => x.GetCompanyAsync(companyId)).ReturnsAsync(cachedCompany);

            var result = await _command.GetCompanyByIdAsync(companyId);

            Assert.NotNull(result);
            Assert.Equal(cachedCompany.Id, result.Id);

            // Vérifie qu'on n'a PAS appelé la base de données (car trouvé dans le cache)
            _repositoryMock.Verify(x => x.getCompanyByIdAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task GetCompanyByIdAsync_ShouldFetchFromDbAndCacheIt_WhenNotInCache()
        {
            var companyId = 2L;
            var dbCompany = new Company
            {
                Id = companyId,
                Name = "DB Company",
                Vat = "VAT456"
            };

            _daprCacheServiceMock.Setup(x => x.GetCompanyAsync(companyId)).ReturnsAsync((Company?)null);
            _repositoryMock.Setup(x => x.getCompanyByIdAsync(companyId)).ReturnsAsync(dbCompany);

            var result = await _command.GetCompanyByIdAsync(companyId);

            Assert.NotNull(result);
            Assert.Equal(dbCompany.Id, result.Id);

            _repositoryMock.Verify(x => x.getCompanyByIdAsync(companyId), Times.Once);
            _daprCacheServiceMock.Verify(x => x.SetCompanyAsync(dbCompany), Times.Once);
        }

        [Fact]
        public async Task GetCompanyByIdAsync_ShouldReturnNull_WhenNotFoundAnywhere()
        {
            var companyId = 3L;
            _daprCacheServiceMock.Setup(x => x.GetCompanyAsync(companyId)).ReturnsAsync((Company?)null);
            _repositoryMock.Setup(x => x.getCompanyByIdAsync(companyId)).ReturnsAsync((Company?)null);

            var result = await _command.GetCompanyByIdAsync(companyId);

            Assert.Null(result);
        }
    }
}
