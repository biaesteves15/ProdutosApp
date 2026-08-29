using Microsoft.EntityFrameworkCore;
using ProdutosApp.Infra.Data.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Infra.Data.Contexts
{
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //mapear a string de conexão do banco de dados 
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BDProdutos;Integrated Security=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoriaMap());
            modelBuilder.ApplyConfiguration(new ProdutoMap());
        }
    }
}
