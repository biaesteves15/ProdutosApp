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

        [HttpPatch("{id}")]
        public IActionResult Put(Guid id, [FromBody] Dictionary<string, object> request)
        {
            var produto = _produtoRepository.ObterPorId(id);
            if (produto == null)
            {
                return NotFound(new { mensagem = "Produto não encontrado para edição." });
            }

            //Modificando o nome do produto de o campo vier preeenchido
            if (request.TryGetValue("nome", out var nome) && !string.IsNullOrWhiteSpace(nome?.ToString()))
                produto.Nome = nome.ToString()!;

            //Modificando o preço do produto de o campo vier preeenchido
            if (request.TryGetValue("preco", out var preco) && !string.IsNullOrWhiteSpace(preco?.ToString()))
                produto.Preco = decimal.Parse(preco?.ToString()!);

            //Modificando a quantidade do produto de o campo vier preeenchido
            if (request.TryGetValue("quantidade", out var quantidade) && !string.IsNullOrWhiteSpace(quantidade?.ToString()))
                produto.Quantidade = int.Parse(quantidade?.ToString()!);

            //Modificando o tipo do produto de o campo vier preeenchido
            if (request.TryGetValue("tipo", out var tipo) && !string.IsNullOrWhiteSpace(tipo?.ToString()))
                produto.Tipo = (TipoProduto) int.Parse(tipo.ToString()!);

            //Modificando o id da categoria do produto de o campo vier preeenchido
            if (request.TryGetValue("categoria_id", out var categoriaId) && !string.IsNullOrWhiteSpace(categoriaId?.ToString()))
                produto.CategoriaId = Guid.Parse(categoriaId.ToString()!);

            //Atualizar o produto no repositório
            _produtoRepository.Atualizar(produto);

            return Ok(new
            {
                mensagem = "Produto atualizado com sucesso!",
                data_hora = DateTime.Now,
                produto_id = produto.Id
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _produtoRepository.Excluir(id);

            return Ok(new
            {
                mensagem = "Produto excluído com sucesso!",
                data_hora = DateTime.Now,
                produto_id = id
            });
        }

        [HttpGet]
        public IActionResult GetPorNome([FromQuery] string nome)
        {
            if (string.IsNullOrEmpty(nome))
                return BadRequest(new { mensagem = "O nome é obrigatório." });

            var produtos = _produtoRepository.ListarPorNome(nome);
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetPorId(Guid id)
        {
            var produto = _produtoRepository.ObterPorId(id);
            if(produto == null)
            {
                return NotFound(new { mensagem = "Produto não encontrado." });
            }

            return Ok(produto);
        }
    }
}
