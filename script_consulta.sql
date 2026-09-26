-- Contagem de produtos por status
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

-- Contagem de produtos por tipo
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

-- Somatório da quantidade de produtos por Categoria
SELECT
	c.Nome AS Categoria,
	COALESCE(SUM(p.Quantidade),0) AS QuantidadeTotal
FROM Categoria AS c
LEFT JOIN Produto AS p
	ON p.CategoriaId = c.Id
GROUP BY c.Nome
ORDER BY QuantidadeTotal DESC;