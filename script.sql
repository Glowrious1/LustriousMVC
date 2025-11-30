-- Criando a Data Base
create database dbilumina;
-- Usando a Data Base
use dbilumina;

-- Criação de tabelas





create table Usuario (
IdUser int primary key auto_increment ,
Nome varchar(155) ,
Foto varchar(255), 
Email varchar(155) ,
Senha varchar(150) ,
Sexo enum('Masculino','Feminino','Outro'),
CPF varchar(14) not null,
Role enum('Admin','Cliente','Funcionario'),
IdEndereco  int null,
Ativo char(1)  default '1'
);

select * from Usuario ;

/*
create table Funcionario (
IdFun int primary key auto_increment
foreing key (IdFun) references Login(IdLogin) on delete cascade
Fica a critério de vocês adicionar mais colunas.
Sugestões de colunas:
Salario decimal(9,2) not null,
Status ENUM("Ativo", "Inativo") default "Ativo"
);


Create table Cliente(
IdClient int primary key auto_increment,
Nome varchar(200) not null,
Email varchar(150) not null,
CPF varchar(14) unique not null, -- Estava 12 mudei pra 14 pq ia dar erro.
Senha varchar(250),
CepCli int -- CEP vai no endereço, da pra fazer integração com API legal, bem facil de fazer
);

create table Funcionario (
IdFun int primary key auto_increment,
Nome varchar(200) not null,
Email varchar(150) not null,
Senha varchar(250) not null
);
*/


create table Bairro (
IdBairro int primary key auto_increment,
Bairro varchar(200) not null
);

create table Cidade (
IdCidade int primary key auto_increment,
Cidade varchar(200) not null
);

create table Estado (
IdUF int primary key auto_increment,
UF char(2) not null
);

create table Endereco (
IdEndereco int primary key auto_increment,
CEP varchar(9) ,
Logradouro varchar(200) not null,
numero varchar(11) ,
complemento varchar(155),
IdBairro int not null,
IdCidade int not null,
IdEstado int not null,
IdUser int  null
);

create table Entrega(
IdEntrega int primary key auto_increment,
IdEndereco int not null,
DataEntrega dateTime,
ValorFrete decimal(7,2),
DataPrevista date,
 Status enum ('Pedido enviado','Produto saiu para entrega', 'Seu Produto Chegou'),
foreign key (IdEndereco) references Endereco(IdEndereco)
);

create table Produto(
CodigoBarras bigint primary key,
NomeProd varchar(200) not null,
qtd int,
foto varchar(255),
Genero Enum('Masculino','Feminino','Unissex'),
Descricao varchar(250),
ValorUnitario decimal(9,2)
);

create table Favoritos(
IdFav int primary key auto_increment,
IdUser int not null,
IdProd bigint not null,
foreign key (IdUser) references Usuario(IdUser) on delete cascade,
foreign key (IdProd) references Produto(CodigoBarras) on delete cascade
);

create table Carrinho(
IdCarrinho int primary key auto_increment,
IdProd bigint not null,
Qtd int not null,
ValorUnitario decimal(9,2),
ValorTotal decimal(9,2),
IdUser int not null,
foreign key (IdUser) references Usuario(IdUser) on delete cascade
-- Se for ter promoção colocar uma coluna pro preço do produto.
);

create table Categoria(
codCategoria int primary key auto_increment,
Categoria varchar(155)
);

create table tipoProduto(
codTipoProduto int primary key auto_increment,
TipoProduto varchar(255) not null,
codCategoria int, -- fk
foreign key (codCategoria) references Categoria(codCategoria)
);
 
 create table Venda(
IdVenda int primary key auto_increment,
NomeProd varchar(250) not null,
ValorTotal decimal(9,2) not null,
DataVenda Datetime,
IdUser int,
NF int,
IdEntrega int
);
 
 create table VendaProduto(
IdVendaProduto int primary key auto_increment,
 valorItem decimal(9,2),
 Qtd int,
 CodigoBarras bigint,
 IdVenda int
 );
 
 create table NotaFiscal (
 NF int primary key auto_increment,
 TotalNota decimal(9,2),
 DataEmissao date not null
);
 
