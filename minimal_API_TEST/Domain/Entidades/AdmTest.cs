using minimal_api.api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minimal_API_TEST.Domain.Entidades
{
    [TestClass]
    public  class AdmTeste
    {
        [TestMethod]
        public void TestGetSetProps()
        {
            //Arrange
            var adm = new Administrator();
            //Act
            adm.Email = "test@test.com";
            adm.Senha = "123";
            adm.Perfil = "Adm";
            adm.Id = 1;

            //Assert
            Assert.AreEqual("Adm", adm.Perfil);
            Assert.AreEqual("test@test.com", adm.Email);
            Assert.AreEqual("123", adm.Senha);
            Assert.AreEqual(1, adm.Id);
        }
    }
}
