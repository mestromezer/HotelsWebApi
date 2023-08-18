public interface IHotelRepository: IDisposable
{
    Task<List<Hotel>> GetHotelAsync();
    Task<Hotel> GetHotelAsync(int id);
    Task InsertHotelAsync(Hotel hotel);
    Task UpdateHotelAsync(Hotel hotel);
    Task DeleteHotelAsync(int id);
    Task SaveAsync();
}