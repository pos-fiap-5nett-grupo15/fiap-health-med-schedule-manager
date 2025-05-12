
# Hackaton - Fiap.Health.Med.Schedule.Manager

Projeto criado pelo **Grupo 15** do curso de **Arquitetura de Sistemas .NET com Azure** da Fiap para atender o Hackaton.

> O Schedule Manager tem como função realizar todos os processos referente ao contexto de agendamentos, sendo um serviço separado e exclusivo para essas funções


## Autores

- Grupo 15

|Integrantes|
|--|
| Caio Vinícius Moura Santos Maia |
| Evandro Prates Silva |
| Guilherme Castro Batista Pereira |
| Luis Gustavo Gonçalves Reimberg |


## Stack utilizada

|Tecnologia utilizada|
|--|
|.Net 8|
|Docker|
|FluentValidation|
|BCypt|
|Swagger|
|XUnit|
|Moq|
|Dapper|
|RabbitMQ|
|FluentMigrator|


## Funcionalidades

- Ações de agendamento do médico(criar, atualizar, aceitar e negar)
- Ações de agendamento do paciente(agendar, cancelar)
- Ações comuns(listar, listar por médico, listar por paciente)


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