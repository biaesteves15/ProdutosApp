using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProdutosApp.Api.Controllers
{
    [Route("api/v1/categorias")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
