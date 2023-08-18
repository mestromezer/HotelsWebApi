
//httprepl

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HotelDb>(options=>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<IHotelRepository,HotelRepository>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HotelDb>();
    db.Database.EnsureCreated();
}


app.MapGet("/hotels", async (HotelRepository repository) => Results.Ok(await repository.GetHotelAsync()));
app.MapGet("/hotels/{id}", async (HotelRepository repository,int id) => 
await repository.GetHotelAsync(id) is Hotel hotel?
Results.Ok(hotel):
Results.NotFound()
);

app.MapPost("/hotels", async (HotelRepository repository, [FromBody]Hotel hotel, HttpResponse response) =>
{ 
    await repository.InsertHotelAsync(hotel);
    await repository.SaveAsync();
    return Results.Created($"/hotels/{hotel.Id}", hotel);
});

app.MapPut("/hotels", async (HotelRepository repository, [FromBody]Hotel hotel) =>
{
    await repository.UpdateHotelAsync(hotel);
    await repository.SaveAsync();
    return Results.NoContent();
}
);
app.MapDelete("/hotels/{id}", async (HotelRepository repository, int id) => 
{
    await repository.DeleteHotelAsync(id);
    await repository.SaveAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();

app.Run();


