using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProdutosApp.Infra.Data.Repositories;

namespace ProdutosApp.Api.Controllers
{
    [Route("api/v1/categorias")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        //Atributo privado
        private readonly CategoriaRepository _categoriaRepository;

        //Construtor para injeção de dependência
        public CategoriaController(CategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var categorias = _categoriaRepository.ListarTodos();

            return Ok(categorias);
        }
    }
}
