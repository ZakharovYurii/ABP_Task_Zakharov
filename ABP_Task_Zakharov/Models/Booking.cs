namespace ABP_Task_Zakharov.Models
{
    public class Booking
    {
        public int Id { get; set; } // Унікальний ідентифікатор бронювання
        public int ConferenceRoomId { get; set; } // ID заброньованого залу
        public DateTime BookingStart { get; set; } // Дата та час початку бронювання
        public DateTime BookingEnd { get; set; } // Дата та час кінця бронювання
        public List<Service> SelectedServices { get; set; } = new List<Service>(); // Список обраних послуг
        public decimal TotalPrice { get; set; } // Загальна вартість бронювання        
    }
}
