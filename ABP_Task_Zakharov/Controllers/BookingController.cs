using ABP_Task_Zakharov.Interfaces;
using ABP_Task_Zakharov.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABP_Task_Zakharov.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Додати нове бронювання
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] Booking bookingRequest)
        {
            if (bookingRequest == null)
            {
                return BadRequest("Booking data is null.");
            }

            var bookingResult = await _bookingService.AddBookingAsync(bookingRequest);
            if (bookingResult == null)
            {
                return BadRequest("Failed to create booking. Room might be unavailable.");
            }

            return CreatedAtAction(nameof(GetBookingById), new { id = bookingResult.Id }, bookingResult);
        }

        // Отримати бронювання за ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        // Отримати всі бронювання
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        // Оновити бронювання
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking updatedBooking)
        {
            if (updatedBooking == null || updatedBooking.Id != id)
            {
                return BadRequest("Booking data is incorrect.");
            }

            var existingBooking = await _bookingService.GetBookingByIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            var success = await _bookingService.UpdateBookingAsync(updatedBooking);
            if (!success)
            {
                return StatusCode(500, "An error occurred while updating the booking.");
            }

            return NoContent(); // Повернути 204 No Content при успішному оновленні
        }

        // Видалити бронювання
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var success = await _bookingService.DeleteBookingAsync(id);
            if (!success)
            {
                return StatusCode(500, "An error occurred while deleting the booking.");
            }

            return NoContent(); // Повернути 204 No Content при успішному видаленні
        }

        // Пошук бронювань за датою
        [HttpGet("search")]
        public async Task<IActionResult> SearchBookingsByDate(DateTime startTime, DateTime endTime)
        {
            var bookings = await _bookingService.GetBookingsByDateAndTime(startTime, endTime);
            if (bookings == null || !bookings.Any())
            {
                return NotFound("No bookings found for the specified dates.");
            }

            return Ok(bookings);
        }
    }
}
