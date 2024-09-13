using ABP_Task_Zakharov.Models;

namespace ABP_Task_Zakharov.Interfaces
{
    public interface IConferenceRoomService
    {
        Task<ConferenceRoom> AddConferenceRoomAsync(ConferenceRoom room);
        Task<ConferenceRoom?> GetConferenceRoomByIdAsync(int id);
        Task<List<ConferenceRoom>> GetAllConferenceRoomsAsync();
        Task<bool> UpdateConferenceRoomAsync(ConferenceRoom room);
        Task<bool> DeleteConferenceRoomAsync(int id);
        Task<List<ConferenceRoom>> SearchAvailableRoomsAsync(DateTime startTime, DateTime endTime, int capacity);
    }
}
