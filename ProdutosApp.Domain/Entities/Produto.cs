using ProdutosApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Domain.Entities
{
    public class Produto
    {
        #region Propriedades

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; } = 0;
        public int Quantidade { get; set; } = 0;
        public DateTime DataHoraCriacao { get; set; } = DateTime.Now;
        public StatusProduto Status { get; set; }
        public TipoProduto Tipo { get; set; }
        public Guid CategoriaId { get; set; } //Chave estrangeira

        #endregion

        #region Relacionamentos

        public Categoria? Categoria { get; set; }

        #endregion
    }
}
