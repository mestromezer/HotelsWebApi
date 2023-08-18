public class HotelRepository : IHotelRepository
{
    private HotelDb _context;
    private bool _disposed;

    public HotelRepository(HotelDb context)
    {
        _disposed = false;
        _context = context;
    }
    public  Task<List<Hotel>> GetHotelAsync()=> _context.Hotels.ToListAsync();
    public async Task<Hotel> GetHotelAsync(int hotelId) =>  await _context.Hotels.FindAsync(new object[]{hotelId});
    public  Task<List<Hotel>> GetHotelsAsync(string query)=>  _context.Hotels.Where(h => h.Name.Contains(query)).ToListAsync();

    public async Task InsertHotelAsync(Hotel hotel) =>  await _context.Hotels.AddAsync(hotel);

    public async Task UpdateHotelAsync(Hotel hotel,int id)
    {
        var fromDbHotel = await _context.Hotels.FindAsync(new object[]{id});
        if(fromDbHotel == null){return ;}
        fromDbHotel.Name = hotel.Name;
        fromDbHotel.CostPerNight = hotel.CostPerNight;
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
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    
}