using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdutosApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Infra.Data.Mappings
{
    public class ProdutoMap : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            //Chave primária
            builder.HasKey(p => p.Id);

            //Demais propriedades
            builder.Property(p => p.Nome).HasMaxLength(150).IsRequired();
            builder.Property(p => p.Preco).HasPrecision(10, 2).IsRequired();
            builder.Property(p => p.Quantidade).IsRequired();
            builder.Property(p => p.DataHoraCriacao).IsRequired();

            //Relacionamento (1 para muitos)
            builder.HasOne(p => p.Categoria) //Produto TEM 1 Categoria
                .WithMany(c => c.Produtos) //Categoria TEM MUITOS Produtos
                .HasForeignKey(p => p.CategoriaId); //Chave estrangeira
        }
    }
}