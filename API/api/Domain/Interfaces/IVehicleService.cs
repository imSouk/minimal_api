using minimal_api.api.Domain.Entities;

namespace minimal_api.api.Domain.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle> getAll(int pagina = 1, string? nome=null, string? marca=null);
        Vehicle FindById (int id);
        Vehicle Save(Vehicle vehicle);
        Vehicle Update(Vehicle vehicle);
        Vehicle Delete(Vehicle vehicle);

    }
}
