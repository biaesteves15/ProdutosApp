using Microsoft.EntityFrameworkCore;
using ProdutosApp.Domain.Entities;
using ProdutosApp.Infra.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Infra.Data.Repositories
{
    public class CategoriaRepository
    {
        //Atributo privado e somente leitura
        private readonly DataContext _dataContext;

        //Método construtor para receber a injeção de dependência
        public CategoriaRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<Categoria> ListarTodos()
        {
            return _dataContext.Set<Categoria>()
                        .OrderBy(c => c.Nome)
                        .ToList();
        }
    }
}
