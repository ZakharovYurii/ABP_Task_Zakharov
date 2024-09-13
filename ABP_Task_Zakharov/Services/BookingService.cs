using ABP_Task_Zakharov.Interfaces;
using ABP_Task_Zakharov.Models;
using MongoDB.Driver;

namespace ABP_Task_Zakharov.Services
{
    public class BookingService : IBookingService
    {
        private readonly IMongoCollection<Booking> _bookings;
        
        private readonly IConferenceRoomService _conferenceRoomService;
        private readonly IServiceService _serviceService;
        private readonly IDiscountService _discountService;

        public BookingService(IMongoDatabase database, IConferenceRoomService conferenceRoomService, IServiceService serviceService, IDiscountService discountService)
        {
            _bookings = database.GetCollection<Booking>("Bookings");            
            _conferenceRoomService = conferenceRoomService;
            _serviceService = serviceService;
            _discountService = discountService;
        }

        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            var discounts = await _discountService.GetAllDiscountsAsync();
            if (booking.BookingStart.Date != booking.BookingEnd.Date)
            {
                throw new Exception("Початок і кінець бронювання мають бути в один і той же день");
            }
            var minStartTime = discounts.Min(d => d.StartTime);
            var maxEndTime = discounts.Max(d => d.EndTime);
            
            if (booking.BookingStart.Hour < minStartTime.Hours || booking.BookingEnd.Hour > maxEndTime.Hours)
                throw new Exception("Обрано занадто ранню чи занадто пізню годину для бронювання");

            // Отримуємо зал для бронювання
            var room = await _conferenceRoomService.GetConferenceRoomByIdAsync(booking.ConferenceRoomId);
            if (room == null) throw new Exception("Зал не знайдено");

            // Перевіряємо доступність залу на вказаний час
            bool isRoomAvailable = await CheckRoomAvailabilityAsync(booking.ConferenceRoomId, booking.BookingStart, booking.BookingEnd);
            if (!isRoomAvailable) throw new Exception("Зал зайнятий на цей час");  
            
            // Розраховуємо загальну вартість без знижок
            decimal baseTotalPrice = CalculateBaseTotalPrice(room, booking);

            // Перевіряємо доступні знижки на вказаний час бронювання
            var applicableDiscounts = await _discountService.GetApplicableDiscountsAsync(booking.BookingStart, booking.BookingEnd);

            // Розраховуємо загальну вартість з урахуванням знижок
            booking.TotalPrice = ApplyDiscounts(applicableDiscounts, booking, room);

            // Додаємо бронювання до бази даних
            await _bookings.InsertOneAsync(booking);
            return booking;
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _bookings.Find(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _bookings.Find(b => true).ToListAsync();
        }

        public async Task<bool> UpdateBookingAsync(Booking updatedBooking)
        {
            var result = await _bookings.ReplaceOneAsync(b => b.Id == updatedBooking.Id, updatedBooking);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var result = await _bookings.DeleteOneAsync(b => b.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId)
        {
            return await _bookings.Find(b => b.ConferenceRoomId == roomId).ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByDateAndTime(DateTime startDate, DateTime endDate)
        {
            return await _bookings.Find(b => b.BookingStart >= startDate && b.BookingEnd <= endDate).ToListAsync();
        }

        // Метод для перевірки доступності кімнати
        public async Task<bool> CheckRoomAvailabilityAsync(int roomId, DateTime startTime, DateTime endTime)
        {
            var bookings = await GetBookingsByRoomIdAsync(roomId);

            // Перевіряємо, чи не перекриваються бронювання
            return !bookings.Any(b => b.BookingStart < endTime && b.BookingEnd > startTime);
        }

        // Розрахунок базової ціни для залу та обраних послуг
        private decimal CalculateBaseTotalPrice(ConferenceRoom room, Booking booking)
        {
            // Розрахунок вартості залу
            var durationHours = (decimal)(booking.BookingEnd - booking.BookingStart).TotalHours;
            booking.TotalPrice = room.BasePricePerHour * durationHours;

            // Додаємо вартість кожної обраної послуги
            foreach (var service in booking.SelectedServices)
            {
                booking.TotalPrice += service.Price;
            }

            return booking.TotalPrice;
        }

        // Застосування знижок
        private decimal ApplyDiscounts(List<Discount> applicableDiscounts, Booking booking, ConferenceRoom room)
        {
            foreach (var discount in applicableDiscounts)
            {
                // Визначаємо початок та кінець періоду знижки, які накладаються на бронювання
                var discountStartHour = Math.Max(booking.BookingStart.Hour, discount.StartTime.Hours);
                var discountEndHour = Math.Min(booking.BookingEnd.Hour, discount.EndTime.Hours);
                

                if (discountStartHour < discountEndHour)
                {
                    var discountDuration = discountEndHour - discountStartHour; // Кількість годин для знижки

                    if (discount.Type == DiscountType.RoomOnly)
                    {
                        // Знижка лише на оренду залу
                        booking.TotalPrice -= room.BasePricePerHour * discountDuration * (discount.Percentage / 100);                        

                    }
                    else if (discount.Type == DiscountType.TotalPrice)
                    {
                        // Розрахунок загальної вартості залу та послуг
                        var totalRoomAndServices = room.BasePricePerHour * discountDuration;                        
                        foreach (var serv in booking.SelectedServices)
                        {
                            totalRoomAndServices += serv.Price;
                            
                        }                      

                        // Застосування знижки на загальну вартість
                        booking.TotalPrice -= totalRoomAndServices * (discount.Percentage / 100);
                        
                    }
                }
            }

            return booking.TotalPrice;
        }
    }
}