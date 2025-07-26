using minimal_api.api.Domain.DTO;
using minimal_api.api.Domain.Entities;
using minimal_api.api.Domain.Interfaces;
using minimal_api.api.Infrastruct.DB;

namespace minimal_api.api.Infrastruct.Service
{
    public class AdministratorService : IAdministratorService
    {
        public readonly StorageBroker _broker;
        public AdministratorService(StorageBroker broker)
        {
            _broker = broker;               
        }
        public Administrator Login(LoginDto logindto)
        {
            Administrator adm = _broker.Administrators.Where(a => a.Email == logindto.Email && a.Senha == logindto.Senha).FirstOrDefault();
            return adm;
        }

        public Administrator? Add(Administrator administrator)
        {
            _broker.Administrators.Add(administrator);
            _broker.SaveChanges();
                return administrator;
        }

        public List<Administrator> GetAdministrator(int pagina)
        {
            var query = _broker.Administrators.AsQueryable();
         
            int qtPag = 10;
            query = query.Skip((pagina - 1) * qtPag).Take(qtPag);
            return query.ToList();
        }
        public Administrator? FindById(int id)
        {

            return _broker.Administrators.FirstOrDefault(v => v.Id == id);

        }

    }
}
