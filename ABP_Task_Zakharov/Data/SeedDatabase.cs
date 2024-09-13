namespace ABP_Task_Zakharov.Data
{
    using MongoDB.Driver;
    using ABP_Task_Zakharov.Models;
    using System;
    using ABP_Task_Zakharov.Services;
    using ABP_Task_Zakharov.Interfaces;

    public class SeedDatabase
    {
        private readonly IMongoCollection<ConferenceRoom> _rooms;
        private readonly IMongoCollection<Service> _services;
        private readonly IMongoCollection<Discount> _discounts;
        private readonly IMongoCollection<Booking> _bookings;
        private readonly IBookingService _bookingService; // Використовуємо сервіс бронювання

        public SeedDatabase(IMongoDatabase database, IBookingService bookingService)
        {
            _rooms = database.GetCollection<ConferenceRoom>("ConferenceRooms");
            _services = database.GetCollection<Service>("Services");
            _discounts = database.GetCollection<Discount>("Discounts");
            _bookings = database.GetCollection<Booking>("Bookings");
            _bookingService = bookingService; // Отримуємо сервіс бронювання
        }

        public async Task SeedDataAsync()
        {
            // Clear existing data
            await _rooms.DeleteManyAsync(Builders<ConferenceRoom>.Filter.Empty);
            await _services.DeleteManyAsync(Builders<Service>.Filter.Empty);
            await _discounts.DeleteManyAsync(Builders<Discount>.Filter.Empty);
            await _bookings.DeleteManyAsync(Builders<Booking>.Filter.Empty);

            // Додаємо послуги
            var projector = new Service { Id = 1, Name = "Проєктор", Price = 500 };
            var wifi = new Service { Id = 2, Name = "Wi-Fi", Price = 300 };
            var sound = new Service { Id = 3, Name = "Звук", Price = 700 };

            await _services.InsertManyAsync(new List<Service> { projector, wifi, sound });

            // Додаємо зали
            var rooms = new List<ConferenceRoom>
        {
            new ConferenceRoom
            {
                Id = 1,
                Name = "Зал А",
                Capacity = 50,
                BasePricePerHour = 2000,
                Services = new List<Service> { projector, wifi } // Додаємо послуги до залу А
            },
            new ConferenceRoom
            {
                Id = 2,
                Name = "Зал B",
                Capacity = 100,
                BasePricePerHour = 3500,
                Services = new List<Service> { sound } // Додаємо послуги до залу B
            },
            new ConferenceRoom
            {
                Id = 3,
                Name = "Зал C",
                Capacity = 30,
                BasePricePerHour = 1500,
                Services = new List<Service> { wifi, sound } // Додаємо послуги до залу C
            }
        };

            await _rooms.InsertManyAsync(rooms);

            // Додаємо знижки
            var discounts = new List<Discount>
        {
            new Discount
            {
                Id = 1,
                Name = "Стандартні години",
                Percentage = 0, // Немає знижки
                Type = DiscountType.RoomOnly,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(18, 0, 0)
            },
            new Discount
            {
                Id = 2,
                Name = "Вечірні години",
                Percentage = 20, // Знижка 20%
                Type = DiscountType.RoomOnly,
                StartTime = new TimeSpan(18, 0, 0),
                EndTime = new TimeSpan(23, 0, 0)
            },
            new Discount
            {
                Id = 3,
                Name = "Ранкові години",
                Percentage = 10, // Знижка 10%
                Type = DiscountType.TotalPrice,
                StartTime = new TimeSpan(6, 0, 0),
                EndTime = new TimeSpan(9, 0, 0)
            },
            new Discount
            {
                Id = 4,
                Name = "Пікові години",
                Percentage = -15, // Націнка 15%
                Type = DiscountType.TotalPrice,
                StartTime = new TimeSpan(12, 0, 0),
                EndTime = new TimeSpan(14, 0, 0)
            }
        };

            await _discounts.InsertManyAsync(discounts);

            // Використовуємо сервіс для додавання бронювань з розрахунком ціни
            var booking1 = new Booking
            {
                Id = 1,
                ConferenceRoomId = 1,
                BookingStart = new DateTime(2024, 9, 12, 10, 0, 0), // 12 вересня 2024, 10:00
                BookingEnd = new DateTime(2024, 9, 12, 12, 0, 0), // 12 вересня 2024, 12:00
                SelectedServices = new List<Service> { projector, wifi },
                TotalPrice = 0 // Ціна буде розрахована
            };

            var booking2 = new Booking
            {
                Id = 2,
                ConferenceRoomId = 2,
                BookingStart = new DateTime(2024, 9, 13, 15, 0, 0), // 13 вересня 2024, 15:00
                BookingEnd = new DateTime(2024, 9, 13, 17, 0, 0), // 13 вересня 2024, 17:00
                SelectedServices = new List<Service> { wifi },
                TotalPrice = 0 // Ціна буде розрахована
            };

            await _bookingService.AddBookingAsync(booking1); // Додаємо бронювання через сервіс
            await _bookingService.AddBookingAsync(booking2);

            Console.WriteLine("Дані успішно додані до бази даних.");
        }
    }
}
