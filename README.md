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
  - Para utilizar a aplicação você necessita estar logado.
  - No primeiro acesso você terá a opção de criar um usuário.
  - Na tela de login digite um usuário e senha e aperte em "Entrar", o sistema irá criar um usuário "admin" para você.

       