ALTER TABLE Produto 
ADD COLUMN codCategoria INT,
ADD COLUMN codTipoProduto INT,
ADD CONSTRAINT fk_Produto_Categoria FOREIGN KEY (codCategoria) REFERENCES Categoria(codCategoria),
ADD CONSTRAINT fk_Produto_TipoProduto FOREIGN KEY (codTipoProduto) REFERENCES tipoProduto(codTipoProduto);

 
 -- Criando as chaves  Foreign key 
 
 Alter table Endereco
 add Constraint fk_IdBairro_Endereco foreign key (IdBairro) references Bairro(IdBairro),
 add Constraint fk_IdCidade_Endereco foreign key (IdCidade) references Cidade(IdCidade),
 add Constraint fk_IdEstado_Endereco foreign key (IdEstado) references Estado(IDUF),
 add Constraint fk_IdUser_Endereco foreign key (IdUser) References Usuario(IdUser) on delete set null;
 
 alter table Usuario add constraint fk_IdEndereco_Usuario foreign key(IdEndereco) references Endereco(IdEndereco);
 
 alter table VendaProduto add constraint fk_Codigobarras_Vendaproduto foreign key(CodigoBarras) references Produto(CodigoBarras),
 add constraint fk_IdVenda_Vendaproduto foreign key (IdVenda) references Venda(IdVenda);
 
 alter table Venda 
 add Constraint fk_IdUser_Venda foreign key (IdUser) references Usuario(IdUser),
 add Constraint fk_NF_Venda foreign key (NF) references NotaFiscal(NF),
 add Constraint fk_IdEntrega_Venda foreign key (IdEntrega) references Entrega(IdEntrega);
 
 
 
 
 
 
 
 
 
 
 
 -- Criando as Procedures
 delimiter $$
 create  procedure InsertCidade(
 in vCidade varchar(200)
 )
 begin 
  if not exists(select Cidade from Cidade where Cidade = vCidade) then
  
   insert into Cidade (Cidade) values (vCidade);
	else select("essa cidade ja está registrada");
   end if ;
   
 end  
 
 $$
 
 
 
  delimiter $$
 create  procedure InsertBairro(
 in vBairro varchar(200)
 )
 begin 
  if not exists(select Bairro from Bairro where Bairro = vBairro) then
  
   insert into Bairro (Bairro) values (vBairro);
	else select("esse Bairro ja está registrado");
   end if ;
   
 end  ;
 
 $$
 
 call InsertBairro ("Novo Horizonte");
 
 
 
  delimiter $$

