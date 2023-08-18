var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HotelDb>(options=>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<IHotelRepository,HotelRepository>();

var app = builder.Build();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

ApplicationModifier.Modify(ref app);

app.Run();

// connect swagger:  http://localhost:{port}/swagger/index.html

