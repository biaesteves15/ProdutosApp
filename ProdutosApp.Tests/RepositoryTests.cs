using Microsoft.EntityFrameworkCore;
using ProdutosApp.Domain.Entities;
using ProdutosApp.Domain.Enums;
using ProdutosApp.Infra.Data.Repositories;

namespace ProdutosApp.Tests;

public class RepositoryTests
{
    [Fact]
    public void CategoriaRepository_ListarTodos_OrdenaPorNome()
    {
        using var context = TestDataContextFactory.Create();
        context.AddRange(new Categoria { Nome = "Zeta" }, new Categoria { Nome = "Alfa" });
        context.SaveChanges();

        var categorias = new CategoriaRepository(context).ListarTodos();

        Assert.Equal(new[] { "Alfa", "Zeta" }, categorias.Select(c => c.Nome));
    }

    [Fact]
    public void ProdutoRepository_Inserir_PersisteProduto()
    {
        using var context = TestDataContextFactory.Create();
        var produto = NewProduto("Cabo");

        new ProdutoRepository(context).Inserir(produto);

        Assert.Same(produto, context.Set<Produto>().Single());
    }

    [Fact]
    public void ProdutoRepository_Atualizar_PersisteAlteracoes()
    {
        using var context = TestDataContextFactory.Create();
        var produto = NewProduto("Antes");
        context.Add(produto);
        context.SaveChanges();
        produto.Nome = "Depois";

        new ProdutoRepository(context).Atualizar(produto);

        Assert.Equal("Depois", context.Set<Produto>().Single().Nome);
    }

    [Fact]
    public void ProdutoRepository_Excluir_MarcaComoInativoSemRemover()
    {
        using var context = TestDataContextFactory.Create();
        var produto = NewProduto("Lápis");
        context.Add(produto);
        context.SaveChanges();

        new ProdutoRepository(context).Excluir(produto.Id);

        Assert.Equal(StatusProduto.Inativo, Assert.Single(context.Set<Produto>()).Status);
    }

    [Fact]
    public void ProdutoRepository_ListarPorNome_FiltraInativosEOrdena()
    {
        using var context = TestDataContextFactory.Create();
        context.AddRange(NewProduto("Mouse Z"), NewProduto("Mouse A"), NewProduto("Mouse oculto", status: StatusProduto.Inativo));
        context.SaveChanges();

        var produtos = new ProdutoRepository(context).ListarPorNome("Mouse");

        Assert.Equal(new[] { "Mouse A", "Mouse Z" }, produtos.Select(p => p.Nome));
    }

    [Fact]
    public void ProdutoRepository_ObterPorId_RetornaAtivoENaoRetornaInativo()
    {
        using var context = TestDataContextFactory.Create();
        var ativo = NewProduto("Ativo");
        var inativo = NewProduto("Inativo", status: StatusProduto.Inativo);
        context.AddRange(ativo, inativo);
        context.SaveChanges();
        var repository = new ProdutoRepository(context);

        Assert.Equal(ativo.Id, repository.ObterPorId(ativo.Id)!.Id);
        Assert.Null(repository.ObterPorId(inativo.Id));
        Assert.Null(repository.ObterPorId(Guid.NewGuid()));
    }

    [Fact]
    public void ProdutoRepository_ConsultarProdutosPorStatus_AgrupaEOrdenaPorContagem()
    {
        using var context = TestDataContextFactory.Create();
        context.AddRange(NewProduto("A", status: StatusProduto.Ativo), NewProduto("B", status: StatusProduto.Ativo), NewProduto("C", status: StatusProduto.Esgotado));
        context.SaveChanges();

        var resultados = new ProdutoRepository(context).ConsultarProdutosPorStatus();

        Assert.Equal("Ativo", resultados[0].status);
        Assert.Equal(2, resultados[0].contagemProdutos);
        Assert.Contains(resultados, item => item.status == "Esgotado" && item.contagemProdutos == 1);
    }