-- Nessa parte é legal colocar todos os UFs, para colocar num selectzão que fica legal. Vou colocar o código com os inserts aqui, fica a critério de vocês utilizar ou não.
-- É melhor hard codar essa parte pq o usuário é burro.
/*
INSERT INTO Estado (UF) VALUES ('AC');
INSERT INTO Estado (UF) VALUES ('AL');
INSERT INTO Estado (UF) VALUES ('AP');
INSERT INTO Estado (UF) VALUES ('AM');
INSERT INTO Estado (UF) VALUES ('BA');
INSERT INTO Estado (UF) VALUES ('CE');
INSERT INTO Estado (UF) VALUES ('DF');
INSERT INTO Estado (UF) VALUES ('ES');
INSERT INTO Estado (UF) VALUES ('GO');
INSERT INTO Estado (UF) VALUES ('MA');
INSERT INTO Estado (UF) VALUES ('MT');
INSERT INTO Estado (UF) VALUES ('MS');
INSERT INTO Estado (UF) VALUES ('MG');
INSERT INTO Estado (UF) VALUES ('PA');
INSERT INTO Estado (UF) VALUES ('PB');
INSERT INTO Estado (UF) VALUES ('PR');
INSERT INTO Estado (UF) VALUES ('PE');
INSERT INTO Estado (UF) VALUES ('PI');
INSERT INTO Estado (UF) VALUES ('RJ');
INSERT INTO Estado (UF) VALUES ('RN');
INSERT INTO Estado (UF) VALUES ('RS');
INSERT INTO Estado (UF) VALUES ('RO');
INSERT INTO Estado (UF) VALUES ('RR');
INSERT INTO Estado (UF) VALUES ('SC');
INSERT INTO Estado (UF) VALUES ('SP');
INSERT INTO Estado (UF) VALUES ('SE');
INSERT INTO Estado (UF) VALUES ('TO');
INSERT INTO Categoria (Categoria) values ('Maquiagens');
INSERT INTO Categoria (Categoria) values ('Skincare');
INSERT INTO Categoria (Categoria) values ('Cabelo');
INSERT INTO Categoria (Categoria) values ('Corpo');
insert into tipoProduto  (TipoProduto,codCategoria) values ('Gloss',1);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Corretivo',1);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Base',1);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Rimel',1);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Batom',1);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Blush',1);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Hidratante fácial',2);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Prime',2);
insert into tipoProduto  (TipoProduto,codCategoria) values ('esfoliante facial',2);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Protetor',2);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Shampoo',3);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Condicionador',3);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Óleo',3);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Hidratante capilar',3);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Creme hidratante',4);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Desodorante',4);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Esfoliante',4);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Perfume',4);
insert into tipoProduto  (TipoProduto,codCategoria) values ('Creme de Barbear',4);

*/
 create  procedure InsertEstado(
 in vUF char(2)
 )
 begin 
  if not exists(select UF from Estado where UF = vUF) then
  
   insert into Estado (UF) values (vUF);
	else select("esse Estado ja está registrado");
   end if ;
   
 end  ;
 
 $$
 
 call InsertEstado ("SP");
 
 CEP varchar(9) ,
Logradouro varchar(200) not null,
numero varchar(11) ,
complemento varchar(155),
IdBairro int not null,
IdCidade int not null,
IdEstado int not null,
IdUser int not null
 
 delimiter $$
  create  procedure insertEndereco(
  in vCEP varchar(9),
  in vLogradouro varchar(200),
  in vNumero int ,
  in vComplemento varchar(155),
  in vCidade varchar(200),
  in vBairro varchar(200),
    in vUF char(2)
  )
  begin
    Declare dBairro int ;
    Declare dCidade int ;
    Declare dEstado int ;
    
    
    if not exists(select CEP from endereco where CEP = vCEP) then
	-- verifica ao registra um endereço se existe as informações das colunas foreign keys
    -- Bairro  
    if not exists(select IdBairro from bairro where Bairro = vBairro) then
    insert into Bairro(Bairro) values (vBairro);
     end if;
     set dBairro := (select IdBairro From Bairro where Bairro = vBairro);
     
      if not exists(select IdCidade from Cidade where Cidade = vCidade) then
    insert into Cidade(Cidade) values (vCidade);
     end if;
     set dCidade := (select IdCidade From Cidade where Cidade = vCidade);
     
      if not exists(select IdUF from Estado where UF = vUF) then
    insert into Estado(UF) values (vUF);
     end if;
     set dEstado := (select IdUF From Estado where UF = vUF);
     insert into Endereco (CEP,Logradouro,numero,complemento,IdBairro,IdCidade,IdEstado) values (vCEP,vLogradouro,vNumero,vComplemento,dBairro,dCidade,dEstado);
      else select("esse Endereço já está registrado!");
   end if ;
  end ;
   $$
   
   delimiter ;
 
 call insertEndereco ('06340250','Rua Do Jão','54','','Lapa','Morro Salgado','sp');
 delimiter $$
 
 select * from Endereco ;
 
 


-- Usuario
 
 delimiter $$
 create  procedure insertUsuario (
 in vNome varchar(250),
 in vEmail varchar(150),
 in vCPF varchar(12),
 in vSenha varchar(250),
 in vRole varchar(20),
 in vSexo varchar(20),
 in vFoto varchar(255)
 )
