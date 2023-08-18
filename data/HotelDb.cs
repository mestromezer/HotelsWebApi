class HotelDb: DbContext
{
    public HotelDb(DbContextOptions options):base(options){}
    public DbSet<Hotel> Hotels{get;set;}
}
