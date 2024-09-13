using ABP_Task_Zakharov.Interfaces;
using ABP_Task_Zakharov.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABP_Task_Zakharov.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        // Додати нову послугу
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] Service service)
        {
            if (service == null)
            {
                return BadRequest("Service data is null.");
            }

            var createdService = await _serviceService.AddServiceAsync(service);
            return CreatedAtAction(nameof(GetServiceById), new { id = createdService.Id }, createdService);
        }

        // Отримати послугу за ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return Ok(service);
        }

        // Отримати всі послуги
        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }

        // Оновити інформацію про послугу
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] Service updatedService)
        {
            if (updatedService == null || updatedService.Id != id)
            {
                return BadRequest("Service data is incorrect.");
            }

            var existingService = await _serviceService.GetServiceByIdAsync(id);
            if (existingService == null)
            {
                return NotFound();
            }

            var success = await _serviceService.UpdateServiceAsync(updatedService);
            if (!success)
            {
                return StatusCode(500, "An error occurred while updating the service.");
            }

            return NoContent(); // Повернути 204 No Content при успішному оновленні
        }

        // Видалити послугу
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var success = await _serviceService.DeleteServiceAsync(id);
            if (!success)
            {
                return StatusCode(500, "An error occurred while deleting the service.");
            }

            return NoContent(); // Повернути 204 No Content при успішному видаленні
        }
    }
}