begin
 
   if not exists (select IdUser from Usuario where CPF = vCPF ) then
   
     insert into Usuario (Nome,Email,Foto,CPF,Senha,Sexo,Role,Ativo) values (vNome,vEmail,vFoto,vCPF,vSenha,vSexo,vRole,1);
     else select("Esse Usuario já está registrado");
     end if;
end ;
$$

call insertUsuario("Daniel","Daniel@email.com","124574242-21","12345","Admin","Masculino","");



delimiter $$
create procedure selectUsuario()
begin
 
select IdUser, Nome,Email,Senha,Sexo,CPF,Role,Foto from Usuario order by Nome; 
end $$

call selectUsuario;

delimiter $$
Create  procedure ObterUsuarioEmail(IN p_email varchar(100))
begin
select IdUser,Nome,Email, Senha,role,ativo,Sexo,CPF from usuario where email= p_email
limit 1;
end $$

use dbilumina ; 



create procedure obterUsuario (in vId int)
begin
  select IdUser,Nome,Email,Senha,Sexo,CPF from Usuario where IdUser = vId;
end $$	

call obterUsuario(1);

DELIMITER $$ 
CREATE  PROCEDURE updateUsuario( 
IN vIdUser INT, IN vNome VARCHAR(200), IN vEmail VARCHAR(150), 
IN vSenha VARCHAR(250), IN vCPF VARCHAR(14), IN vSexo VARCHAR(20), IN vFoto VARCHAR(255) ) 
BEGIN 
	IF EXISTS(SELECT * FROM Usuario WHERE IdUser = vIdUser) THEN 
		UPDATE Usuario SET Nome = vNome, Email = vEmail, Senha = vSenha, CPF = vCPF, Sexo = vSexo, Foto = vFoto WHERE IdUser = vIdUser; 
	ELSE SELECT 'Usuario não encontrado' AS Mensagem;
 END IF ; 
end $$ 



create procedure DeleteUsuario(in vIdUser int)
begin
  if exists (select IdUser from Usuario where IdUser = vIdUser)then
	 delete  from Usuario  where IdUser = vIdUser ;
     else select('Não existe este Usuario');
     end if ;
end $$
delimiter ;
-- Produtos

delimiter $$
create   procedure selectProdutos()
begin
  select 
    p.CodigoBarras,
    p.NomeProd,
    p.Foto,
    p.qtd,
    p.Descricao,
    p.ValorUnitario,
    p.Genero,
    c.Categoria as NomeCategoria,
    t.TipoProduto as NomeTipoProduto
  from Produto p
  left join Categoria c on p.codCategoria = c.codCategoria
  left join tipoProduto t on p.codTipoProduto = t.codTipoProduto
  order by p.NomeProd;
end$$
delimiter ;
call selectProdutos

delimiter $$ 
create  procedure updateProduto(
  in vCodigo bigint,
  in vNome varchar(200),
  in vQtd int,
  in vDesc varchar(250),
  in vValor decimal(9,2),
  in vRole enum('Masculino','Feminino','Unissex'),
  in vCodCategoria int,
  in vCodTipoProduto int,
  in vFoto varchar(255)
)
begin
  if exists(select CodigoBarras from Produto where CodigoBarras = vCodigo) then
  
    update Produto 
    set NomeProd = vNome,
        qtd = vQtd,
        Descricao = vDesc,
        ValorUnitario = vValor,
        Foto = vFoto,
        Genero = vRole,
        codCategoria = vCodCategoria,
        codTipoProduto = vCodTipoProduto
    where CodigoBarras = vCodigo;
    
  else
    select 'Produto não encontrado' as Mensagem;
  end if;
end$$
delimiter ;
call insertProduto (254932837248,'Pergume Lavuar',25,'Perfume mais que demais é lavuar',12.2,'Masculino',3,5);


delimiter $$
create procedure deleteProduto(in vCodigo bigint)
begin
  if exists(select CodigoBarras from Produto where CodigoBarras = vCodigo) then
    delete from Produto where CodigoBarras = vCodigo;
  else
    select 'Produto não encontrado';
  end if;
