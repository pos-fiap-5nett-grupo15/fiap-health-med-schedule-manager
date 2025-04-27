
## Build do projeto
Vá até o diretório da solução e execute o seguinte comando para realizar o build da imagem docker a ser utilizada.

``` shell
    docker build -f ./infrastructure/docker/Dockerfile -t schedule-app .
```

Ou se estiver na pasta de infra

``` shell
    docker build -f ../fiap-health-med-schedule-manager/src/Fiap.Health.Med.Schedule.Manager.Api/Dockerfile -t schedule-app ../fiap-health-med-schedule-manager/

    docker build -f ../fiap-health-med-schedule-manager/src/Fiap.Health.Med.Schedule.Manager.Worker/Dockerfile -t schedule-worker ../fiap-health-med-schedule-manager/
```


# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)

## Comandos utilizados para criar o projeto:
### Comandos para criar pastas que gosto de utilizar:
```shell
mkdir src
```
```shell
mkdir tests
```
```shell
mkdir docs
```
```shell
mkdir scripts
```

### Comando para gerar arquivo gitignore usando template de .NET:
```shell
dotnet new gitignore
```

### Comando para criar uma nova solution:
```shell
dotnet new sln -n Fiap.Health.Med.Schedule.Manager
```

### Comandos para criar uma nova biblioteca genérica:
```shell
dotnet new classlib -n Fiap.Health.Med.Schedule.Manager.Infrastructure
```
```shell
dotnet new classlib -n Fiap.Health.Med.Schedule.Manager.Application
```
```shell
dotnet new classlib -n Fiap.Health.Med.Schedule.Manager.CrossCutting
```
```shell
dotnet new classlib -n Fiap.Health.Med.Schedule.Manager.Domain
```
```shell
dotnet new classlib -n Fiap.Health.Med.Schedule.Manager.Worker
```

### Comando para criar uma nova biblioteca de testes unitários utilizando o template do xUnit para:
```shell
dotnet new xunit -n Fiap.Health.Med.Schedule.Manager.UnitTests
```

### Comando para criar uma nova biblioteca para API utilizando o template de APIs Rest:
```shell
dotnet new webapi -n Fiap.Health.Med.Schedule.Manager.Api
```

### Comandos para adicionar projetos na solução:
```shell
dotnet sln Fiap.Health.Med.Schedule.Manager.sln add src/Fiap.Health.Med.Schedule.Manager.Api/Fiap.Health.Med.Schedule.Manager.Api.csproj
```
```shell
dotnet sln Fiap.Health.Med.Schedule.Manager.sln add src/Fiap.Health.Med.Schedule.Manager.Application/Fiap.Health.Med.Schedule.Manager.Application.csproj
```
```shell
dotnet sln Fiap.Health.Med.Schedule.Manager.sln add src/Fiap.Health.Med.Schedule.Manager.CrossCutting/Fiap.Health.Med.Schedule.Manager.CrossCutting.csproj
```
```shell
dotnet sln Fiap.Health.Med.Schedule.Manager.sln add src/Fiap.Health.Med.Schedule.Manager.Domain/Fiap.Health.Med.Schedule.Manager.Domain.csproj
```
```shell
dotnet sln Fiap.Health.Med.Schedule.Manager.sln add src/Fiap.Health.Med.Schedule.Manager.Infrastructure/Fiap.Health.Med.Schedule.Manager.Infrastructure.csproj
```
```shell
dotnet sln Fiap.Health.Med.Schedule.Manager.sln add src/Fiap.Health.Med.Schedule.Manager.Worker/Fiap.Health.Med.Schedule.Manager.Worker.csproj
```
```shell
dotnet sln Fiap.Health.Med.Schedule.Manager.sln add tests/Fiap.Health.Med.Schedule.Manager.UnitTests/Fiap.Health.Med.Schedule.Manager.UnitTests.csproj
```