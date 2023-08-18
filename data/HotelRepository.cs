public class HotelRepository : IHotelRepository
{
    private HotelDb _context;
    private bool _disposed;

    public HotelRepository(HotelDb context)
    {
        _disposed = false;
        _context = context;
    }
    public async Task<List<Hotel>> GetHotelAsync()=> await _context.Hotels.ToListAsync();
    public async Task<Hotel> GetHotelAsync(int hotelId) => await _context.Hotels.FindAsync(new object[]{hotelId});

    public async Task InsertHotelAsync(Hotel hotel) => await _context.Hotels.AddAsync(hotel);

    public async Task UpdateHotelAsync(Hotel hotel)
    {
        var fromDbHotel = await _context.Hotels.FindAsync(new object[]{hotel.Id});
        if(fromDbHotel == null){return ;}
        fromDbHotel.Name = hotel.Name;
        fromDbHotel.CostPerNight = hotel.CostPerNight;
        //await _context.SaveChangesAsync();
    }
    public async Task DeleteHotelAsync(int id)
    {
        var fromDbHotel = await _context.Hotels.FindAsync(new object[]{id});
        if(fromDbHotel == null){return ;}
        _context.Hotels.Remove(fromDbHotel);
    }

    public async Task SaveAsync()=>await _context.SaveChangesAsync();

    protected virtual void Dispose(bool disposing)
    {
        if(!_disposed)
        {
            if(disposing)
            {
                _context.Dispose();
            }
            _disposed=true;
        }
    }
}