end$$
delimiter ;

delimiter $$
create  procedure selectProdutosPorCategoria(in vCodCategoria int)
begin
  select 
    p.CodigoBarras, p.NomeProd, p.qtd, p.Descricao, p.ValorUnitario,p.Foto, p.Genero,
    c.Categoria, t.TipoProduto
  from Produto p
  join Categoria c on p.codCategoria = c.codCategoria
  join tipoProduto t on p.codTipoProduto = t.codTipoProduto
  where p.codCategoria = vCodCategoria
  order by p.NomeProd;
end$$
delimiter ;

delimiter $$
create  procedure selectProdutosPorTipo(in vCodTipoProduto int)
begin
  select 
    p.CodigoBarras, p.NomeProd, p.qtd, p.Descricao, p.ValorUnitario,p.Foto, p.Genero,
    c.Categoria, t.TipoProduto
  from Produto p
  join Categoria c on p.codCategoria = c.codCategoria
  join tipoProduto t on p.codTipoProduto = t.codTipoProduto
  where p.codTipoProduto = vCodTipoProduto
  order by p.NomeProd;
end$$
delimiter ;





delimiter $$ 
create   procedure insertProduto(
  in vCodigoBarras bigint,
  in vNomeProd varchar(200),
  in vQtd int,
  in vDescricao varchar(250),
  in vValorUnitario decimal(7,2),
  in vRole enum('Masculino','Feminino','Unissex'),
  in vCodCategoria int,
  in vCodTipoProduto int,
  in vFoto varchar(255)
)
begin
  if not exists(select CodigoBarras from Produto where CodigoBarras = vCodigoBarras) then
  
    if exists(select codCategoria from Categoria where codCategoria = vCodCategoria)
       and exists(select codTipoProduto from tipoProduto where codTipoProduto = vCodTipoProduto) then
    
      insert into Produto 
      (CodigoBarras, NomeProd, qtd, Descricao, ValorUnitario, Genero, codCategoria, codTipoProduto,foto)
      values 
      (vCodigoBarras, vNomeProd, vQtd, vDescricao, vValorUnitario, vRole, vCodCategoria, vCodTipoProduto,vFoto);
      
    else 
      select 'Categoria ou Tipo de Produto inválido' as Mensagem;
    end if;
    
  else 
    select 'Produto já está registrado' as Mensagem;
  end if;
end$$
delimiter ;

call insertProduto (254932835248,'Perfume Maria',155,'Quer que sua princesa sinta o poder do bigode, use Pergume Maria',12.2,'Masculino',4,3,"foto");


delimiter $$
create procedure addCarrinho(
  in vIdUser int,
  in vCodigo bigint,
  in vQtd int
)
begin
  if exists(select CodigoBarras from Produto where CodigoBarras = vCodigo) then
    if exists(select IdCarrinho from Carrinho where IdUser = vIdUser and IdProd = vCodigo) then
      update Carrinho set Qtd = Qtd + vQtd 
      where IdUser = vIdUser and IdProd = vCodigo;
    else
      insert into Carrinho (IdUser, IdProd, Qtd) values (vIdUser, vCodigo, vQtd);
    end if;
  else
    select 'Produto inexistente';
  end if;
end$$
delimiter ;

delimiter $$
create procedure selectCarrinho(in vIdUser int)
begin
  select c.IdCarrinho, p.NomeProd,p.foto, c.Qtd, p.ValorUnitario, (p.ValorUnitario * c.Qtd) as Subtotal
  from Carrinho c
  join Produto p on c.IdProd = p.CodigoBarras
  where c.IdUser = vIdUser;
end$$
delimiter ;

delimiter $$
create procedure deleteCarrinhoItem(in vIdCarrinho int)
begin
  delete from Carrinho where IdCarrinho = vIdCarrinho;
end$$
delimiter ;


 use dbilumina ;
 
 
 -- Procedures adicionadas para compatibilizar com o código C# do projeto
