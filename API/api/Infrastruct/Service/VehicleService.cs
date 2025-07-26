using Microsoft.EntityFrameworkCore;
using minimal_api.api.Domain.Entities;
using minimal_api.api.Domain.Interfaces;
using minimal_api.api.Infrastruct.DB;

namespace minimal_api.api.Infrastruct.Service
{
    public class VehicleService : IVehicleService
    {
        public readonly StorageBroker _broker;
        public VehicleService(StorageBroker broker)
        {
            _broker = broker;
        }

        public Vehicle Delete(Vehicle vehicle)
        {
            _broker.Remove(vehicle);
            _broker.SaveChanges();
            return vehicle;
        }

        public Vehicle FindById(int id)
        {

            return _broker.Vehicles.FirstOrDefault(v => v.Id == id);
             
        }

        public List<Vehicle> getAll(int pagina, string? nome, string? marca)
        {
               var query = _broker.Vehicles.AsQueryable();  
            if (nome != null)
            {
                query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%"));
            }
            int qtPag = 10;
            query = query.Skip((pagina - 1) * qtPag).Take(qtPag);
            return query.ToList();
        }

        public Vehicle Save(Vehicle vehicle)
        {
            _broker.Add(vehicle);
            _broker.SaveChanges();
            return vehicle;
        }

        public Vehicle Update(Vehicle vehicle)
        {
            _broker.Vehicles.Update(vehicle);    
            _broker.SaveChanges();
            return vehicle;
        }
    }
}
