using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProdutosApp.Domain.Entities;
using ProdutosApp.Domain.Enums;
using ProdutosApp.Infra.Data.Repositories;

namespace ProdutosApp.Api.Controllers
{
    [Route("api/v1/produtos")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        //Atributo privado
        private readonly ProdutoRepository _produtoRepository;

        //Construtor para injeção de dependência
        public ProdutosController(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Dictionary<string, object> request)
        {
            var produto = new Produto()
            {
                Nome = request["nome"].ToString()!,
                Preco = decimal.Parse(request["preco"].ToString()!),
                Quantidade = int.Parse(request["quantidade"].ToString()!),
                Tipo = (TipoProduto) int.Parse(request["tipo"].ToString()!),
                CategoriaId = Guid.Parse(request["categoria_id"].ToString()!),
                Status = StatusProduto.Ativo
            };

            _produtoRepository.Inserir(produto);

            return Ok(new { 
                mensagem = "Produto cadastrado com sucesso!",
                data_hora = DateTime.Now,
                produto_id = produto.Id
            });
        }

        [HttpPut]
        public IActionResult Put()
        {
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
