using Microsoft.EntityFrameworkCore;
using ProdutosApp.Domain.Entities;
using ProdutosApp.Domain.Enums;
using ProdutosApp.Domain.Models;
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

        //Método para retornar a contagem de produtos por status
        /*
            SELECT
	            CASE p.Status
		            WHEN 1 THEN 'Ativo'
		            WHEN 2 THEN 'Inativo'
		            WHEN 3 THEN 'Esgotado'
		            ELSE 'Desconhecido'
	            END AS Status,
	            COUNT(p.Id) AS ContagemProdutos
            FROM Produto AS p
            GROUP BY p.Status
            ORDER BY ContagemProdutos DESC
         */
        public List<ContagemStatusModel> ConsultarProdutosPorStatus()
        {
            var consulta = _dataContext.Set<Produto>()
                .GroupBy(p => p.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    ContagemProdutos = g.Count()
                })
                .OrderByDescending(p => p.ContagemProdutos)
                .ToList();

            return consulta
                .Select(p => new ContagemStatusModel(
                    p.Status.ToString(),
                    p.ContagemProdutos
                ))
                .ToList();
        }

        //Método para retornar a contagem de produto por tipo
        /*
            SELECT
	            CASE p.Tipo
		            WHEN 1 THEN 'Fisico'
		            WHEN 2 THEN 'Digital'
		            WHEN 3 THEN 'Servico'
		            ELSE 'Desconhecido'
	            END AS Tipo,
	            COUNT(p.Id) AS ContagemProdutos
            FROM Produto AS p
            GROUP BY p.Tipo
            ORDER BY ContagemProdutos DESC
        */
        public List<ContagemTipoModel> ConsultarProdutosPorTipo()
        {
            var consulta = _dataContext.Set<Produto>()
                .GroupBy(p => p.Tipo)
                .Select(g => new
                {
                    Tipo = g.Key,
                    ContagemProdutos = g.Count()
                })
                .OrderByDescending(p => p.ContagemProdutos)
                .ToList();

            return consulta
                .Select(p => new ContagemTipoModel(
                    p.Tipo.ToString(),
                    p.ContagemProdutos
                ))
                .ToList();
        }

        //Método para retornar o somatorio da quantidade de produtos por categoria
        /*
            SELECT
	            c.Nome AS Categoria,
	            COALESCE(SUM(p.Quantidade),0) AS QuantidadeTotal
            FROM Categoria AS c
            LEFT JOIN Produto AS p
	            ON p.CategoriaId = c.Id
            GROUP BY c.Nome
            ORDER BY QuantidadeTotal DESC;
         */
        public List<SomatorioCategoriaModel> ConsultarProdutosPorCategoria()
        {
            var consulta = _dataContext.Set<Produto>()
                    .Include(p => p.Categoria)
                    .GroupBy(p => p.Categoria!.Nome)
                    .Select(g => new
                    {
                        Categoria = g.Key,
                        QuantidadeTotal = g.Sum(p => p.Quantidade)
                    })
                    .OrderByDescending(p => p.QuantidadeTotal)
                    .ToList();

            return consulta
                .Select(p => new SomatorioCategoriaModel(
                    p.Categoria,
                    p.QuantidadeTotal
                ))
                .ToList();
        }
    }
}
