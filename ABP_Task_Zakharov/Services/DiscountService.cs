using ABP_Task_Zakharov.Interfaces;
using ABP_Task_Zakharov.Models;
using MongoDB.Driver;

namespace ABP_Task_Zakharov.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IMongoCollection<Discount> _discounts;

        public DiscountService(IMongoDatabase database)
        {
            _discounts = database.GetCollection<Discount>("Discounts");
        }

        public async Task<Discount> AddDiscountAsync(Discount discount)
        {
            await _discounts.InsertOneAsync(discount);
            return discount;
        }

        public async Task<Discount?> GetDiscountByIdAsync(int id)
        {
            return await _discounts.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Discount>> GetAllDiscountsAsync()
        {
            return await _discounts.Find(d => true).ToListAsync();
        }

        public async Task<bool> UpdateDiscountAsync(Discount updatedDiscount)
        {
            var result = await _discounts.ReplaceOneAsync(d => d.Id == updatedDiscount.Id, updatedDiscount);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteDiscountAsync(int id)
        {
            var result = await _discounts.DeleteOneAsync(d => d.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Discount>> GetApplicableDiscountsAsync(DateTime startTime, DateTime endTime)
        {
            // Отримання всіх знижок
            var discounts = await GetAllDiscountsAsync();

            // Виділення часу (години і хвилини) з початку та кінця бронювання
            var bookingStartTime = startTime.TimeOfDay;
            var bookingEndTime = endTime.TimeOfDay;

            // Фільтрація знижок, що підходять для часу бронювання
            var applicableDiscounts = discounts
                .Where(d => d.StartTime <= bookingEndTime && d.EndTime >= bookingStartTime)
                .ToList();

            return applicableDiscounts;
        }
    }
}
