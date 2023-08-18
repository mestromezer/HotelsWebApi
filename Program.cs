
//httprepl

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HotelDb>(options=>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<IHotelRepository,HotelRepository>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HotelDb>();
    db.Database.EnsureCreated();
}


app.MapGet("/hotels", async ([FromServices]HotelRepository repository) => Results.Ok(await repository.GetHotelAsync()))
.Produces<List<Hotel>>(StatusCodes.Status200OK)
.WithName("Get all hotels")
.WithTags("Getters");
app.MapGet("/hotels/{id}", async ([FromServices]HotelRepository repository,int id) => 
await repository.GetHotelAsync(id) is Hotel hotel?
Results.Ok(hotel):
Results.NotFound()
).Produces<Hotel>(StatusCodes.Status200OK)
.WithName("Gets hotel by id")
.WithTags("Getters");
app.MapGet("/hotels/search/name/{query}", async (string name,[FromServices] HotelRepository repository) =>
    await repository.GetHotelsAsync(name) is IEnumerable<Hotel> hotels
        ? Results.Ok(hotels)
        : Results.NotFound(Array.Empty<Hotel>()))
    .Produces<List<Hotel>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("Search hotels by query")
    .WithTags("Getters").ExcludeFromDescription(); // exclude from swagger

app.MapPost("/hotels", async ([FromServices]HotelRepository repository, [FromBody]Hotel hotel, HttpResponse response) =>
{ 
    await repository.InsertHotelAsync(hotel);
    await repository.SaveAsync();
    return Results.Created($"/hotels/{hotel.Id}", hotel);
}).Accepts<Hotel>("application/json").Produces<Hotel>(StatusCodes.Status201Created).WithName("Create new hotel")
.WithTags("Creators");

app.MapPut("/hotels", async ([FromServices]HotelRepository repository, Hotel hotel) =>
{
    await repository.UpdateHotelAsync(hotel);
    await repository.SaveAsync();
    return Results.NoContent();
}
).Accepts<Hotel>("application/json").WithName("Update hotel's info")
.WithTags("Updaters");

app.MapDelete("/hotels/{id}", async ([FromServices]HotelRepository repository, int id) => 
{
    await repository.DeleteHotelAsync(id);
    await repository.SaveAsync();
    return Results.NoContent();
}).WithName("Delete hotel").WithTags("Deleters");

app.UseHttpsRedirection();

app.Run();


