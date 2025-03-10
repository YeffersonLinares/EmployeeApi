using EmployeeApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<EmployeeService>();
builder.Services.AddMemoryCache();
// builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Permitir Angular
              .AllowAnyMethod() // Permitir cualquier método (GET, POST, etc.)
              .AllowAnyHeader() // Permitir cualquier encabezado
              .AllowCredentials(); // Permitir credenciales si es necesario
    });
});

builder.Services.AddControllers();
var app = builder.Build();
app.UseCors("AllowAngularApp");

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();