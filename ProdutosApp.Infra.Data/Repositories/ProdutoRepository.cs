using Microsoft.EntityFrameworkCore;
using ProdutosApp.Domain.Entities;
using ProdutosApp.Domain.Enums;
using ProdutosApp.Infra.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProdutosApp.Infra.Data.Repositories
{
    public class ProdutoRepository
    {
        //Atributo privado e somente leitura
        private readonly DataContext _dataContext;

        //Método construtor para injeção de dependência
        public ProdutoRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Inserir(Produto produto)
        {
            _dataContext.Set<Produto>().Add(produto);
            _dataContext.SaveChanges();
        }

        public void Atualizar(Produto produto)
        {
            _dataContext.Set<Produto>().Update(produto);
            _dataContext.SaveChanges();
        }

        public void Excluir(Guid id)
        {
            var produto = _dataContext.Set<Produto>().Find(id);

            produto!.Status = StatusProduto.Inativo; //Exclusão lógica

            _dataContext.Set<Produto>().Update(produto);
            _dataContext.SaveChanges();
        }

        public List<Produto> ListarPorNome(string nome)
        {
            return _dataContext.Set<Produto>()
                    .Include(p => p.Categoria) //LEFT JOIN
                    .Where(p => p.Nome.Contains(nome)
                             && p.Status != StatusProduto.Inativo)
                    .OrderBy(p => p.Nome)
                    .ToList();
        }

        public Produto? ObterPorId(Guid id)
        {
            return _dataContext.Set<Produto>()
                    .Include(p => p.Categoria)
                    .Where(p => p.Id == id
                             && p.Status != StatusProduto.Inativo)
                    .SingleOrDefault();
        }
    }
}
