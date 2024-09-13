using ABP_Task_Zakharov.Models;

namespace ABP_Task_Zakharov.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> AddBookingAsync(Booking booking);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);
        Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId);
        Task<List<Booking>> GetBookingsByDateAndTime(DateTime startDate, DateTime endDate);
    }
}
