using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

public static readonly List<GameDto> games = 
[
    new
    (
        1,
        "Street Fighter II",
        "Fighting",
        19.88M,
        new DateOnly(1998, 7, 15)                     
    ),
    new
    (
        2,
        "Final Fantsy XIV",
        "roleplaying",
        69.69M,
        new DateOnly(2010, 10, 10)
    ),
    new
    (
        3,
        "FIFA",
        "Sports",
        100.12M,
        new DateOnly(2021, 12, 8)
    ),
    new
    (
        4,
        "PUBG",
        "battle Royal",
        200.68M,
        new DateOnly(2020, 8, 15)
    ),

];

public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
{
    //we are going to implement a group builder 
    var group = app.MapGroup("games")
                   .WithParameterValidation(); //insted of using it seperatley for each request we can directly put it up here

    //GET/games
    group.MapGet("/" , () => games); 


//GET/games/1    
group.MapGet("/{id}", (int id) => 
{
    GameDto? game = games.Find(game => game.Id == id);

    return game is null ? Results.NotFound() : Results.Ok(game);
})
.WithName(GetGameEndpointName);





//POST /games
group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbcontext) =>
{
    //This id condition is for valiadtion purpose but this is not the best approach
    //instead of this for loop we will be using data anotations
    // if(string.IsNullOrEmpty(newGame.Name))
    // {
    //     return Results.BadRequest("Name is Rquired");
    // }
    
    Game game = new()
    {
        Name = newGame.Name,
        Genre = dbcontext.Genres.Find(newGame.GenreId),

        // games.Count + 1,
        // newGame.Name,
        // newGame.Genre,
        // newGame.Price,
        // newGame.ReleaseDate
    };

    games.Add(game);

    return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id}, game);

});
//This one is directly used from the extension "MinimalApis.Ext" which you can check out in gamestore.api
//.WithParameterValidation();





//PUT /games
group.MapPut("/{id}",(int id, UpdateGameDto updatedGame) =>
{
    var index = games.FindIndex(game => game.Id == id);

    if(index == -1)
    {
        return Results.NotFound();
    }

    games[index] = new GameDto
    (
        id,
        updatedGame.Name,
        updatedGame.Genre,
        updatedGame.Price,
        updatedGame.ReleaseDate
    );

    return Results.NoContent();
});




//DELETE /games
group.MapDelete("/{id}",(int id)=>
{
    games.RemoveAll(game => game.Id == id);

    return Results.NoContent();
});

return group;

}

}
