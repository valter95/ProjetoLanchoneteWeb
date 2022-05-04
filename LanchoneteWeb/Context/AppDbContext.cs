using LanchoneteWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace LanchoneteWeb.Context
{
    public class AppDbContext : DbContext
    {
        // DbContextOptions => Define as opções a serem usadas pelo DbContext, ela vai carregar as informações de configurações necessárias para configurar o DbContext 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //DbSet Define quais classes quero mapeas para as tabelas 
        //Classe Categorias, tabela no banco Categorias
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Lanche> Lanches { get; set; }  
    }
}
