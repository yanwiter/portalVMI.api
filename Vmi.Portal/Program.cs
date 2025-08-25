using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Vmi.Portal.Data.Repositories;
using Vmi.Portal.Data.Repositories.Interfaces;
using Vmi.Portal.DbContext;
using Vmi.Portal.Repositories;
using Vmi.Portal.Repositories.Interfaces;
using Vmi.Portal.Repository;
using Vmi.Portal.Services;
using Vmi.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment;

// Configuração	 JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
    };
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // Configuração necessária para JsonPatch
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithExposedHeaders("Authorization");
        });
});

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthenticationCodeService, AuthenticationCodeService>();
builder.Services.AddSingleton<IActiveSessionService, ActiveSessionService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IClienteFornecedorService, ClienteFornecedorService>();
builder.Services.AddScoped<IColumnPreferencesService, ColumnPreferencesService>();

// Repositories
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IAuthenticationCodeRepository, AuthenticationCodeRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IAcessoRepository, AcessoRepository>();
builder.Services.AddScoped<IModuloRepository, ModuloRepository>();
builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
builder.Services.AddScoped<IPerfilRotinaRepository, PerfilRotinaRepository>();
builder.Services.AddScoped<IRotinaRepository, RotinaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IClienteFornecedorRepository, ClienteFornecedorRepository>();
builder.Services.AddScoped<IContatoRepository, ContatoRepository>();
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
builder.Services.AddScoped<IDadoFinanceiroRepository, DadoFinanceiroRepository>();
builder.Services.AddScoped<IAnexoRepository, AnexoRepository>();
builder.Services.AddScoped<IColumnPreferencesRepository, ColumnPreferencesRepository>();

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

app.UseDeveloperExceptionPage();
app.MapFallbackToFile("/src/index.html");
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();