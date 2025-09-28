-- Criando a Data Base
create database dbilumina;
-- Usando a Data Base
use dbilumina ;

-- Criação de tabelas

Create table Cliente(
IdClient int primary key auto_increment,
Nome varchar(200) not null,
Email varchar(150) not null,
CPF varchar(12) unique not null,
Senha varchar(250),
CepCli int
);

create table Funcionario (
IdFun int primary key auto_increment,
Nome varchar(200) not null,
Email varchar(150) not null,
Senha varchar(250) not null
);

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
CEP int primary key,
Logradouro varchar(200) not null,
IdBairro int not null,
IdCidade int not null,
IdEstado int not null
);

create table Entrega(
IdEntrega int primary key auto_increment,
DataEntrega date,
ValorFrete decimal(7,2),
DataPrevista date,
 role enum ('Pedido enviado','Produto saiu para entrega', 'Seu Produto Chegou')
);

create table Produto(
CodigoBarras bigint primary key,
 NomeProd varchar(200) not null,
 qtd int,
 Descricao varchar(250),
 ValorUnitario decimal(7,2)
 );
 
 create table Venda (
 IdVenda int primary key auto_increment,
 NomeProd varchar(250) not null,
 ValorTotal decimal(7,2) not null,
 DataVenda Datetime,
 IdClient int,
 NF int,
 IdEntrega int,
 IdFun int
 );
 
 create table VendaProduto(
 valorItem decimal(7,2),
 Qtd int,
 CodigoBarras bigint,
 IdVenda int);
 
 create table NotaFiscal (
 NF int primary key auto_increment,
 TotalNota decimal(7,2),
 DataEmissao date not null
 );
 
 
 -- Criando as chaves primarias
 
 Alter table Endereco
 add Constraint fk_IdBairro_Endereco foreign key (IdBairro) references Bairro(IdBairro),
 add Constraint fk_IdCidade_Endereco foreign key (IdCidade) references Cidade(IdCidade),
 add Constraint fk_IdEstado_Endereco foreign key (IdEstado) references Estado(IDUF);
 
 alter table Cliente add constraint fk_CepCli_Cliente foreign key(CepCli) references Endereco(Cep);
 
 alter table VendaProduto add constraint fk_Codigobarras_Vendaproduto foreign key(CodigoBarras) references Produto(CodigoBarras),
 add constraint fk_IdVenda_Vendaproduto foreign key (IdVenda) references Venda(IdVenda);
 
 alter table Venda 
 add Constraint fk_IdCliente_Venda foreign key (IdClient) references Cliente(IdClient),
 add Constraint fk_NF_Venda foreign key (NF) references NotaFiscal(NF),
 add Constraint fk_IdEntrega_Venda foreign key (IdEntrega) references Entrega(IdEntrega),
 add Constraint fk_IdFun_Venda     foreign key (IdFun)         references    Funcionario(IdFun);
 
 
 
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
 
 delimiter $$
  create  procedure insertEndereco(
  in vCEP int,
  in vLogradouro varchar(200),
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
     insert into Endereco (CEP,Logradouro,IdBairro,IdCidade,IdEstado) values (vCEP,vLogradouro,dBairro,dCidade,dEstado);
      else select("esse Endereço já está registrado!");
   end if ;
  end ;
   $$
   
   delimiter ;
 
 call insertEndereco ('06340250','Rua Do Jão','Lapa','Morro Salgado','sp');
 delimiter $$
 create procedure InsertFuncionario(
 in vNome varchar(250),
 in vEmail varchar(150),
 in vSenha varchar(2250)
 )
 begin
    if not exists (select IdFun from funcionario where Email = vEmail) then
    insert into Funcionario (Nome,Email,Senha)values (vNome,vEmail,vSenha);
    else select("Este Funcionario já está registrado, por favor use outro email não registrado");
    end if ;
 end ;
 $$
 
 call InsertFuncionario ('Daniel','Daniel.Admin@email.com','1234');
 select * from Funcionario ;
 
 
 delimiter $$
 create  procedure insertCliente (
 in vNome varchar(250),
 in vEmail varchar(150),
 in vCPF varchar(12),
 in vSenha varchar(250)
 )
begin
 
   if not exists (select IdClient from cliente where CPF = vCPF ) then
   
     insert into cliente (Nome,Email,CPF,Senha) values (vNome,vEmail,vCPF,vSenha);
     else select("Esse Cliente já está registrado");
     end if;
end ;
$$

call insertCliente("Let0icia","Moto@4.com","43289210825","1234");

delimiter $$
create procedure insertProduto(
in vCodigoBarras bigint,
in vNomeProd varchar(200),
in vQtd int,
in vDescricao varchar(250),
in vValorUnitario decimal(7,2)
)
begin
 if not exists(select CodigoBarras from Produto where CodigoBarras = vCodigoBarras)then
    insert into Produto values(vCodigoBarras,vNomeProd,vQtd,vDescricao,vValorUnitario);
    else select('Produto já está registrado');
    end if ;
end ;
$$
call insertProduto (254932837248,'Pergume Lavuar',25,'Pergume lAVOUAR',12.2)

delimiter $$
create procedure selectFuncionario()
begin
 
select IdFun, Nome,Email,Senha from Funcionario order by Nome; 
end $$

call selectFuncionario;

create  procedure selectCliente()
begin
 select  IdClient,nome,CPF,Email,Senha from cliente order by Nome ; 
end $$

call selectCliente;

create procedure selectProduto()
begin
 select NomeProd,CodigoBarras,Descricao,qtd,ValorUnitario from Produto order by NomeProd ;
end $$

call selectProduto;
create  procedure obterCliente (in vIdClient int)
begin
   select IdClient,Nome,Email,CPF,Senha from Cliente where IdClient = vIdClient;
end $$

create procedure obterFuncionario (in vId int)
begin
  select IdFun,Nome,Email,Senha from funcionario where IdFun = vId;
end $$	

	
create procedure updateFuncionario (
in vIdFun int, in vNome varchar(200), in vEmail varchar(150),in vSenha varchar(250)
)
begin
    update funcionario set Nome = vNome, Email = vEmail, Senha = vSenha  where IdFun = vIdFun;
end $$

create procedure updateCliente (
in vIdClient int, in vNome varchar(200), in vEmail varchar(150),in vSenha varchar(250), in vCPF varchar(11)
)
begin
    update cliente set Nome = vNome, Email = vEmail,CPF = vCPF, Senha = vSenha  where IdClient = vIdClient;
end $$

delimiter $$
create procedure DeleteCliente(in vIdClient int)
begin
  if exists (select IdClient from Cliente where IdClient = vIdClient)then
	 delete from Cliente where IdClient = vIdClient ;
     else select('Não existe este Cliente');
     end if ;
end $$


create procedure DeleteFuncionario(in vIdFun int)
begin
  if exists (select IdFun from Funcionario where IdFun = vIdFun)then
	 delete  from Funcionario  where IdFun = vIdFun ;
     else select('Não existe este Funcionario');
     end if ;
end $$



/*
delimiter $$
create procedure selectEndereco()
begin 
 select CEP,Logradouro from Endereco order by Logradouro;
end$$
delimiter ;
*/
$$
 use dbilumina ;
