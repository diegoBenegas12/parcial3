using api.primerparcial;
using api.primerparcial.Data;
using api.primerparcial.Interfaces;
using api.primerparcial.Models;
using api.primerparcial.Repositories;
using api.primerparcial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddTransient<IUserRepository, UsersInMemoryRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");

builder.Services.AddDbContext<parcial1>(options =>
    options.UseNpgsql(connectionString));

var info = new OpenApiInfo
{
    Title = "Curso JWT"
};
var security = new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JWT para curso ua"
};
var requirement = new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id   = "Bearer"
            }
        },
        new List<string>()
    }
};

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", security);
    options.AddSecurityRequirement(requirement);
});

builder.Services.Configure<TokenSettings>(
    builder.Configuration.GetSection(nameof(TokenSettings)));

builder.Services.AddAuthentication()
    .AddJwtBearer("CURSO-UA", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["TokenSettings:Issuer"],
            ValidAudience = builder.Configuration["TokenSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder
                .Configuration["TokenSettings:Secret"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("CURSO-UA")
        .Build();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/clientes/", async (Clientes clientes, parcial1 db) =>
{
    db.Clientes.Add(clientes);
    await db.SaveChangesAsync();

    return Results.Created($"/clientes/{clientes.Id}", clientes);
}).RequireAuthorization();

app.MapGet("/clientes/{id:int}", (int id, parcial1 db) =>
{

    var query = db.Clientes
                           .Where(s => s.Id == id)
                           .Include(s => s.Ciudades)
                           .FirstOrDefault();

    return Task.FromResult(query);
}).RequireAuthorization();

app.MapGet("/ListClientes/", (parcial1 db) =>
{
    var query = db.Clientes.Include(s => s.Ciudades).ToList();

    return Task.FromResult(query);
}).RequireAuthorization();

app.MapGet("/ListCiudades/", (parcial1 db) =>
{
    var query = db.Ciudades.ToList();

    return Task.FromResult(query);
}).RequireAuthorization();

app.MapPut("/clientes/{id:int}", async (int id, Clientes inputTodo, parcial1 db) =>
{
    var todo = await db.Clientes.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.IdCiudad = inputTodo.IdCiudad;
    todo.Nombres = inputTodo.Nombres;
    todo.Apellidos = inputTodo.Apellidos;
    todo.Documento = inputTodo.Documento;
    todo.Telefono = inputTodo.Telefono;
    todo.Email = inputTodo.Email;
    todo.FechaNacimiento = inputTodo.FechaNacimiento;
    todo.Ciudad = inputTodo.Ciudad;
    todo.Nacionalidad = inputTodo.Nacionalidad;

    await db.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization();

app.MapDelete("/clientes/{id}", async (int id, parcial1 db) =>
{
    if (await db.Clientes.FindAsync(id) is Clientes todo)
    {
        db.Clientes.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }

    return Results.NotFound();
}).RequireAuthorization();

app.MapPost("/ciudades/", async (Ciudades ciudades, parcial1 db) =>
{
    db.Ciudades.Add(ciudades);
    await db.SaveChangesAsync();

    return Results.Created($"/ciudades/{ciudades.Id}", ciudades);
}).RequireAuthorization();

app.MapGet("/ciudades/{id:int}", (int id, parcial1 db) =>
{
    /*return await db.Clientes.FindAsync(id)
              is Clientes cliente ? Results.Ok(cliente) : Results.NotFound();*/

    var ciudad = db.Ciudades
                           .Where(s => s.Id == id)
                           .Include(s => s.Clientes)
                           .FirstOrDefault();

    return Task.FromResult(ciudad);
}).RequireAuthorization();

app.MapPut("/ciudades/{id:int}", async (int id, Ciudades inputTodo, parcial1 db) =>
{
    var todo = await db.Ciudades.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Ciudad = inputTodo.Ciudad;
    todo.Estado = inputTodo.Estado;

    await db.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization();

app.MapDelete("/ciudades/{id}", async (int id, parcial1 db) =>
{
    if (await db.Ciudades.FindAsync(id) is Ciudades todo)
    {
        db.Ciudades.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }

    return Results.NotFound();
}).RequireAuthorization();

app.MapPost("/token", [AllowAnonymous] async (IUserService userService,
    IAuthenticationService authenticationService,
    AuthenticationRequest request) =>
{
    var isValidAuthentication = await authenticationService
        .Authenticate(request.Username, request.Password);

    if (isValidAuthentication)
    {
        var user = await userService.GetByCredentials(request.Username, request.Password);

        var token = await authenticationService.GenerateJwt(user);

        return Results.Ok(new { AccessToken = token });
    }

    return Results.Forbid();
});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}