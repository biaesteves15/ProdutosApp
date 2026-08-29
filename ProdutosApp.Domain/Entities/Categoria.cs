using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Domain.Entities
{
    public class Categoria
    {
        #region Propriedades

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;

        #endregion

        #region Relacionamentos

        public List<Produto>? Produtos { get; set; }

        #endregion
    }
}
