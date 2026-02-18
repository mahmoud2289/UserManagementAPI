using Microsoft.AspNetCore.OpenApi;
using Swashbuckle.AspNetCore;

using UserManagementAPI.Middleware;
using UserManagementAPI.Models;
using UserManagementAPI.Services;
using UserManagementAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddOpenApi();

// 🔹 Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure middleware pipeline in the correct order
// 1. Error-handling middleware (must be first to catch all exceptions)
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. Authentication middleware (validates tokens before processing requests)
app.UseMiddleware<AuthenticationMiddleware>();

// 3. Logging middleware (logs all requests/responses after authentication)
app.UseMiddleware<LoggingMiddleware>();

// Health check endpoint (accessible without authentication)
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("HealthCheck")
    .Produces(StatusCodes.Status200OK);

// Minimal APIs for User Management
var usersGroup = app.MapGroup("/api/users")
    .WithName("Users");

// GET: Retrieve all users or a specific user by ID
usersGroup.MapGet("/", GetAllUsers)
    .WithName("GetAllUsers")
    .Produces<IEnumerable<User>>(StatusCodes.Status200OK);

usersGroup.MapGet("/{id}", GetUserById)
    .WithName("GetUserById")
    .Produces<User>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

// POST: Add a new user
usersGroup.MapPost("/", CreateUser)
    .WithName("CreateUser")
    .Produces<User>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

// PUT: Update an existing user
usersGroup.MapPut("/{id}", UpdateUser)
    .WithName("UpdateUser")
    .Produces<User>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest);

// DELETE: Remove a user by ID
usersGroup.MapDelete("/{id}", DeleteUser)
    .WithName("DeleteUser")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

// Endpoint handler methods
static async Task<IResult> GetAllUsers(IUserService userService)
{
    try
    {
        var users = await userService.GetAllUsersAsync();
        return Results.Ok(users);
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError, 
            title: "An error occurred while retrieving users");
    }
}

static async Task<IResult> GetUserById(int id, IUserService userService)
{
    try
    {
        if (id <= 0)
            return Results.BadRequest(new { message = "Invalid user ID" });

        var user = await userService.GetUserByIdAsync(id);
        if (user == null)
            return Results.NotFound(new { message = $"User with ID {id} not found" });
        
        return Results.Ok(user);
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError, 
            title: "An error occurred while retrieving the user");
    }
}

static async Task<IResult> CreateUser(User user, IUserService userService)
{
    try
    {
        // Validate user input
        var (isValid, errorMessage) = UserValidator.ValidateUser(user);
        if (!isValid)
            return Results.BadRequest(new { message = errorMessage });

        var createdUser = await userService.CreateUserAsync(user);
        return Results.Created($"/api/users/{createdUser.Id}", createdUser);
    }
    catch (ArgumentNullException ex)
    {
        return Results.BadRequest(new { message = $"Invalid user data: {ex.Message}" });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError, 
            title: "An error occurred while creating the user");
    }
}

static async Task<IResult> UpdateUser(int id, User user, IUserService userService)
{
    try
    {
        if (id <= 0)
            return Results.BadRequest(new { message = "Invalid user ID" });

        // Validate user input
        var (isValid, errorMessage) = UserValidator.ValidateUser(user);
        if (!isValid)
            return Results.BadRequest(new { message = errorMessage });

        var updatedUser = await userService.UpdateUserAsync(id, user);
        if (updatedUser == null)
            return Results.NotFound(new { message = $"User with ID {id} not found" });

        return Results.Ok(updatedUser);
    }
    catch (ArgumentNullException ex)
    {
        return Results.BadRequest(new { message = $"Invalid user data: {ex.Message}" });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError, 
            title: "An error occurred while updating the user");
    }
}

static async Task<IResult> DeleteUser(int id, IUserService userService)
{
    try
    {
        if (id <= 0)
            return Results.BadRequest(new { message = "Invalid user ID" });

        var result = await userService.DeleteUserAsync(id);
        if (!result)
            return Results.NotFound(new { message = $"User with ID {id} not found" });

        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError, 
            title: "An error occurred while deleting the user");
    }
}

app.Run();
