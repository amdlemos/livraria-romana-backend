# Livraria Romana Backend
Este projeto foi desenvolvido para avaliação da Theos Sistemas quanto a vaga de programador back-end. 
  
## Configurando a Aplicação

### Banco de Dados
- Configure a Connection String em app.settings.json, não se esqueça de fazer a mesma configuração no projeto de testes.
  
```javascript
{  
  "AllowedHosts": "*",
  "ConnectionStrings": {
  "DevConnection": "Password=123;Persist Security Info=True;User ID=sa;Initial Catalog=LivrariaRomana;Data Source=PC_ALAN"
}
```

### Log
- Configure o caminho onde o arquivo será criado no construtor da classe Startup.
 
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
- Para você utilizar a API necessida estar logado.
- Faça um post em Login pelo Swagger com o usuário e senha desejados que o sistema ira criar um usuário "admin" para você. 
- Na resposta vai ser lhe informado que seu usuário foi criado com sucesso.
- Faça um novo post para obter o token, como o abaixo: 
```json
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IlRoaWFnbyIsInN1YiI6IjEzIiwianRpIjoiZDBlMGFkZDItOTlkMC00NWY1LThlYzEtY2FiYzIwZjkxMGYyIiwiaWF0IjoxNTAwMDMzMjE0LCJKd3RWYWxpZGF0aW9uIjoiVXN1YXJpbyIsIm5iZiI6MTUwMDAzMzIxMywiZXhwIjoxNTAwMDMzMjczLCJpc3MiOiJJc3N1ZXIiLCJhdWQiOiJBdWRpZW5jZSJ9.SmjuyXgloA2RUhIlAEetrQwfC0EhBmhu-xOMzyY3Y_Q
```
- Na parte direita do Swagger há um botão Authorize, click e adicione ao campo a palavra Bearer + o token, desta forma você irá adicionar o token ao seu request.
- Agora você já pode utilizar as outras rotas da API.       

