using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProdutosApp.Api.Controllers;
using ProdutosApp.Domain.Entities;
using ProdutosApp.Domain.Enums;
using ProdutosApp.Infra.Data.Contexts;

namespace ProdutosApp.IntegrationTests;

internal sealed class IntegrationApiFactory : IDisposable
{
    private readonly WebApplicationFactory<ProdutosController> _factory;
    private readonly HttpClient _client;

    public IntegrationApiFactory()
    {
        var databaseName = $"ProdutosAppIntegration_{Guid.NewGuid()}";
        _factory = new WebApplicationFactory<ProdutosController>()
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DataContext>();
                services.RemoveAll<DbContextOptions<DataContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<DataContext>>();
                services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(databaseName));
            }));

        _client = _factory.CreateClient();
    }

    public HttpClient Client => _client;

    public async Task SeedAsync(params Produto[] produtos)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var categorias = produtos
            .Select(produto => produto.Categoria!)
            .DistinctBy(categoria => categoria.Id)
            .ToArray();
        context.Set<Categoria>().AddRange(categorias);
        context.Set<Produto>().AddRange(produtos);
        await context.SaveChangesAsync();
    }

    public async Task SeedCategoriasAsync(params Categoria[] categorias)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Set<Categoria>().AddRange(categorias);
        await context.SaveChangesAsync();
    }

    public async Task<StatusProduto?> ObterStatusAsync(Guid produtoId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var produto = await context.Set<Produto>().FindAsync(produtoId);
        return produto?.Status;
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    public static Produto CriarProduto(
        string nome,
        Categoria? categoria = null,
        decimal preco = 25m,
        int quantidade = 1,
        StatusProduto status = StatusProduto.Ativo,
        TipoProduto tipo = TipoProduto.Fisico)
    {
        categoria ??= new Categoria { Nome = $"Categoria {Guid.NewGuid()}" };
        return new Produto
        {
            Nome = nome,
            Preco = preco,
            Quantidade = quantidade,
            Status = status,
            Tipo = tipo,
            Categoria = categoria,
            CategoriaId = categoria.Id
        };
    }
}
