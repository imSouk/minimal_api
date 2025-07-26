using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.api.Domain.DTO;
using minimal_api.api.Domain.Entities;
using minimal_api.api.Domain.Interfaces;
using minimal_api.api.Domain.ModelViews;
using minimal_api.api.Infrastruct.DB;
using minimal_api.api.Infrastruct.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace minimal_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
        }

        private string key = "";
        public IConfiguration Configuration { get; set; } = default!;
        public void ConfigureServices (IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddAuthorization();
            services.AddScoped<IAdministratorService, AdministratorService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT aqui"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            });
            services.AddDbContext<StorageBroker>(options =>
            {
                options.UseMySql
                (
                    Configuration.GetConnectionString("mysql"),
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("mysql"))
                );
            });
            services.AddCors(options =>
                    {
                        options.AddDefaultPolicy(
                            builder =>
                            {
                                builder.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader();
                            });
                    });
    }
            
        
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

           
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(enpoints => {

                #region home
                enpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("home");
                #endregion home
                #region Adm
                string GenerateToken(Administrator administrator)
                {
                    if (string.IsNullOrEmpty(key)) return string.Empty;

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>()
                {
                    new Claim("Email", administrator.Email),
                    new Claim("Perfil", administrator.Perfil),
                    new Claim(ClaimTypes.Role, administrator.Perfil),
                };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }
                enpoints.MapPost("/administrators/login", ([FromBody] LoginDto loginDto, IAdministratorService administrator) =>
                {
                    var adm = administrator.Login(loginDto);
                    if (adm != null)
                    {
                        string token = GenerateToken(adm);
                        return Results.Ok(new AdmLogin
                        {
                            Email = adm.Email,
                            Perfil = adm.Perfil,
                            Token = token
                        });
                    }
                    else
                        return Results.Unauthorized();

                }).AllowAnonymous().WithTags("adm");
                enpoints.MapPost("/", ([FromBody] AdmDTO administratorDto, IAdministratorService administratorService) =>
                {
                    var validationAdm = new ValidationErrors()
                    {
                        Messages = new List<string>()
                    };
                    if (string.IsNullOrEmpty(administratorDto.Email))
                    {
                        validationAdm.Messages.Add("Email cant be null");
                    }
                    if (string.IsNullOrEmpty(administratorDto.Senha))
                    {
                        validationAdm.Messages.Add("Password cant be null");
                    }
                    if (administratorDto?.Perfil == null)
                    {
                        validationAdm.Messages.Add("Perfil cant be null");
                    }
                    if (validationAdm.Messages.Count() > 0)
                    {
                        return Results.BadRequest(validationAdm.Messages);
                    }
                    var adm = new Administrator()
                    {
                        Email = administratorDto.Email,
                        Senha = administratorDto.Senha,
                        Perfil = administratorDto.Perfil.ToString(),
                    };
                    administratorService.Add(adm);

                    return Results.Created($"/veiculo/{adm.Id}", new AdmModelView
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                    });
                }).RequireAuthorization().WithTags("adm");
                enpoints.MapGet("/adm", ([FromQuery] int pagina, IAdministratorService administratorService) => {
                    var adms = new List<AdmModelView>();
                    var adm = administratorService.GetAdministrator(pagina);
                    foreach (var admin in adm)
                    {
                        adms.Add(new AdmModelView
                        {
                            Id = admin.Id,
                            Email = admin.Email,
                            Perfil = admin.Perfil,
                        });

                    }
                    return Results.Ok(adm);
                }).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("adm");
                enpoints.MapGet("/adm/{id}", ([FromRoute] int id, IAdministratorService administratorService) => {

                    Administrator? adm = administratorService.FindById(id);
                    if (adm != null)
                        return Results.Ok(new AdmModelView
                        {
                            Id = adm.Id,
                            Email = adm.Email,
                            Perfil = adm.Perfil,
                        });
                    return Results.NotFound("Adm not found");

                }).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("adm");
                #endregion Adm
                #region vehicles
                ValidationErrors getValidation(VehicleDto vehicleDto)
                {
                    var validation = new ValidationErrors()
                    {
                        Messages = new List<string>()
                    };
                    if (string.IsNullOrEmpty(vehicleDto.Nome))
                    {
                        validation.Messages.Add("The name cant be null");
                    }
                    if (string.IsNullOrEmpty(vehicleDto.Marca))
                    {
                        validation.Messages.Add("The model cant be null");
                    }
                    if (vehicleDto.Ano < 1950)
                    {
                        validation.Messages.Add("The year cant be less than 1951.");
                    }
                    return validation;
                }
                enpoints.MapPost("/vehicles", ([FromBody] VehicleDto vehicleDto, IVehicleService vehicleService) => {

                    var validation = getValidation(vehicleDto);
                    if (validation.Messages.Count > 0)
                    {
                        return Results.BadRequest(validation);
                    }
                    Vehicle vehicle = new Vehicle
                    {
                        Nome = vehicleDto.Nome,
                        Marca = vehicleDto.Marca,
                        Ano = vehicleDto.Ano
                    };
                    vehicleService.Save(vehicle);

                    return Results.Created($"/veiculo/{vehicle.Id}", vehicle);
                }).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("vehicles");
                enpoints.MapGet("/vehicles", ([FromQuery] int pagina, IVehicleService vehicleService) => {
                    var vehicles = vehicleService.getAll(pagina);

                    return Results.Ok(vehicles);
                }).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("vehicles");
                enpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {

                    Vehicle vehicle = vehicleService.FindById(id);
                    if (vehicle != null)
                        return Results.Ok(vehicle);
                    return Results.NotFound("Vehicle not found");

                }).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("vehicles");
                enpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDto vehicleDto, IVehicleService vehicleService) => {
                    Vehicle vehicle = vehicleService.FindById(id);
                    if (vehicle == null)
                        return Results.NotFound("Vehicle not found");
                    vehicle.Nome = vehicleDto.Nome;
                    vehicle.Marca = vehicleDto.Marca;
                    vehicle.Ano = vehicleDto.Ano;
                    vehicleService.Update(vehicle);
                    return Results.Ok($"new vhicle informations: {vehicle.Nome} {vehicle.Marca} {vehicle.Ano}");

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("vehicles");
                enpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
                    Vehicle vehicle = vehicleService.FindById(id);
                    if (vehicle == null)
                        return Results.NotFound("Vehicle not found");
                    vehicleService.Delete(vehicle);
                    return Results.Ok($"Delete vhicle informations: {vehicle.Nome} {vehicle.Marca} {vehicle.Ano}");

                }).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("vehicles");
                #endregion vehicles
               

            });
        }
    }
}
