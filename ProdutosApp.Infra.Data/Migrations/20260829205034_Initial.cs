using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProdutosApp.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    DataHoraCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produto_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Nome",
                table: "Categoria",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produto_CategoriaId",
                table: "Produto",
                column: "CategoriaId");

            migrationBuilder.InsertData(
                table: "Categoria",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    {
                        new Guid("e3a92b17-6c84-4f05-9d21-8b7a13c6e490"),
                        "Informática"
                    },
                    {
                        new Guid("7b1f04d9-a632-48ec-b590-2d86f37a1c45"),
                        "Eletrônicos"
                    },
                    {
                        new Guid("c46d8a23-91f7-4b60-ae35-5f02d9c718ab"),
                        "Eletrodomésticos"
                    },
                    {
                        new Guid("19f7c2e8-5a43-46bd-8c90-e6317b4d2a05"),
                        "Móveis"
                    },
                    {
                        new Guid("a82e5d61-3b94-47c0-9f26-6d13e8a502bc"),
                        "Roupas"
                    },
                    {
                        new Guid("5d903a7c-e218-4f65-b4a9-71c6d02e83fb"),
                        "Calçados"
                    },
                    {
                        new Guid("f61b8c04-7d32-49ae-86f5-c2039a1d74e8"),
                        "Livros"
                    },
                    {
                        new Guid("2c74e9a5-b061-43df-a827-9e5d18c6f302"),
                        "Brinquedos"
                    },
                    {
                        new Guid("8a35d0f2-4e79-46b1-93c8-d6172f0a5eb4"),
                        "Esportes"
                    },
                    {
                        new Guid("d9076b3e-2a85-41fc-b649-3e8c50a712df"),
                        "Papelaria"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Categoria");
        }
    }
}