    [Fact]
    public void ProdutoRepository_ConsultarProdutosPorTipo_AgrupaEOrdenaPorContagem()
    {
        using var context = TestDataContextFactory.Create();
        context.AddRange(NewProduto("A", tipo: TipoProduto.Digital), NewProduto("B", tipo: TipoProduto.Digital), NewProduto("C", tipo: TipoProduto.Servico));
        context.SaveChanges();

        var resultados = new ProdutoRepository(context).ConsultarProdutosPorTipo();

        Assert.Equal("Digital", resultados[0].tipo);
        Assert.Equal(2, resultados[0].contagemProdutos);
        Assert.Contains(resultados, item => item.tipo == "Servico" && item.contagemProdutos == 1);
    }

    [Fact]
    public void ProdutoRepository_ConsultarProdutosPorCategoria_SomaEOrdenaPorQuantidade()
    {
        using var context = TestDataContextFactory.Create();
        var categoriaMaior = new Categoria { Nome = "Maior" };
        var categoriaMenor = new Categoria { Nome = "Menor" };
        context.AddRange(categoriaMaior, categoriaMenor);
        context.AddRange(
            new Produto { Nome = "A", Categoria = categoriaMaior, CategoriaId = categoriaMaior.Id, Quantidade = 3 },
            new Produto { Nome = "B", Categoria = categoriaMaior, CategoriaId = categoriaMaior.Id, Quantidade = 4 },
            new Produto { Nome = "C", Categoria = categoriaMenor, CategoriaId = categoriaMenor.Id, Quantidade = 2 });
        context.SaveChanges();

        var resultados = new ProdutoRepository(context).ConsultarProdutosPorCategoria();

        Assert.Equal("Maior", resultados[0].categoria);
        Assert.Equal(7, resultados[0].quantidadeTotal);
        Assert.Contains(resultados, item => item.categoria == "Menor" && item.quantidadeTotal == 2);
    }

    private static Produto NewProduto(
        string nome,
        StatusProduto status = StatusProduto.Ativo,
        TipoProduto tipo = TipoProduto.Fisico)
    {
        var categoria = new Categoria { Nome = $"Categoria {Guid.NewGuid()}" };
        return new Produto
        {
            Nome = nome, Status = status, Tipo = tipo,
            Categoria = categoria, CategoriaId = categoria.Id
        };
    }
}

public class EntityFrameworkMappingTests
{
    [Fact]
    public void CategoriaMap_ConfiguraChaveNomeObrigatorioTamanhoEIndiceUnico()
    {
        using var context = TestDataContextFactory.Create();
        var categoria = context.Model.FindEntityType(typeof(Categoria))!;
        var nome = categoria.FindProperty(nameof(Categoria.Nome))!;
        var indiceNome = Assert.Single(categoria.GetIndexes(), index => index.Properties.Single().Name == nameof(Categoria.Nome));

        Assert.Equal(nameof(Categoria.Id), Assert.Single(categoria.FindPrimaryKey()!.Properties).Name);
        Assert.False(nome.IsNullable);
        Assert.Equal(50, nome.GetMaxLength());
        Assert.True(indiceNome.IsUnique);
    }

    [Fact]
    public void ProdutoMap_ConfiguraCamposObrigatoriosPrecisaoEChaveEstrangeira()
    {
        using var context = TestDataContextFactory.Create();
        var produto = context.Model.FindEntityType(typeof(Produto))!;
        var nome = produto.FindProperty(nameof(Produto.Nome))!;
        var preco = produto.FindProperty(nameof(Produto.Preco))!;
        var quantidade = produto.FindProperty(nameof(Produto.Quantidade))!;
        var dataCriacao = produto.FindProperty(nameof(Produto.DataHoraCriacao))!;
        var fkCategoria = Assert.Single(produto.GetForeignKeys(), fk => fk.Properties.Single().Name == nameof(Produto.CategoriaId));

        Assert.Equal(nameof(Produto.Id), Assert.Single(produto.FindPrimaryKey()!.Properties).Name);
        Assert.False(nome.IsNullable);
        Assert.Equal(150, nome.GetMaxLength());
        Assert.False(preco.IsNullable);
        Assert.Equal(10, preco.GetPrecision());
        Assert.Equal(2, preco.GetScale());
        Assert.False(quantidade.IsNullable);
        Assert.False(dataCriacao.IsNullable);
        Assert.Equal(typeof(Categoria), fkCategoria.PrincipalEntityType.ClrType);
        Assert.Equal(DeleteBehavior.Cascade, fkCategoria.DeleteBehavior);
    }
}
