INSERT INTO Categoria(ID, NOME)
VALUES
	(NEWID(), 'Informática'),
	(NEWID(), 'Eletrônicos'),
	(NEWID(), 'Games'),
	(NEWID(), 'Papelaria'),
	(NEWID(), 'Vestuário'),
	(NEWID(), 'Outros');

SELECT * FROM Categoria
ORDER BY Nome;