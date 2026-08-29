using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdutosApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Infra.Data.Mappings
{
    public class CategoriaMap : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            //Chave primária
            builder.HasKey(c => c.Id);

            //Mapeamento dos demais campos
            builder.Property(c => c.Nome).HasMaxLength(50).IsRequired();

            //Indices para campos
            builder.HasIndex(c => c.Nome).IsUnique();
        }
    }
}
