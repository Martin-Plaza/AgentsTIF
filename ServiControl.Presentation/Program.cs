using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiControl.Application.Interfaces;
using ServiControl.Application.Services;
using ServiControl.Infrastructure;

// Modulo: API
// Capa: Presentation
// Responsabilidad: Configura servicios HTTP, autenticacion, Swagger y dependencias.
// Nota: La configuracion se lee desde appsettings o variables de entorno, compatible con Azure App Service.
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//servicio para los controladores
builder.Services.AddControllers();

//servicio para que swagger lea los endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //definimos como va a usar el token (es para cuando usemos [autorize] en swagger boton arriba a la derecha)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token con el formato: Bearer {token}"
    });
    //aca usamos el esquema anterior OpenApiSecurityRequrement, el que acabamos de definir arriba.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

//las politicas de CORS permiten que un frontend consuma nuestra API. en nuestras politicas validamos quien queremos que consuma nuestra API (desde el navegador)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        //en entorno de desarrollo puede consumirla desde cualquier lugar (swagger por ejemplo)
        if (builder.Environment.IsDevelopment())
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();

            return;
        }

        //aca aplicamos la politica CORS fuera del entorno de desarrollo
        var allowedOrigins = builder.Configuration
        //getsection toma toda la seccion cors
        //getChildren los hijos de esa seccion
        //
            .GetSection("Cors:AllowedOrigins")
            .GetChildren()
            .Select(origin => origin.Value)
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin!)
            .ToArray();

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Jwt:Key debe configurarse como variable de entorno en Azure: Jwt__Key.
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("La clave JWT no esta configurada.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITrabajoService, TrabajoService>();
builder.Services.AddScoped<ICostoService, CostoService>();
builder.Services.AddScoped<IMetricaService, MetricaService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Swagger queda activo solo en Development para documentar y probar la API durante desarrollo.
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
