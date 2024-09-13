using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABP_Task_Zakharov.Interfaces;
using ABP_Task_Zakharov.Models;

namespace ABP_Task_Zakharov.Services
{
    public class AnalyticsService
    {
        private readonly IBookingService _bookingService;
        private readonly IConferenceRoomService _conferenceRoomService;

        // Зберігаємо кількість викликів функцій
        private Dictionary<string, int> functionUsage = new Dictionary<string, int>();

        public AnalyticsService(IBookingService bookingService, IConferenceRoomService conferenceRoomService)
        {
            _bookingService = bookingService;
            _conferenceRoomService = conferenceRoomService;
        }

        // Логування викликів функцій
        private void LogFunctionCall(string functionName)
        {
            if (functionUsage.ContainsKey(functionName))
            {
                functionUsage[functionName]++;
            }
            else
            {
                functionUsage[functionName] = 1;
            }
        }

        // Функція для створення аналітичного файлу
        public async Task GenerateAnalyticsReportAsync(string filePath)
        {
            LogFunctionCall(nameof(GenerateAnalyticsReportAsync));

            // Отримуємо всі бронювання
            var bookings = await _bookingService.GetAllBookingsAsync();
            var rooms = await _conferenceRoomService.GetAllConferenceRoomsAsync();

            // Статистика по залах
            var roomBookings = bookings.GroupBy(b => b.ConferenceRoomId)
                .Select(group => new
                {
                    RoomId = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(r => r.Count)
                .ToList();

            // Найпопулярніші години
            var popularHours = bookings.SelectMany(b =>
                Enumerable.Range(b.BookingStart.Hour, (int)(b.BookingEnd - b.BookingStart).TotalHours))
                .GroupBy(hour => hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(h => h.Count)
                .ToList();

            // Статистика по послугах
            var serviceBookings = bookings.SelectMany(b => b.SelectedServices)
                .GroupBy(service => service.Name)
                .Select(group => new
                {
                    ServiceName = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToList();

            // Генерація звіту
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Виведення статистики по функціях
                await writer.WriteLineAsync("Статистика викликів функцій:");
                foreach (var function in functionUsage)
                {
                    await writer.WriteLineAsync($"{function.Key}: {function.Value} разів");
                }

                // Виведення найчастіше заброньованих залів
                await writer.WriteLineAsync("\nЗали, що були заброньовані найбільше разів:");
                foreach (var roomBooking in roomBookings)
                {
                    var room = rooms.FirstOrDefault(r => r.Id == roomBooking.RoomId);
                    if (room != null)
                    {
                        await writer.WriteLineAsync($"{room.Name}: {roomBooking.Count} разів");
                    }
                }

                // Виведення найпопулярніших годин
                await writer.WriteLineAsync("\nНайпопулярніші години бронювань:");
                foreach (var hour in popularHours)
                {
                    await writer.WriteLineAsync($"{hour.Hour}: {hour.Count} бронювань");
                }

                // Виведення кількості бронювань послуг
                await writer.WriteLineAsync("\nПослуги, що були заброньовані найбільше разів:");
                foreach (var serviceBooking in serviceBookings)
                {
                    await writer.WriteLineAsync($"{serviceBooking.ServiceName}: {serviceBooking.Count} разів");
                }
            }
        }
    }
}