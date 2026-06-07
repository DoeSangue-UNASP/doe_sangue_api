using DoeSangue.Application.UseCases.Doadores;
using DoeSangue.Domain.Enums;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using DoeSangue.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<DoeSangueContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<UsuarioIdentity, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<DoeSangueContext>()
                .AddDefaultTokenProviders();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IDoadorRepository, DoadorRepository>();
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();

builder.Services.AddScoped<CriarDoadorUseCase>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DoeSangue.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autorização JWT usando o esquema Bearer. \r\n\r\nInsira 'Bearer' [space] e seu token logo em seguida.\r\n\r\n Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
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
