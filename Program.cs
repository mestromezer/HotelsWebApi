var builder = WebApplication.CreateBuilder(args);

// Adding services to pipeline.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HotelDb>(options=>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<IHotelRepository,HotelRepository>();

// Configuring API.
var app = builder.Build();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});
app.UseSwagger();
//app.UseSwaggerUI();
/*using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<HotelDb>();
db.Database.EnsureCreated();*/
app.UseHttpsRedirection();
EndpointsCreator.Modify(ref app);

// Run the API.
app.Run();

