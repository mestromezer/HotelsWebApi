public static class EndpointsCreator
{
    public static void Modify(ref WebApplication app)
    {
        // Get list of hotels.
        app.MapGet("/hotels", async (IHotelRepository repository) => 
        Results.Ok(await repository.GetHotelAsync()))
        .Produces<List<Hotel>>(StatusCodes.Status200OK)
        .WithName("Get all hotels")
        .WithTags("Getters");

        // Get exact hotel info.
        app.MapGet("/hotels/{id}", async (int id, IHotelRepository repository) => 
        await repository.GetHotelAsync(id) is Hotel hotel?
        Results.Ok(hotel):
        Results.NotFound()
        ).Produces<Hotel>(StatusCodes.Status200OK)
        .WithName("Gets hotel by id")
        .WithTags("Getters");

        // Get exact hotel info by query.
        app.MapGet("/hotels/search/name/{query}", async (string query, IHotelRepository repository) =>
            await repository.GetHotelsAsync(query) is IEnumerable<Hotel> hotels
                ? Results.Ok(hotels)
                : Results.NotFound(Array.Empty<Hotel>()))
            .Produces<List<Hotel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("Search hotels by query")
            .WithTags("Getters").ExcludeFromDescription(); // exclude from swagger

        // Create new hotel and provide it to the database.
        app.MapPost("/hotels", async ([FromBody]Hotel hotel, IHotelRepository repository) =>
        { 
            await repository.InsertHotelAsync(hotel);
            await repository.SaveAsync();
            return Results.Created($"/hotels/{hotel.Id}", hotel);
        })
        .Accepts<Hotel>("application/json")
        .Produces<Hotel>(StatusCodes.Status201Created)
        .WithName("Create new hotel")
        .WithTags("Creators");

        // Update info about a hotel.
        app.MapPut("/hotels", async ([FromBody]Hotel hotel, IHotelRepository repository) =>
        {
            await repository.UpdateHotelAsync(hotel, hotel.Id);
            await repository.SaveAsync();
            return Results.NoContent();
        }
        )
        .Accepts<Hotel>("application/json")
        .WithName("Update hotel's info")
        .WithTags("Updaters");

        // Delete a hotel from the database.
        app.MapDelete("/hotels/{id}", async (int id, IHotelRepository repository) => 
        {
            await repository.DeleteHotelAsync(id);
            await repository.SaveAsync();
            return Results.NoContent();
        })
        .WithName("Delete hotel")
        .WithTags("Deleters");
    }
}