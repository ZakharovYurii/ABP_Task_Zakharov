namespace ABP_Task_Zakharov.Models
{
    public class ConferenceRoom
    {
        public int Id { get; set; } // Ідентифікатор залу
        public string Name { get; set; } // Назва залу 
        public int Capacity { get; set; } // Місткість 
        public decimal BasePricePerHour { get; set; } // Базова вартість
        public List<Service> Services { get; set; } = new List<Service>(); // Список доступних послуг
    }
}
