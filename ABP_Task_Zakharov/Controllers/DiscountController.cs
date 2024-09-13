using ABP_Task_Zakharov.Interfaces;
using ABP_Task_Zakharov.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABP_Task_Zakharov.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        // Додати нову знижку
        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromBody] Discount discount)
        {
            if (discount == null)
            {
                return BadRequest("Discount data is null.");
            }

            var createdDiscount = await _discountService.AddDiscountAsync(discount);
            return CreatedAtAction(nameof(GetDiscountById), new { id = createdDiscount.Id }, createdDiscount);
        }

        // Отримати знижку за ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountById(int id)
        {
            var discount = await _discountService.GetDiscountByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            return Ok(discount);
        }

        // Отримати всі знижки
        [HttpGet]
        public async Task<IActionResult> GetAllDiscounts()
        {
            var discounts = await _discountService.GetAllDiscountsAsync();
            return Ok(discounts);
        }

        // Оновити знижку
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] Discount updatedDiscount)
        {
            if (updatedDiscount == null || updatedDiscount.Id != id)
            {
                return BadRequest("Discount data is incorrect.");
            }

            var existingDiscount = await _discountService.GetDiscountByIdAsync(id);
            if (existingDiscount == null)
            {
                return NotFound();
            }

            var success = await _discountService.UpdateDiscountAsync(updatedDiscount);
            if (!success)
            {
                return StatusCode(500, "An error occurred while updating the discount.");
            }

            return NoContent(); // Повернути 204 No Content при успішному оновленні
        }

        // Видалити знижку
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var discount = await _discountService.GetDiscountByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            var success = await _discountService.DeleteDiscountAsync(id);
            if (!success)
            {
                return StatusCode(500, "An error occurred while deleting the discount.");
            }

            return NoContent(); // Повернути 204 No Content при успішному видаленні
        }
    }
}
