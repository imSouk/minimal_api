using minimal_api.api.Domain.DTO;
using minimal_api.api.Domain.Entities;

namespace minimal_api.api.Domain.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDto logindto);
        List<Administrator> GetAdministrator(int pagina);
        Administrator? Add(Administrator administrator);
        Administrator? FindById(int id); 

    }
}
