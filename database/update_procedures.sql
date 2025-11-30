-- SQL script to update procedures used by the Lustrious project
-- Run this on your MySQL database (use the dbilumina database)

USE dbilumina;

DELIMITER $$

--1) selectUsuario: returns user list including Foto
DROP PROCEDURE IF EXISTS selectUsuario$$
CREATE PROCEDURE selectUsuario(IN p_role VARCHAR(20))
BEGIN
 SELECT IdUser, Nome, Email, Senha, Sexo, CPF, Role, Foto
 FROM Usuario
 WHERE Role = p_role
 ORDER BY Nome;
END$$

--2) obterUsuario: returns single user including Foto
DROP PROCEDURE IF EXISTS obterUsuario$$
CREATE PROCEDURE obterUsuario(IN vId INT)
BEGIN
 SELECT IdUser, Nome, Email, Senha, Sexo, CPF, Foto
 FROM Usuario
 WHERE IdUser = vId;
END$$

--3) updateUsuario: update user including Foto
DROP PROCEDURE IF EXISTS updateUsuario$$
CREATE PROCEDURE updateUsuario(
 IN vIdUser INT,
 IN vNome VARCHAR(200),
 IN vEmail VARCHAR(150),
 IN vSenha VARCHAR(250),
 IN vCPF VARCHAR(14),
 IN vSexo VARCHAR(20),
 IN vFoto VARCHAR(255)
)
BEGIN
 IF EXISTS(SELECT * FROM Usuario WHERE IdUser = vIdUser) THEN
 UPDATE Usuario
 SET Nome = vNome,
 Email = vEmail,
 Senha = vSenha,
 CPF = vCPF,
 Sexo = vSexo,
 Foto = vFoto
 WHERE IdUser = vIdUser;
 ELSE
 SELECT 'Usuario não encontrado' AS Mensagem;
 END IF;
END$$

--4) insertProduto: include foto parameter
DROP PROCEDURE IF EXISTS insertProduto$$
CREATE PROCEDURE insertProduto(
 IN vCodigoBarras BIGINT,
 IN vNomeProd VARCHAR(200),
 IN vQtd INT,
 IN vDescricao VARCHAR(250),
 IN vValorUnitario DECIMAL(7,2),
 IN vRole ENUM('Masculino','Feminino','Unissex'),
 IN vCodCategoria INT,
 IN vCodTipoProduto INT,
 IN vFoto VARCHAR(255)
)
BEGIN
 IF NOT EXISTS(SELECT CodigoBarras FROM Produto WHERE CodigoBarras = vCodigoBarras) THEN
 INSERT INTO Produto (CodigoBarras, NomeProd, qtd, Descricao, ValorUnitario, Genero, codCategoria, codTipoProduto, foto)
 VALUES (vCodigoBarras, vNomeProd, vQtd, vDescricao, vValorUnitario, vRole, vCodCategoria, vCodTipoProduto, vFoto);
 ELSE
 SELECT 'Produto já está registrado' AS Mensagem;
 END IF;
END$$

--5) updateProduto: include foto
DROP PROCEDURE IF EXISTS updateProduto$$
CREATE PROCEDURE updateProduto(
 IN vCodigo BIGINT,
 IN vNome VARCHAR(200),
 IN vQtd INT,
 IN vDesc VARCHAR(250),
 IN vValor DECIMAL(9,2),
 IN vRole ENUM('Masculino','Feminino','Unissex'),
 IN vCodCategoria INT,
 IN vCodTipoProduto INT,
 IN vFoto VARCHAR(255)
)
BEGIN
 IF EXISTS(SELECT CodigoBarras FROM Produto WHERE CodigoBarras = vCodigo) THEN
 UPDATE Produto
 SET NomeProd = vNome,
 qtd = vQtd,
 Descricao = vDesc,
 ValorUnitario = vValor,
 Foto = vFoto,
 Genero = vRole,
 codCategoria = vCodCategoria,
 codTipoProduto = vCodTipoProduto
 WHERE CodigoBarras = vCodigo;
 ELSE
 SELECT 'Produto não encontrado' AS Mensagem;
 END IF;
END$$

--6) selectProdutos: ensure Foto is returned
DROP PROCEDURE IF EXISTS selectProdutos$$
CREATE PROCEDURE selectProdutos(IN vCodTipoProduto INT)
BEGIN
 IF vCodTipoProduto IS NULL OR vCodTipoProduto =0 THEN
 SELECT p.CodigoBarras AS IdProduto,
 p.NomeProd AS Nome,
 p.qtd,
 p.Descricao AS descricao,
 p.ValorUnitario AS valor_unitario,
 p.Foto AS foto,
 p.Genero
 FROM Produto p
 ORDER BY p.NomeProd;
 ELSE
 SELECT p.CodigoBarras AS IdProduto,
 p.NomeProd AS Nome,
 p.qtd,
 p.Descricao AS descricao,
 p.ValorUnitario AS valor_unitario,
 p.Foto AS foto,
 p.Genero
 FROM Produto p
 WHERE p.codTipoProduto = vCodTipoProduto
 ORDER BY p.NomeProd;
 END IF;
END$$

DELIMITER ;

-- After running, verify results:
-- SELECT IdUser, Nome, Foto FROM Usuario LIMIT10;
-- SELECT CodigoBarras, NomeProd, Foto FROM Produto LIMIT10;

-- End of script
