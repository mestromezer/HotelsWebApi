
//httprepl

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HotelDb>(options=>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HotelDb>();
    db.Database.EnsureCreated();
}


app.MapGet("/hotels", async (HotelDb db) => await db.Hotels.ToListAsync());
app.MapGet("/hotels/{id}", async (HotelDb db,int id) => 
await db.Hotels.FirstOrDefaultAsync(h=> h.Id == id) is Hotel hotel?
Results.Ok(hotel):
Results.NotFound()
);

app.MapPost("/hotels", async ([FromServices]HotelDb db, [FromBody]Hotel hotel, HttpResponse response) =>
{ 
    await db.AddAsync(hotel);
    await db.SaveChangesAsync();
    return Results.Created($"/hotels/{hotel.Id}", hotel);
});

app.MapPut("/hotels", async (HotelDb db, [FromBody]Hotel hotel) =>
{
    var hotelFromDb = await db.Hotels.FindAsync(new int[] { hotel.Id });
    if (hotelFromDb == null) return Results.NotFound();

    hotelFromDb.Name = hotel.Name;
    hotelFromDb.CostPerNight=hotel.CostPerNight;
    await db.SaveChangesAsync();
    return Results.NoContent();
}
);
app.MapDelete("/hotels/{id}", async (HotelDb db, int id) => 
{
    var hotelFromDb = await db.Hotels.FirstOrDefaultAsync(h => h.Id == id);
    if (hotelFromDb == null) return Results.NotFound();
    if(hotelFromDb.Id != id) return Results.Conflict(new Exception("Ids are different"));
    db.Hotels.Remove(hotelFromDb);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

//app.UseHttpsRedirection();

app.Run();

class HotelDb: DbContext
{
    public HotelDb(DbContextOptions options):base(options){}
    public DbSet<Hotel> Hotels{get;set;}
}

// Model hotel class
class Hotel
{
    public int Id {get;set;}
    public string Name {get;set;}
    public decimal CostPerNight {get;set;}
}