-- Execute este script no banco `dbilumina` (mysql)

DELIMITER $$

-- Ajuste: selectProdutos com filtro por tipo (parâmetro opcional: passe  para todos)
DROP PROCEDURE IF EXISTS selectProdutos$$
CREATE PROCEDURE selectProdutos(IN vCodTipoProduto INT)
BEGIN
 IF vCodTipoProduto IS NULL OR vCodTipoProduto =0 THEN
 SELECT 
 p.CodigoBarras,
 p.NomeProd,
 p.qtd,
 p.Descricao,
 p.ValorUnitario,
 p.Genero,
 c.Categoria AS NomeCategoria,
 t.TipoProduto AS NomeTipoProduto
 FROM Produto p
 LEFT JOIN Categoria c ON p.codCategoria = c.codCategoria
 LEFT JOIN tipoProduto t ON p.codTipoProduto = t.codTipoProduto
 ORDER BY p.NomeProd;
 ELSE
 SELECT 
 p.CodigoBarras,
 p.NomeProd,
 p.qtd,
 p.Descricao,
 p.ValorUnitario,
 p.Genero,
 c.Categoria AS NomeCategoria,
 t.TipoProduto AS NomeTipoProduto
 FROM Produto p
 LEFT JOIN Categoria c ON p.codCategoria = c.codCategoria
 LEFT JOIN tipoProduto t ON p.codTipoProduto = t.codTipoProduto
 WHERE p.codTipoProduto = vCodTipoProduto
 ORDER BY p.NomeProd;
 END IF;
END$$

-- 2) obterEnderecosPorUsuario
DROP PROCEDURE IF EXISTS obterEnderecosPorUsuario$$
CREATE PROCEDURE obterEnderecosPorUsuario(IN vUserId INT)
BEGIN
 SELECT 
 IdEndereco,
 CEP AS Cep,
 Logradouro AS logradouro,
 numero,
 complemento,
 IdBairro AS Idbairro,
 IdCidade AS Idcidade,
 IdEstado AS Idestado,
 IdUser
 FROM Endereco
 WHERE IdUser = vUserId;
END$$

-- 3) updateEndereco
DROP PROCEDURE IF EXISTS updateEndereco$$
CREATE PROCEDURE updateEndereco(
 IN vIdEndereco INT,
 IN vCep VARCHAR(9),
 IN vLogradouro VARCHAR(200),
 IN vNumero VARCHAR(11),
 IN vComplemento VARCHAR(155),
 IN vIdBairro INT,
 IN vIdCidade INT,
 IN vIdEstado INT,
 IN vIdUser INT
)
BEGIN
 IF EXISTS (SELECT vIdEndereco FROM Endereco WHERE IdEndereco = vIdEndereco) THEN
 UPDATE Endereco
 SET CEP = vCep,
 Logradouro = vLogradouro,
 numero = vNumero,
 complemento = vComplemento,
 IdBairro = vIdBairro,
 IdCidade = vIdCidade,
 IdEstado = vIdEstado,
 IdUser = vIdUser
 WHERE IdEndereco = vIdEndereco;
 ELSE
 SELECT 'Endereço não encontrado' AS Mensagem;
 END IF;
END$$

-- 4) insertCarrinho (compatível com a lógica addCarrinho)
DROP PROCEDURE IF EXISTS insertCarrinho$$
CREATE PROCEDURE insertCarrinho(
 IN vUserId INT,
 IN vCodigo BIGINT,
 IN vQtd INT
)
BEGIN
 IF EXISTS(SELECT CodigoBarras FROM Produto WHERE CodigoBarras = vCodigo) THEN
 IF EXISTS(SELECT * FROM Carrinho WHERE IdUser = vUserId AND IdProd = vCodigo) THEN
 UPDATE Carrinho SET Qtd = Qtd + vQtd
 WHERE IdUser = vUserId AND IdProd = vCodigo;
 ELSE
 INSERT INTO Carrinho (IdUser, IdProd, Qtd) VALUES (vUserId, vCodigo, vQtd);
 END IF;
 ELSE
 SELECT 'Produto inexistente' AS Mensagem;
 END IF;
