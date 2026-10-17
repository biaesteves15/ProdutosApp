using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProdutosApp.Domain.Entities;
using ProdutosApp.Domain.Enums;

namespace ProdutosApp.IntegrationTests;

public class ProdutosApiIntegrationTests
{
    [Fact]
    public async Task PostProduto_PersisteEDepoisPodeSerConsultadoPorId()
    {
        using var app = new IntegrationApiFactory();
        var categoria = new Categoria { Nome = "Teclados" };
        await app.SeedCategoriasAsync(categoria);

        var response = await app.Client.PostAsJsonAsync("/api/v1/produtos", new
        {
            nome = "Teclado",
            preco = "120",
            quantidade = "3",
            tipo = "1",
            categoria_id = categoria.Id.ToString()
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var criado = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var produtoId = criado.RootElement.GetProperty("produto_id").GetGuid();

        var consulta = await app.Client.GetAsync($"/api/v1/produtos/{produtoId}");
        Assert.Equal(HttpStatusCode.OK, consulta.StatusCode);
        using var produto = JsonDocument.Parse(await consulta.Content.ReadAsStringAsync());
        Assert.Equal("Teclado", produto.RootElement.GetProperty("nome").GetString());
        Assert.Equal(120, produto.RootElement.GetProperty("preco").GetDecimal());
        Assert.Equal((int)StatusProduto.Ativo, produto.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("Teclados", produto.RootElement.GetProperty("categoria").GetProperty("nome").GetString());
    }

    [Fact]
    public async Task GetProdutosPorNome_AplicaFiltroEDevolveJson()
    {
        using var app = new IntegrationApiFactory();
        await app.SeedAsync(
            IntegrationApiFactory.CriarProduto("Mouse sem fio"),
            IntegrationApiFactory.CriarProduto("Teclado"));

        var response = await app.Client.GetAsync("/api/v1/produtos?nome=Mouse");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Mouse sem fio", Assert.Single(json.RootElement.EnumerateArray()).GetProperty("nome").GetString());
    }

    [Fact]
    public async Task GetProdutosPorNome_RetornaBadRequest_QuandoNomeNaoInformado()
    {
        using var app = new IntegrationApiFactory();

        var response = await app.Client.GetAsync("/api/v1/produtos?nome=");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProdutoPorId_RetornaNotFound_QuandoNaoExiste()
    {
        using var app = new IntegrationApiFactory();

        var response = await app.Client.GetAsync($"/api/v1/produtos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchProduto_AtualizaCamposEnviados()
    {
        using var app = new IntegrationApiFactory();
        var produto = IntegrationApiFactory.CriarProduto("Antes", quantidade: 2);
        await app.SeedAsync(produto);

        var response = await app.Client.PatchAsJsonAsync($"/api/v1/produtos/{produto.Id}", new
        {
            nome = "Depois",
            quantidade = "7"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var consulta = await app.Client.GetAsync($"/api/v1/produtos/{produto.Id}");
        using var json = JsonDocument.Parse(await consulta.Content.ReadAsStringAsync());
        Assert.Equal("Depois", json.RootElement.GetProperty("nome").GetString());
        Assert.Equal(7, json.RootElement.GetProperty("quantidade").GetInt32());
    }

    [Fact]
    public async Task PatchProduto_RetornaNotFound_QuandoProdutoNaoExiste()
    {
        using var app = new IntegrationApiFactory();

        var response = await app.Client.PatchAsJsonAsync($"/api/v1/produtos/{Guid.NewGuid()}", new { nome = "Novo" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduto_MarcaInativoEDeixaDeSerConsultavel()
    {
        using var app = new IntegrationApiFactory();
        var produto = IntegrationApiFactory.CriarProduto("Descartado");
        await app.SeedAsync(produto);

        var response = await app.Client.DeleteAsync($"/api/v1/produtos/{produto.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(StatusProduto.Inativo, await app.ObterStatusAsync(produto.Id));
        Assert.Equal(HttpStatusCode.NotFound, (await app.Client.GetAsync($"/api/v1/produtos/{produto.Id}")).StatusCode);
    }
}

public class CategoriaApiIntegrationTests
{
    [Fact]
    public async Task GetCategorias_RetornaListaOrdenada()
    {
        using var app = new IntegrationApiFactory();
        await app.SeedCategoriasAsync(new Categoria { Nome = "Zeta" }, new Categoria { Nome = "Alfa" });

        var response = await app.Client.GetAsync("/api/v1/categorias");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(new[] { "Alfa", "Zeta" }, json.RootElement.EnumerateArray().Select(item => item.GetProperty("nome").GetString()));
    }
}

public class DashboardApiIntegrationTests
{
    [Fact]
    public async Task GetProdutosPorStatus_RetornaContagensPelaApi()
    {
        using var app = new IntegrationApiFactory();
        await app.SeedAsync(
            IntegrationApiFactory.CriarProduto("A", status: StatusProduto.Ativo),
            IntegrationApiFactory.CriarProduto("B", status: StatusProduto.Ativo),
            IntegrationApiFactory.CriarProduto("C", status: StatusProduto.Esgotado));

        var response = await app.Client.GetAsync("/api/v1/dashboard/produtos-por-status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Ativo", json.RootElement[0].GetProperty("status").GetString());
        Assert.Equal(2, json.RootElement[0].GetProperty("contagemProdutos").GetInt32());
    }

    [Fact]
    public async Task GetProdutosPorTipo_RetornaContagensPelaApi()
    {
        using var app = new IntegrationApiFactory();
        await app.SeedAsync(
            IntegrationApiFactory.CriarProduto("A", tipo: TipoProduto.Digital),
            IntegrationApiFactory.CriarProduto("B", tipo: TipoProduto.Digital),
            IntegrationApiFactory.CriarProduto("C", tipo: TipoProduto.Servico));

        var response = await app.Client.GetAsync("/api/v1/dashboard/produtos-por-tipo");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Digital", json.RootElement[0].GetProperty("tipo").GetString());
        Assert.Equal(2, json.RootElement[0].GetProperty("contagemProdutos").GetInt32());
    }

    [Fact]
    public async Task GetProdutosPorCategoria_RetornaSomaPelaApi()
    {
        using var app = new IntegrationApiFactory();
        var categoria = new Categoria { Nome = "Livros" };
        await app.SeedAsync(
            IntegrationApiFactory.CriarProduto("A", categoria, quantidade: 3),
            IntegrationApiFactory.CriarProduto("B", categoria, quantidade: 5));

        var response = await app.Client.GetAsync("/api/v1/dashboard/produtos-por-categoria");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Livros", json.RootElement[0].GetProperty("categoria").GetString());
        Assert.Equal(8, json.RootElement[0].GetProperty("quantidadeTotal").GetInt32());
    }
}
