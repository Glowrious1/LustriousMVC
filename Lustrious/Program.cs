using Lustrious.Models;
using Lustrious.Repositorio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Adicionando as Interfaces de Repositorio
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IFuncionarioRepositorio, FuncionarioRepositorio>();
builder.Services.AddScoped<Cliente>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