END$$

-- 5) insertVenda (insere venda e não precisa do NomeProd vindo do C#)
DROP PROCEDURE IF EXISTS insertVenda$$
CREATE PROCEDURE insertVenda(
 IN vIdUser INT,
 IN vDataVenda DATETIME,
 IN vValorTotal DECIMAL(9,2),
 IN vNF INT,
 IN vIdEntrega INT
)
BEGIN
 -- Insere com NomeProd vazio (o campo existe na tabela)
 INSERT INTO Venda (NomeProd, ValorTotal, DataVenda, IdUser, NF, IdEntrega)
 VALUES ('', vValorTotal, vDataVenda, vIdUser, vNF, vIdEntrega);
END$$

-- 6) insertVendaProduto
DROP PROCEDURE IF EXISTS insertVendaProduto$$
CREATE PROCEDURE insertVendaProduto(
 IN vIdVenda INT,
 IN vCodigoBarras BIGINT,
 IN vQtd INT,
 IN vValorItem DECIMAL(9,2)
)
BEGIN
 INSERT INTO VendaProduto (valorItem, Qtd, CodigoBarras, IdVenda)
 VALUES (vValorItem, vQtd, vCodigoBarras, vIdVenda);
END$$

-- 7) insertEntrega
DROP PROCEDURE IF EXISTS insertEntrega$$
CREATE PROCEDURE insertEntrega(
 IN vIdEndereco INT,
 IN vDataEntrega DATETIME,
 IN vValorFrete DECIMAL(7,2),
 IN vDataPrevista DATE,
 IN vStatus VARCHAR(100)
)
BEGIN
 INSERT INTO Entrega (IdEndereco, DataEntrega, ValorFrete, DataPrevista, Status)
 VALUES (vIdEndereco, vDataEntrega, vValorFrete, vDataPrevista, vStatus);
END$$

-- 8) obterVenda
DROP PROCEDURE IF EXISTS obterVenda$$
CREATE PROCEDURE obterVenda(IN vId INT)
BEGIN
 SELECT IdVenda, NomeProd, ValorTotal, DataVenda, IdUser, NF, IdEntrega
 FROM Venda
 WHERE IdVenda = vId;
END$$

-- 9) obterVendasPorUsuario
DROP PROCEDURE IF EXISTS obterVendasPorUsuario$$
CREATE PROCEDURE obterVendasPorUsuario(IN vUserId INT)
BEGIN
 SELECT IdVenda, NomeProd, ValorTotal, DataVenda, IdUser, NF, IdEntrega
 FROM Venda
 WHERE IdUser = vUserId
 ORDER BY DataVenda DESC;
END$$

-- 10) Corrigir/atualizar updateUsuario (existente no seu script aparenta ter inconsistência com vCPF)
DROP PROCEDURE IF EXISTS updateUsuario$$
CREATE PROCEDURE updateUsuario(
 IN vIdUser INT,
 IN vNome VARCHAR(200),
 IN vEmail VARCHAR(150),
 IN vSenha VARCHAR(250),
 IN vCPF VARCHAR(14),
 IN vSexo VARCHAR(20)
)
BEGIN
 IF EXISTS(SELECT * FROM Usuario WHERE IdUser = vIdUser) THEN
 UPDATE Usuario
 SET Nome = vNome,
 Email = vEmail,
 Senha = vSenha,
 CPF = vCPF,
 Sexo = vSexo
 WHERE IdUser = vIdUser;
 ELSE
 SELECT 'Usuario não encontrado' AS Mensagem;
 END IF;
END$$

DELIMITER ;

-- FIM


USE dbilumina;

-- Criar categorias necessárias se não existirem
INSERT INTO Categoria (Categoria)
SELECT 'Maquiagem' FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM Categoria WHERE Categoria = 'Maquiagem');

