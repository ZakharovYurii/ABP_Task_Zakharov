using ABP_Task_Zakharov.Interfaces;
using ABP_Task_Zakharov.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABP_Task_Zakharov.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConferenceRoomsController : ControllerBase
    {
        private readonly IConferenceRoomService _conferenceRoomService;

        public ConferenceRoomsController(IConferenceRoomService conferenceRoomService)
        {
            _conferenceRoomService = conferenceRoomService;
        }

        // Додати нову кімнату
        [HttpPost]
        public async Task<IActionResult> CreateConferenceRoom([FromBody] ConferenceRoom conferenceRoom)
        {
            if (conferenceRoom == null)
            {
                return BadRequest("Conference room data is null.");
            }

            var createdRoom = await _conferenceRoomService.AddConferenceRoomAsync(conferenceRoom);
            return CreatedAtAction(nameof(GetConferenceRoomById), new { id = createdRoom.Id }, createdRoom);
        }

        // Отримати кімнату за ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConferenceRoomById(int id)
        {
            var room = await _conferenceRoomService.GetConferenceRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // Отримати всі кімнати
        [HttpGet]
        public async Task<IActionResult> GetAllConferenceRooms()
        {
            var rooms = await _conferenceRoomService.GetAllConferenceRoomsAsync();
            return Ok(rooms);
        }

        // Оновити інформацію про кімнату
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConferenceRoom(int id, [FromBody] ConferenceRoom updatedRoom)
        {
            if (updatedRoom == null || updatedRoom.Id != id)
            {
                return BadRequest("Room data is incorrect.");
            }

            var existingRoom = await _conferenceRoomService.GetConferenceRoomByIdAsync(id);
            if (existingRoom == null)
            {
                return NotFound();
            }

            var success = await _conferenceRoomService.UpdateConferenceRoomAsync(updatedRoom);
            if (!success)
            {
                return StatusCode(500, "An error occurred while updating the room.");
            }

            return NoContent(); // Повернути 204 No Content при успішному оновленні
        }

        // Видалити кімнату
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConferenceRoom(int id)
        {
            var room = await _conferenceRoomService.GetConferenceRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var success = await _conferenceRoomService.DeleteConferenceRoomAsync(id);
            if (!success)
            {
                return StatusCode(500, "An error occurred while deleting the room.");
            }

            return NoContent(); // Повернути 204 No Content при успішному видаленні
        }

        // Пошук доступних кімнат за датою, часом та місткістю
        [HttpGet("search")]
        public async Task<IActionResult> SearchAvailableRooms(DateTime startTime, DateTime endTime, int capacity)
        {
            var rooms = await _conferenceRoomService.SearchAvailableRoomsAsync(startTime, endTime, capacity);
            if (rooms == null || !rooms.Any())
            {
                return NotFound("No available rooms found for the specified criteria.");
            }

            return Ok(rooms);
        }
    }
}
