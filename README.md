# Livraria Romana Backend (ASP.NET CORE 2.2)
Este projeto foi desenvolvido para avaliação da Theos Sistemas quanto a vaga de programador back-end. 

Obs: O front-end está sendo desenvolvimento em Angular, no entanto preciso melhorar a usabilidade do mesmo. O site já pode consumir a API porém eu ainda não tratei as respostas de erro e não validei as autorizações entre algumas outras coisas. 

## Pacotes Utilizados
- AutoMapper v9.0.0
- FluentAssertion v5.10.0
- FluentValidation v8.6.1
- NLog.Extensions.Loggin v1.6.1
- Swashbuckle.AspNetCore v5.0.0
- System.IdentityModel.Tokens.Jwt v5.6.0
- Utf8Json v1.3.7
- Konsciou.Security.Cryptography.Argon2
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

CREATE TABLE [Books] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [OriginalTitle] nvarchar(max) NULL,
    [Author] nvarchar(max) NULL,
    [PublishingCompany] nvarchar(max) NULL,
    [ISBN] nvarchar(max) NULL,
    [PublicationYear] nvarchar(max) NULL,
    [Amount] int NOT NULL,
    CONSTRAINT [PK_Books] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    [Hash] nvarchar(max) NULL,
    [Salt] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Role] nvarchar(max) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200126171823_create_database', N'2.2.0-rtm-35687');

GO
```

#### Configurar Connection String
- Configure a Connection String em app.settings.json, não se esqueça de fazer a mesma configuração no projeto de testes de repositório e serviços. Os testes da API (integração e autorização) utilizam banco de dados na memória.
  
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
 
 ```c#
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
- Definir LivrariaRomana.API como projeto de inicialzação.
- Para criar o primeiro usuário faça um post em Login pelo Swagger com o usuário e senha desejados que o sistema ira criar um usuário "admin" para você. 
- Na resposta vai ser lhe informado seu token conforme padrão abaixo:

```bash
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IlRoaWFnbyIsInN1YiI6IjEzIiwianRpIjoiZDBlMGFkZDItOTlkMC00NWY1LThlYzEtY2FiYzIwZjkxMGYyIiwiaWF0IjoxNTAwMDMzMjE0LCJKd3RWYWxpZGF0aW9uIjoiVXN1YXJpbyIsIm5iZiI6MTUwMDAzMzIxMywiZXhwIjoxNTAwMDMzMjczLCJpc3MiOiJJc3N1ZXIiLCJhdWQiOiJBdWRpZW5jZSJ9.SmjuyXgloA2RUhIlAEetrQwfC0EhBmhu-xOMzyY3Y_Q
```

- Na parte direita do Swagger há um botão Authorize, click e adicione ao campo a palavra Bearer + token, desta forma você irá adicionar o token ao seu request.
- Usuários que não possuam Role "admin" terão as mesmas permissões de usuários não logados.
- Agora você já pode utilizar as outras rotas da API.   

## Controlando o estoque
- Primeiramente você deve adicionar um livro no banco.
- Após ter sido feita a inclusão você irá usar o BookStockController para consumir o serviço que atualiza o estoque, não é possível alterar a quantidade de livros diretamente pela edição de livros. 


## Rodando os testes
- Não se esquecer de copiar o arquivo "/nlog.config" para o projeto de testes da API.
- Implementei uma transaction nos testes de repositório e serviços para que o banco esteja sempre vazio antes dos testes. 
- Lembrando, os testes da API utilizam banco de dados na memória.
