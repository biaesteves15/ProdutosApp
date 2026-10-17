using Microsoft.EntityFrameworkCore;
using ProdutosApp.Infra.Data.Contexts;

namespace ProdutosApp.Tests;

internal static class TestDataContextFactory
{
    public static DataContext Create()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase($"ProdutosAppTests_{Guid.NewGuid()}")
            .Options;

        return new DataContext(options);
    }
}
