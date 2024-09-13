using ABP_Task_Zakharov.Data;
using ABP_Task_Zakharov.Interfaces;
using ABP_Task_Zakharov.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ABP_Task_Zakharov.Services
{
    public class ConferenceRoomService : IConferenceRoomService
    {
        private readonly IMongoCollection<ConferenceRoom> _conferenceRooms;
        private readonly IMongoCollection<Booking> _bookings;

        public ConferenceRoomService(IMongoDatabase database)
        {
            _conferenceRooms = database.GetCollection<ConferenceRoom>("ConferenceRooms");
            _bookings = database.GetCollection<Booking>("Bookings");
        }

        public async Task<ConferenceRoom> AddConferenceRoomAsync(ConferenceRoom room)
        {
            await _conferenceRooms.InsertOneAsync(room);
            return room;
        }

        public async Task<ConferenceRoom?> GetConferenceRoomByIdAsync(int id)
        {
            return await _conferenceRooms.Find(room => room.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<ConferenceRoom>> GetAllConferenceRoomsAsync()
        {
            return await _conferenceRooms.Find(room => true).ToListAsync();
        }

        public async Task<bool> UpdateConferenceRoomAsync(ConferenceRoom updatedRoom)
        {
            var result = await _conferenceRooms.ReplaceOneAsync(room => room.Id == updatedRoom.Id, updatedRoom);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteConferenceRoomAsync(int id)
        {
            var result = await _conferenceRooms.DeleteOneAsync(room => room.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<ConferenceRoom>> SearchAvailableRoomsAsync(DateTime startTime, DateTime endTime, int capacity)
        {
            // Отримуємо всі кімнати
            var rooms = await _conferenceRooms.Find(r => r.Capacity >= capacity).ToListAsync();

            // Отримуємо всі бронювання, які перетинаються з вказаними датами
            var overlappingBookings = await _bookings
                .Find(b => b.BookingStart < endTime && b.BookingEnd > startTime)
                .ToListAsync();

            // Видаляємо кімнати, які вже заброньовані
            var availableRooms = rooms
                .Where(r => !overlappingBookings.Any(b => b.ConferenceRoomId == r.Id))
                .ToList();

            return availableRooms;
        }
    }
}
