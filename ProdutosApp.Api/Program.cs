using Microsoft.EntityFrameworkCore;
using ProdutosApp.Infra.Data.Contexts;
using ProdutosApp.Infra.Data.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

//Adicionando as configurações da biblioteca Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Configurações de injeção de dependência

//Registrando a classe DataContext para receber a conexão com o banco de dados
builder.Services.AddDbContext<DataContext>
    (options => options.UseSqlServer
        (builder.Configuration.GetConnectionString("DefaultConnection")));

//Registrando os repositórios
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<ProdutoRepository>();

#endregion

#region Configurações do CORS

// Capturando as origens permitidas do appsettings.json
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#endregion

var app = builder.Build();

//Ativando o Swagger quando o projeto for inicializado
app.UseSwagger();
app.UseSwaggerUI();

//Ativando a documentação do Scalar
app.MapScalarApiReference(s => { s.WithTheme(ScalarTheme.BluePlanet); });

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

/// Habilitando o Cors
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
