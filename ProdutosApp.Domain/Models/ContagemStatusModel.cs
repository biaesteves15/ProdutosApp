using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Domain.Models
{
    public record ContagemStatusModel(
        string status,
        int contagemProdutos
     );
    
}
