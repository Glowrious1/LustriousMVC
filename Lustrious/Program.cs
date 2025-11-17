using Lustrious.Models;
using Lustrious.Data;
using Lustrious.Repositorio;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Habilitar sessão
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
 options.IdleTimeout = TimeSpan.FromHours(1);
 options.Cookie.HttpOnly = true;
 options.Cookie.IsEssential = true;
});

// Registrar serviços e repositórios no DI
builder.Services.AddSingleton<DataBase>();
builder.Services.AddScoped<IFavoritosRepositorio, FavoritosRepositorio>();
builder.Services.AddScoped<IProdutoRepostorio, ProdutoRepositorio>();
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<ICarrinhoRepositorio, CarrinhoRepositorio>();
builder.Services.AddScoped<IFuncionarioRepositorio, FuncionarioRepositorio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
 app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
 name: "default",
 pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
