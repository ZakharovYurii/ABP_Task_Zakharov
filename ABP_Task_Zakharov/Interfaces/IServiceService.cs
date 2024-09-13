using ABP_Task_Zakharov.Models;

namespace ABP_Task_Zakharov.Interfaces
{
    public interface IServiceService
    {
        Task<Service> AddServiceAsync(Service service);
        Task<Service?> GetServiceByIdAsync(int id);
        Task<List<Service>> GetAllServicesAsync();
        Task<bool> UpdateServiceAsync(Service service);
        Task<bool> DeleteServiceAsync(int id);
    }
}