INSERT INTO Categoria (Categoria)
SELECT 'Cabelo' FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM Categoria WHERE Categoria = 'Cabelo');

-- Opcional: outras categorias usadas pelo projeto
INSERT INTO Categoria (Categoria)
SELECT 'Skincare' FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM Categoria WHERE Categoria = 'Skincare');

-- Inserir produtos (CodigoBarras é a PK). Ajuste os valores se já existirem.
-- Observação: ValorUnitario usa ponto decimal. Foto armazena o caminho relativo conforme as imagens em /public

INSERT INTO Produto (CodigoBarras, NomeProd, qtd, foto, Genero, Descricao, ValorUnitario, codCategoria)
VALUES
(1, 'Blush Compacto 4g - pêssego-terroso', 50, '/blush.png', 'Unissex', 'Um tom pêssego-terroso sofisticado que ilumina naturalmente a pele. Sua textura ultrafina desliza com suavidade, garantindo um esfumado impecável e acabamento aveludado.', 39.90, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(2, 'Creme Facial', 50, '/cremefacial.jpg', 'Unissex', 'Creme facial hidratante e restaurador.', 130.00, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(3, 'Prime', 50, '/Prime1.png', 'Unissex', 'Prime para preparação da pele, melhora a durabilidade da maquiagem.', 120.00, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(4, 'Rouge Royale – 6 ml', 50, '/gloss1.png', 'Unissex', 'Rouge Royale é um gloss criado para quem deseja dominar os holofotes. Seu tom vermelho profundo e acabamento luminoso.', 49.00, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(5, 'Crystal Frost – 6 ml', 50, '/gloss2.png', 'Unissex', 'Crystal Frost com microbrilhos prateados para um reflexo espelhado.', 49.00, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(6, 'Pink Velvet – 6 ml', 50, '/gloss3.png', 'Unissex', 'Pink Velvet tom rosa vibrante com acabamento luminoso.', 47.00, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(7, 'Bronze Amour – 6 ml', 50, '/gloss4.png', 'Unissex', 'Bronze Amour tonalidade bronze quente com partículas douradas.', 47.00, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(8, 'Cocoa Glow – 6 ml', 50, '/gloss5.png', 'Unissex', 'Cocoa Glow tom marrom suave com acabamento sofisticado.', 54.90, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(10, '15N – Ivory Glow', 50, '/base1.png', 'Unissex', 'Base 15N – Ivory Glow com cobertura média e acabamento aveludado.', 89.90, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(11, '20W – Golden Radiance', 50, '/base2.png', 'Unissex', 'Base 20W – Golden Radiance com acabamento radiante.', 89.90, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(12, '10N – Pearl Porcelain', 50, '/base3.png', 'Unissex', 'Base 10N – Pearl Porcelain entrega luminosidade e acabamento impecável.', 89.90, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(13, '40W – Caramel Essence', 50, '/base4.png', 'Unissex', 'Base 40W – Caramel Essence tonalidade caramelizada e acabamento luminoso.', 89.90, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(14, '30N – Nude Harmony', 50, '/base5.png', 'Unissex', 'Base 30N – Nude Harmony equilíbrio entre naturalidade e cobertura.', 89.90, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(15, '50C – Mocha Deep', 50, '/base6.png', 'Unissex', 'Base 50C – Mocha Deep formulada para peles profundas.', 89.90, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Maquiagem')),
(17, 'Shampoo de Banana', 100, '/Shampoo.png', 'Unissex', 'Hidratação suave e maciez imediata para fios radiantes.', 40.00, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Cabelo')),
(18, 'Condicionador de Banana', 100, '/Condicionador.png', 'Unissex', 'Hidratação profunda e nutrição imediata para fios mais fortes e macios.', 40.00, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Cabelo')),
(19, 'Hidratante Capilar de Banana', 100, '/Capilar.png', 'Unissex', 'Hidratação profunda e nutrição imediata.', 40.00, (SELECT codCategoria FROM Categoria WHERE Categoria = 'Cabelo'));

-- Commit explícito
COMMIT;

select * from Produto;

inser