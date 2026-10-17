using Microsoft.AspNetCore.Mvc;
using ProdutosApp.Api.Controllers;
using ProdutosApp.Domain.Entities;
using ProdutosApp.Domain.Enums;
using ProdutosApp.Infra.Data.Repositories;

namespace ProdutosApp.Tests;

public class ControllerTests
{
    [Fact]
    public void ProdutosController_Post_IncluiProdutoAtivo()
    {
        using var context = TestDataContextFactory.Create();
        var categoria = new Categoria { Nome = "Informática" };
        context.Add(categoria);
        context.SaveChanges();
        var controller = new ProdutosController(new ProdutoRepository(context));
        var request = new Dictionary<string, object>
        {
            ["nome"] = "Teclado", ["preco"] = "99,90", ["quantidade"] = "4",
            ["tipo"] = "1", ["categoria_id"] = categoria.Id.ToString()
        };

        var result = Assert.IsType<OkObjectResult>(controller.Post(request));
        var salvo = Assert.Single(context.Set<Produto>());
        Assert.Equal("Teclado", salvo.Nome);
        Assert.Equal(99.90m, salvo.Preco);
        Assert.Equal(4, salvo.Quantidade);
        Assert.Equal(TipoProduto.Fisico, salvo.Tipo);
        Assert.Equal(StatusProduto.Ativo, salvo.Status);
        Assert.Equal(categoria.Id, salvo.CategoriaId);
        Assert.NotEqual(Guid.Empty, result.Value!.GetType().GetProperty("produto_id")!.GetValue(result.Value));
    }

    [Fact]
    public void ProdutosController_Put_RetornaNotFound_QuandoProdutoNaoExiste()
    {
        using var context = TestDataContextFactory.Create();
        var result = new ProdutosController(new ProdutoRepository(context))
            .Put(Guid.NewGuid(), new Dictionary<string, object>());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void ProdutosController_Put_AtualizaSomenteCamposInformados()
    {
        using var context = TestDataContextFactory.Create();
        var produto = AddProduto(context, "Antes", 12m, 2);
        var result = new ProdutosController(new ProdutoRepository(context)).Put(produto.Id,
            new Dictionary<string, object> { ["nome"] = "Depois", ["quantidade"] = "8", ["preco"] = " " });

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Depois", produto.Nome);
        Assert.Equal(8, produto.Quantidade);
        Assert.Equal(12m, produto.Preco);
    }

    [Fact]
    public void ProdutosController_Delete_MarcaProdutoComoInativo()
    {
        using var context = TestDataContextFactory.Create();
        var produto = AddProduto(context);

        var result = new ProdutosController(new ProdutoRepository(context)).Delete(produto.Id);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusProduto.Inativo, context.Set<Produto>().Single().Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ProdutosController_GetPorNome_RetornaBadRequest_ParaNomeVazio(string? nome)
    {
        using var context = TestDataContextFactory.Create();
        var result = new ProdutosController(new ProdutoRepository(context)).GetPorNome(nome!);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void ProdutosController_GetPorNome_RetornaResultados()
    {
        using var context = TestDataContextFactory.Create();
        AddProduto(context, "Teclado mecânico");
        var result = new ProdutosController(new ProdutoRepository(context)).GetPorNome("Teclado");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Single(Assert.IsAssignableFrom<List<Produto>>(ok.Value));
    }

    [Fact]
    public void ProdutosController_GetPorId_RetornaNotFound_QuandoNaoExiste()
    {
        using var context = TestDataContextFactory.Create();
        var result = new ProdutosController(new ProdutoRepository(context)).GetPorId(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void ProdutosController_GetPorId_RetornaProdutoEncontrado()
    {
        using var context = TestDataContextFactory.Create();
        var produto = AddProduto(context, "Mouse");
        var result = new ProdutosController(new ProdutoRepository(context)).GetPorId(produto.Id);

        Assert.Same(produto, Assert.IsType<OkObjectResult>(result).Value);
    }

    [Fact]
    public void CategoriaController_Get_RetornaCategoriasOrdenadas()
    {
        using var context = TestDataContextFactory.Create();
        context.AddRange(new Categoria { Nome = "Zeta" }, new Categoria { Nome = "Alfa" });
        context.SaveChanges();

        var result = new CategoriaController(new CategoriaRepository(context)).Get();

        var categorias = Assert.IsAssignableFrom<List<Categoria>>(Assert.IsType<OkObjectResult>(result).Value);
        Assert.Equal(new[] { "Alfa", "Zeta" }, categorias.Select(c => c.Nome));
    }

    [Fact]
    public void DashboardController_GetProdutosPorStatus_RetornaContagens()
    {
        using var context = TestDataContextFactory.Create();
        AddProduto(context, status: StatusProduto.Ativo);
        AddProduto(context, status: StatusProduto.Ativo);
        AddProduto(context, status: StatusProduto.Esgotado);

        var result = new DashboardController(new ProdutoRepository(context)).GetProdutosPorStatus();

        var dados = Assert.IsAssignableFrom<List<ProdutosApp.Domain.Models.ContagemStatusModel>>(Assert.IsType<OkObjectResult>(result).Value);
        Assert.Equal(2, dados[0].contagemProdutos);
        Assert.Equal("Ativo", dados[0].status);
    }

    [Fact]
    public void DashboardController_GetProdutosPorTipo_RetornaContagens()
    {
        using var context = TestDataContextFactory.Create();
        AddProduto(context, tipo: TipoProduto.Digital);
        AddProduto(context, tipo: TipoProduto.Digital);
        AddProduto(context, tipo: TipoProduto.Servico);

        var result = new DashboardController(new ProdutoRepository(context)).GetProdutosPorTipo();

        var dados = Assert.IsAssignableFrom<List<ProdutosApp.Domain.Models.ContagemTipoModel>>(Assert.IsType<OkObjectResult>(result).Value);
        Assert.Equal(2, dados[0].contagemProdutos);
        Assert.Equal("Digital", dados[0].tipo);
    }

    [Fact]
    public void DashboardController_GetProdutosPorCategoria_RetornaSomaDasQuantidades()
    {
        using var context = TestDataContextFactory.Create();
        var categoria = new Categoria { Nome = "Livros" };
        context.Add(categoria);
        context.AddRange(
            new Produto { Nome = "A", CategoriaId = categoria.Id, Categoria = categoria, Quantidade = 3 },
            new Produto { Nome = "B", CategoriaId = categoria.Id, Categoria = categoria, Quantidade = 5 });
        context.SaveChanges();

        var result = new DashboardController(new ProdutoRepository(context)).GetProdutosPorCategoria();

        var dado = Assert.Single(Assert.IsAssignableFrom<List<ProdutosApp.Domain.Models.SomatorioCategoriaModel>>(Assert.IsType<OkObjectResult>(result).Value));
        Assert.Equal("Livros", dado.categoria);
        Assert.Equal(8, dado.quantidadeTotal);
    }

    private static Produto AddProduto(
        ProdutosApp.Infra.Data.Contexts.DataContext context,
        string nome = "Produto",
        decimal preco = 10m,
        int quantidade = 1,
        StatusProduto status = StatusProduto.Ativo,
        TipoProduto tipo = TipoProduto.Fisico)
    {
        var categoria = new Categoria { Nome = $"Categoria {Guid.NewGuid()}" };
        var produto = new Produto
        {
            Nome = nome, Preco = preco, Quantidade = quantidade, Status = status, Tipo = tipo,
            Categoria = categoria, CategoriaId = categoria.Id
        };
        context.Add(produto);
        context.SaveChanges();
        return produto;
    }
}
