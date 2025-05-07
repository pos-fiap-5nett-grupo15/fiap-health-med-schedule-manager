
testes

## Build do projeto
Vá até o diretório da solução e execute o seguinte comando para realizar o build da imagem docker a ser utilizada.

``` shell
    docker build -f ./infrastructure/docker/api/Dockerfile -t schedule-app .
    docker build -f ./infrastructure/docker/worker/Dockerfile -t schedule-app .
```

Ou se estiver na pasta de infra

``` shell
    docker build -f ../fiap-health-med-schedule-manager/infrastructure/docker/api/Dockerfile -t schedule-app ../fiap-health-med-schedule-manager/
    docker build -f ../fiap-health-med-schedule-manager/infrastructure/docker/worker/Dockerfile -t schedule-worker ../fiap-health-med-schedule-manager/
```


## Testar deployment como Minikube

### Instalação
Instale o minikube no WSL seguindo instalação linux no [link](https://minikube.sigs.k8s.io/docs/start/?arch=%2Flinux%2Fx86-64%2Fstable%2Fdebian+package)


### Configuração do Minikube
Passos para Configurar o kubectl com Minikube


#### Inicie o Minikube:

Primeiro, certifique-se de que o Minikube está instalado e inicie o cluster Minikube com o seguinte comando:
```shell
minikube start
```

Para testar se o minikube está na lista de clusters use o seguinte comando:
```shell
kubectl config get-clusters
```

#### Configure o kubectl para Usar o Minikube:

O Minikube automaticamente configura o kubectl para usar o contexto do Minikube quando você inicia o cluster. No entanto, se você precisar configurar manualmente ou verificar a configuração, use o seguinte comando:

```shell
kubectl config use-context minikube
```

#### Verifique a Configuração Atual:

Para verificar se o kubectl está configurado corretamente para usar o Minikube, você pode listar os contextos disponíveis e verificar qual está ativo:

```shell
kubectl config get-contexts
```

O contexto ativo será marcado com um asterisco (*). Certifique-se de que o contexto minikube está ativo.
Teste a Conexão com o Cluster:

Para garantir que o kubectl está se comunicando corretamente com o Minikube, você pode executar um comando simples, como listar os nós do cluster:
```shell
kubectl get nodes
```
Isso deve retornar informações sobre o nó do Minikube.

Outros comandos comumente usados são os seguintes:

```shell
    minikube status
    minikube stop
    minikube start
```

Onde o próprio nome do comando já diz o que é feito.

#### Como adicionar o ACR no minikube

Using a Private Registry
**GCR/ECR/ACR/Docker**: minikube has an addon, registry-creds which maps credentials into minikube to support pulling from Google Container Registry (GCR), Amazon’s EC2 Container Registry (ECR), Azure Container Registry (ACR), and Private Docker registries. You will need to run minikube addons configure registry-creds and minikube addons enable registry-creds to get up and running. An example of this is below:

```shell
$ minikube addons configure registry-creds
Do you want to enable AWS Elastic Container Registry? [y/n]: n

Do you want to enable Google Container Registry? [y/n]: y
-- Enter path to credentials (e.g. /home/user/.config/gcloud/application_default_credentials.json):/home/user/.config/gcloud/application_default_credentials.json

Do you want to enable Docker Registry? [y/n]: n

Do you want to enable Azure Container Registry? [y/n]: n
registry-creds was successfully configured
$ minikube addons enable registry-creds
```
docker login -u minikubetoken -p oNHiUIToqGRbBCDOhHInRJuSJhMpFqB9LtFXA/mijf+ACRDZeNbi fiapacrhackathon.azurecr.io


az ad sp create-for-rbac --name <service-principal-name> --role acrpull --scopes /subscriptions/<subscription-id>/resourceGroups/<resource-group>/providers/Microsoft.ContainerRegistry/registries/<acr-name>

Para criar a secret via kubectl:
```shell
kubectl create secret docker-registry acr-secret \
  --docker-server=fiapacrhackathon.azurecr.io \
  --docker-username=minikubetoken \
  --docker-password=oNHiUIToqGRbBCDOhHInRJuSJhMpFqB9LtFXA/mijf+ACRDZeNbi \
  --namespace=hk
```

agora é só colocar no deployment seguindo o exemplo abaixo:

```yaml
spec:
  containers:
  - name: myapp
    image: <acr-name>.azurecr.io/myapp:latest
  imagePullSecrets:
  - name: acr-secret
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