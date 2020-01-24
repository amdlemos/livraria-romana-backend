# Livraria Romana Backend (ASP.NET CORE 2.2)
Este projeto foi desenvolvido para avaliação da Theos Sistemas quanto a vaga de programador back-end. 

## Pacotes Utilizados
- AutoMapper v9.0.0
- FluentAssertion v5.10.0
- FluentValidation v8.6.1
- NLog.Extensions.Loggin v1.6.1
- Swashbuckle.AspNetCore v5.0.0
- System.IdentityModel.Tokens.Jwt v5.6.0
- Utf8Json v1.3.7
- xunit v2.4.0
- xunit.runner.visualstudio v2.4.0
- coverlet.collector v1.0.1  

## Configurando a Aplicação
### Banco de Dados (SQL SERVER 2017)
#### Criando o banco de dados

- MIGRATION: No console de gerenciador de pacotes, no projeto LivrariaRomana.Infrastructure, voce deve utilizar o comando abaixo:
```bash
update-migration
```
- SCRIPT-SQL: Rodar o script abaixo no banco desejado:
```bash
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Livros] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [OriginalTitle] nvarchar(max) NULL,
    [Author] nvarchar(max) NULL,
    [PublishingCompany] nvarchar(max) NULL,
    [ISBN] nvarchar(max) NULL,
    [PublicationYear] datetime2 NOT NULL,
    [Amount] int NOT NULL,
    CONSTRAINT [PK_Livros] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(50) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Email] nvarchar(30) NOT NULL,
    [Token] nvarchar(max) NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200112230240_Migracao_Inicial', N'2.2.0-rtm-35687');

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'Username');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Usuarios] ALTER COLUMN [Username] nvarchar(max) NULL;

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'Password');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Usuarios] ALTER COLUMN [Password] nvarchar(max) NULL;

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'Email');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Usuarios] ALTER COLUMN [Email] nvarchar(max) NULL;

GO

ALTER TABLE [Usuarios] ADD [Role] nvarchar(max) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200121171819_InclusaoCampoRole', N'2.2.0-rtm-35687');

GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'Token');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Usuarios] DROP COLUMN [Token];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200122211934_CorrecaoDeEntidades', N'2.2.0-rtm-35687');

GO


```

#### Configurar Connection String
- Configure a Connection String em app.settings.json, não se esqueça de fazer a mesma configuração no projeto de testes de repositório. Os testes da API (integração e autorização) utilizam banco de dados na memória.
  
```bash
{  
  "AllowedHosts": "*",
  "ConnectionStrings": {
  "DevConnection": "Password=123;Persist Security Info=True;User ID=sa;Initial Catalog=LivrariaRomana;Data Source=PC_ALAN"
}
```

### Log
- Configure o caminho onde o arquivo será criado em "/nlog.config" e carregue no construtor da classe Startup.
- Adicione o arquivo nlog.config no seguinte caminho: "\LivrariaRomana.API.Tests\bin\Debug\netcoreapp2.2"
 
 ```C#
public Startup(IConfiguration configuration)
{
    // Define onde os logs serão criados.
    LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));            
    _configuration = configuration;
}
```

### Swagger
- O Swagger é inicializado na raiz da aplicação: http://localhost:4726/

## Utilizando a Aplicação
- Para criar o primeiro usuário faça um post em Login pelo Swagger com o usuário e senha desejados que o sistema ira criar um usuário "admin" para você. 
- Na resposta vai ser lhe informado seu token conforme padrão abaixo:
```json
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IlRoaWFnbyIsInN1YiI6IjEzIiwianRpIjoiZDBlMGFkZDItOTlkMC00NWY1LThlYzEtY2FiYzIwZjkxMGYyIiwiaWF0IjoxNTAwMDMzMjE0LCJKd3RWYWxpZGF0aW9uIjoiVXN1YXJpbyIsIm5iZiI6MTUwMDAzMzIxMywiZXhwIjoxNTAwMDMzMjczLCJpc3MiOiJJc3N1ZXIiLCJhdWQiOiJBdWRpZW5jZSJ9.SmjuyXgloA2RUhIlAEetrQwfC0EhBmhu-xOMzyY3Y_Q
```
- Na parte direita do Swagger há um botão Authorize, click e adicione ao campo a palavra Bearer + token, desta forma você irá adicionar o token ao seu request.
- Usuários que não possuam Role "admin" terão as mesmas permissões de usuários não logados.
- Agora você já pode utilizar as outras rotas da API.    

## Observações sobre os testes
- Por algum conflito de namespace, o gerenciador de testes está criando um <Projeto Desconhecido> contendo 3 testes, favor ignora-los.
- Por alguma razão misteriosa ao tentar executar todos os testes através de "Testes>Executar todos os teste"s ou pelo botão no gerenciador de testes, os testes não rodam. Estavam rodando mesmo tendo esse projeto desconhecido. Desta forma, para todar todos os testes de uma só vez, é necessário selecionar os demais projetos de testes (API, Domain e Repositories) e executa-los.
- Implementei uma transaction nos testes de repositório para que o banco esteja sempre vazio antes dos testes. 
- Lembrando, os testes da API utilizam banco de dados na memória.