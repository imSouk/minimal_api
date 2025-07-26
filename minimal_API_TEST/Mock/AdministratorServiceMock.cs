using minimal_api.api.Domain.DTO;
using minimal_api.api.Domain.Entities;
using minimal_api.api.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minimal_API_TEST.Mock
{
    public class AdministratorServiceMock : IAdministratorService
    {
        private static List<Administrator> administrators = new List<Administrator>() 
        {
            new Administrator
            {
                Id=1,
                Email ="adm@teste.com",
                Senha = "123456",
                Perfil = "Adm"

            },
            new Administrator
            {
                Id=2,
                Email ="Editor@teste.com",
                Senha = "123456",
                Perfil = "Editor"

            },
        };
        public Administrator? Add(Administrator administrator)
        {
            administrator.Id = administrators.Count();
            administrators.Add(administrator);
            return administrator;   
        }

        public Administrator? FindById(int id)
        {
            return administrators.Find(a => a.Id == id);
        }

        public List<Administrator> GetAdministrator(int pagina)
        {
            return administrators;
        }

        public Administrator? Login(LoginDto logindto)
        {
            return  administrators.Find(a => a.Email == logindto.Email && a.Senha == logindto.Senha);
            
        }
    }
}
