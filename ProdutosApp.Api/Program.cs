using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

//Adicionando as configurações da biblioteca swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
