using LanchoneteWeb.Areas.Admin.Servicos;
using LanchoneteWeb.Context;
using LanchoneteWeb.Models;
using LanchoneteWeb.Repositories;
using LanchoneteWeb.Repositories.Interfaces;
using LanchoneteWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

var builder = WebApplication.CreateBuilder(args);
// Área que seria do  Configurebuilder.Services se houvesse a classe startup

//regitrar o contexto como serviço de acesso ao banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
                 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// incluindo o serviço do Identity para gerenciar usuarios e perfis 
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

builder.Services.AddTransient<ILancheRepository, LancheRepository>();
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();

builder.Services.AddScoped<RelatorioVendasService>();
builder.Services.AddScoped<GraficoVendasService>();


builder.Services.Configure<ConfigurationImagens>(builder.Configuration.GetSection("ConfigurationPastaImagens"));

//registrando serviço
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
//registrando o serviço
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", politica =>
    {
        politica.RequireRole("Admin");
    });
});

//recuperar um instancia de httpcontextAccessor, request e response
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//Registrando com addScoped gerado a cada request 
builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache(); // Ativa o uso de cache em memória  
builder.Services.AddSession(); // habilitando o session 

builder.Services.AddPaging(options =>
{
    options.ViewName = "Bootstrap4";
    options.PageParameterName = "pageindex";
});


var app = builder.Build();
// Área que seria do  Configure se houvesse a classe startup
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. you may want to change 
    app.UseHsts();
}

CriarPerfisUsuarios(app);

////Cria os perfis 
//seedUserRoleInitial.SeedRoles();
////cria o usuario e atribui ao perfil 
//seedUserRoleInitial.SeedUsers();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Ativando o session 

app.UseAuthentication(); //Ativando a autenticação feita pelo Identity

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
          name: "areas",
          pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
        );
    });

    endpoints.MapControllerRoute(
        name: "categoriaFiltro",
        pattern: "Lanche/{action}/{categoria?}",
        defaults: new { Controller = "Lanche", action = "List" }
        );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});


app.Run();

 void CriarPerfisUsuarios(WebApplication app) 
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
        service.SeedUsers();
        service.SeedRoles();
    }

}