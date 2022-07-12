using LanchoneteWeb.Areas.Admin.Servicos;
using LanchoneteWeb.Context;
using LanchoneteWeb.Models;
using LanchoneteWeb.Repositories;
using LanchoneteWeb.Repositories.Interfaces;
using LanchoneteWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace LanchoneteWeb;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        //regitrar o contexto como serviço de acesso ao banco de dados
        services.AddDbContext<AppDbContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // incluindo o serviço do Identity para gerenciar usuarios e perfis 
        services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

        services.AddTransient<ILancheRepository, LancheRepository>();
        services.AddTransient<ICategoriaRepository, CategoriaRepository>();
        services.AddTransient<IPedidoRepository, PedidoRepository>();

        services.AddScoped<RelatorioVendasService>();

        //registrando serviço
        services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
        //registrando o serviço
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", politica =>
                                                    {
                                                        politica.RequireRole("Admin");
                                                    });
        });

        //recuperar um instancia de httpcontextAccessor, request e response
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        //Registrando com addScoped gerado a cada request 
        services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

        services.AddControllersWithViews();
        services.AddMemoryCache(); // Ativa o uso de cache em memória  
        services.AddSession(); // habilitando o session 

        services.AddPaging(options =>
        {
            options.ViewName = "Bootstrap4";
            options.PageParameterName = "pageindex";
        });


    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISeedUserRoleInitial seedUserRoleInitial)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. you may want to change 
            app.UseHsts();
        }

        //Cria os perfis 
        seedUserRoleInitial.SeedRoles();
        //cria o usuario e atribui ao perfil 
        seedUserRoleInitial.SeedUsers();

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
    }
}
