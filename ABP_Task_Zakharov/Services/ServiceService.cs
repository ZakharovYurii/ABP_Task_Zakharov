using ABP_Task_Zakharov.Interfaces;
using ABP_Task_Zakharov.Models;
using MongoDB.Driver;

namespace ABP_Task_Zakharov.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IMongoCollection<Service> _services;

        public ServiceService(IMongoDatabase database)
        {
            _services = database.GetCollection<Service>("Services");
        }

        public async Task<Service> AddServiceAsync(Service service)
        {
            await _services.InsertOneAsync(service);
            return service;
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _services.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return await _services.Find(s => true).ToListAsync();
        }

        public async Task<bool> UpdateServiceAsync(Service updatedService)
        {
            var result = await _services.ReplaceOneAsync(s => s.Id == updatedService.Id, updatedService);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var result = await _services.DeleteOneAsync(s => s.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
