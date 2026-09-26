using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProdutosApp.Infra.Data.Repositories;

namespace ProdutosApp.Api.Controllers
{
    [Route("api/v1/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        //atributo privado
        private readonly ProdutoRepository _produtoRepository;

        //construtor para injeção de dependência
        public DashboardController(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        [HttpGet("produtos-por-status")]
        public IActionResult GetProdutosPorStatus()
        {
            return Ok(_produtoRepository.ConsultarProdutosPorStatus());
        }

        [HttpGet("produtos-por-tipo")]
        public IActionResult GetProdutosPorTipo()
        {
            return Ok(_produtoRepository.ConsultarProdutosPorTipo());
        }

        [HttpGet("produtos-por-categoria")]
        public IActionResult GetProdutosPorCategoria()
        {
            return Ok(_produtoRepository.ConsultarProdutosPorCategoria());
        }
    }
}
