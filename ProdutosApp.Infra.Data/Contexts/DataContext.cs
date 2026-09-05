using Microsoft.EntityFrameworkCore;
using ProdutosApp.Infra.Data.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Infra.Data.Contexts
{
    public class DataContext : DbContext
    {
        //Método construtor para receber a conexão do banco de dados
        //Essa sintaxe é especifica do Entity Framework para fazer a
        //injeção de dependência passndo as configurações do banco
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoriaMap());
            modelBuilder.ApplyConfiguration(new ProdutoMap());
        }
    }
}
