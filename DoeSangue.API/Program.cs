using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using DoeSangue.Application.UseCases.Agendamentos;
using DoeSangue.Application.UseCases.Autenticacao;
using DoeSangue.Application.UseCases.BolsasSangue;
using DoeSangue.Application.UseCases.Depoimentos;
using DoeSangue.Application.UseCases.Doadores;
using DoeSangue.Application.UseCases.Hemocentros;
using DoeSangue.Domain.Enums;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using DoeSangue.Infrastructure.Repository;
using DoeSangue.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Database");
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT secret key is not configured.");
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "DoeSangue.API";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "DoeSangue.Client";

builder.Services.AddDbContext<DoeSangueContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<UsuarioIdentity, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<DoeSangueContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("DepoimentosPublicos", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "ip-desconhecido",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 1,
                Window = TimeSpan.FromSeconds(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IDoadorRepository, DoadorRepository>();
builder.Services.AddScoped<IHemocentroRepository, HemocentroRepository>();
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
builder.Services.AddScoped<IDepoimentoRepository, DepoimentoRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IBolsaSangueRepository, BolsaSangueRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthService>();

builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<CriarDoadorUseCase>();
builder.Services.AddScoped<CadastrarLeadUseCase>();
builder.Services.AddScoped<BuscarDoadorPorId>();
builder.Services.AddScoped<BuscarDoadorPorUsuarioIdUseCase>();
builder.Services.AddScoped<BuscarTodosDoadoresUseCase>();
builder.Services.AddScoped<ExcluirDoadorPorUsuarioId>();
builder.Services.AddScoped<AtualizarDoadorPorId>();
builder.Services.AddScoped<CriarHemocentroUseCase>();
builder.Services.AddScoped<BuscarHemocentroPorUsuarioIdUseCase>();
builder.Services.AddScoped<BuscarTodosHemocentrosUseCase>();
builder.Services.AddScoped<CriarDepoimentoUseCase>();
builder.Services.AddScoped<BuscarDepoimentosUseCase>();
builder.Services.AddScoped<CriarAgendamentoUseCase>();
builder.Services.AddScoped<BuscarAgendamentosDoadorUseCase>();
builder.Services.AddScoped<BuscarAgendaHemocentroUseCase>();
builder.Services.AddScoped<CancelarAgendamentoUseCase>();
builder.Services.AddScoped<ReagendarAgendamentoUseCase>();
builder.Services.AddScoped<ConcluirAgendamentoUseCase>();
builder.Services.AddScoped<CriarBolsaSangueUseCase>();
builder.Services.AddScoped<BuscarBolsasSangueUseCase>();
builder.Services.AddScoped<BuscarBolsaSangueUseCase>();
builder.Services.AddScoped<AtualizarStatusBolsaUseCase>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DoeSangue.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autorização JWT usando o esquema Bearer. \r\n\r\nInsira 'Bearer' [space] e seu token logo em seguida.\r\n\r\n Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<DoeSangueContext>();
    await dbContext.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var roleDoador = await roleManager.FindByNameAsync(nameof(UsuarioRole.DOADOR));
    var roleHemocentro = await roleManager.FindByNameAsync(nameof(UsuarioRole.HEMOCENTRO));

    if (roleDoador is null)
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>(nameof(UsuarioRole.DOADOR)));
    }

    if (roleHemocentro is null)
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>(nameof(UsuarioRole.HEMOCENTRO)));
    }
}

app.Run();
