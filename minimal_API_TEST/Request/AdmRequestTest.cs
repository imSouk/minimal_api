using minimal_api.api.Domain.DTO;
using minimal_api.api.Domain.Entities;
using minimal_API_TEST.Helpers;
using System.Net;
using System.Text.Json;
using System.Text;
using minimal_api.api.Domain.ModelViews;

namespace minimal_API_TEST.Request
{
    [TestClass]
    public class AdmRequestTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }
        [TestMethod]
        public async Task TestarGetSetPropriedades()
        {
            // Arrange
            var loginDTO = new LoginDto
            {
                Email = "adm@teste.com",
                Senha = "123456"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

            // Act
            var response = await Setup.client.PostAsync("/administrators/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var admLogin = JsonSerializer.Deserialize<AdmLogin>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admLogin.Email ?? "");
            Assert.IsNotNull(admLogin.Perfil ?? "");
            Assert.IsNotNull(admLogin.Token ?? "");
            Console.WriteLine(admLogin.Token);
        }
    }
}
