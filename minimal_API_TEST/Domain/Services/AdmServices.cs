using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.api.Domain.Entities;
using minimal_api.api.Infrastruct.DB;
using minimal_api.api.Infrastruct.Service;
namespace minimal_API_TEST.Domain.Entidades
{
    [TestClass]
    public  class AdmServiceTest
    {
        private StorageBroker CreateTestDbContext() 
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
           var configuration = builder.Build(); 
            return new StorageBroker(configuration);
        }
        [TestMethod]
        public void TesteService()
        {
            //Arrange
            var _broker = CreateTestDbContext();
            _broker.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");
            var adm = new Administrator();
            adm.Email = "test@test.com";
            adm.Senha = "123";
            adm.Perfil = "Adm";
            var admistratorService = new AdministratorService(_broker);

            //Act
            admistratorService.Add(adm);
            //Assert
            Assert.AreEqual("Adm", adm.Perfil);
            Assert.AreEqual("test@test.com", adm.Email);
            Assert.AreEqual("123", adm.Senha);
            Assert.AreEqual(1, admistratorService.GetAdministrator(1).Count());
        }
        public void TesteServiceID()
        {
            //Arrange
            var _broker = CreateTestDbContext();
            _broker.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");
            var adm = new Administrator();
            adm.Email = "test@test.com";
            adm.Senha = "123";
            adm.Perfil = "Adm";
            var admistratorService = new AdministratorService(_broker);

            //Act
            admistratorService.Add(adm);
            var adm1 = admistratorService.FindById(adm.Id);
            //Assert
            Assert.AreEqual(1, adm1?.Id);
        }
    }
}
