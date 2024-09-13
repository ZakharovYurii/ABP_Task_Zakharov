using ABP_Task_Zakharov.Models;

namespace ABP_Task_Zakharov.Interfaces
{
    public interface IDiscountService
    {
        Task<Discount> AddDiscountAsync(Discount discount);
        Task<Discount?> GetDiscountByIdAsync(int id);
        Task<List<Discount>> GetAllDiscountsAsync();
        Task<bool> UpdateDiscountAsync(Discount discount);
        Task<bool> DeleteDiscountAsync(int id);
        Task<List<Discount>> GetApplicableDiscountsAsync(DateTime startTime, DateTime endTime);
    }
}
