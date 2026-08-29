using Microsoft.EntityFrameworkCore;
using ProdutosApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Infra.Data.Mappings
{
    public class CategoriaMap : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Categoria> builder)
        {
            //Chave Primária
            builder.HasKey(c => c.Id);

            //mapeamento dos demais campos
            builder.Property(c => c.Nome).HasMaxLength(50).IsRequired();

            //Indices para campos
            builder.HasIndex(c => c.Nome).IsUnique();
        }
    }
}
