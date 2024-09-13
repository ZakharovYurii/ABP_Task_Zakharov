namespace ABP_Task_Zakharov.Models
{
    public class Discount
    {
        public int Id { get; set; } // Ідентифікатор знижки
        public string Name { get; set; } // Назва знижки
        public decimal Percentage { get; set; } // Відсоток знижки
        public DiscountType Type { get; set; } // Тип знижки (на оренду залу або загальну)
        public TimeSpan StartTime { get; set; } // Початок періоду знижки
        public TimeSpan EndTime { get; set; } // Кінець періоду знижки
    }

    public enum DiscountType
    {
        RoomOnly, // Знижка лише на оренду залу
        TotalPrice // Знижка на загальну вартість (зал + послуги)
    }
}
