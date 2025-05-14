using Vmi.Portal.Data.Repositories;
using Vmi.Portal.Data.Repositories.Interfaces;
using Vmi.Portal.DbContext;
using Vmi.Portal.Repositories;
using Vmi.Portal.Repositories.Interfaces;
using Vmi.Portal.Repository;
using Vmi.Portal.Services;
using Vmi.Portal.Services.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment;

builder.Services.AddControllers();

if (environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200", "https://localhost")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Services
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Repositories
builder.Services.AddScoped<IAcessoRepository, AcessoRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IModuloRepository, ModuloRepository>();
builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
builder.Services.AddScoped<IPerfilRotinaRepository, PerfilRotinaRepository>();
builder.Services.AddScoped<IRotinaRepository, RotinaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

string vmiDb;
if (environment.IsDevelopment())
{
    vmiDb = builder.Configuration.GetConnectionString(environment.EnvironmentName);
}
else if (environment.IsStaging())
{
    vmiDb = builder.Configuration.GetConnectionString(environment.EnvironmentName);
}
else
{
    vmiDb = builder.Configuration.GetConnectionString(environment.EnvironmentName);
}

builder.Services.AddTransient(_ => new VmiDbContext(vmiDb));

WebApplication app = builder.Build();


var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation($"Aplicação iniciada no ambiente: {environment.EnvironmentName}");


if (environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();
app.MapControllers();

app.Run();