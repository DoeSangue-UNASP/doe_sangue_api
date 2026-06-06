using DoeSangue.Infrastructure.Data;
using DoeSangue.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<DoeSangueContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<UsuarioIdentity, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<DoeSangueContext>()
                .AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
