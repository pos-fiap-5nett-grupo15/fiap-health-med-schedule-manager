FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Fiap.Health.Med.Schedule.Manager.Api/Fiap.Health.Med.Schedule.Manager.Api.csproj", "src/Fiap.Health.Med.Schedule.Manager.Api/"]
COPY ["src/Fiap.Health.Med.Schedule.Manager.Domain/Fiap.Health.Med.Schedule.Manager.Domain.csproj", "src/Fiap.Health.Med.Schedule.Manager.Domain/"]
COPY ["src/Fiap.Health.Med.Schedule.Manager.CrossCutting/Fiap.Health.Med.Schedule.Manager.CrossCutting.csproj", "src/Fiap.Health.Med.Schedule.Manager.CrossCutting/"]
COPY ["src/Fiap.Health.Med.Schedule.Manager.Infrastructure/Fiap.Health.Med.Schedule.Manager.Infrastructure.csproj", "src/Fiap.Health.Med.Schedule.Manager.Infrastructure/"]
COPY ["src/Fiap.Health.Med.Schedule.Manager.Application/Fiap.Health.Med.Schedule.Manager.Application.csproj", "src/Fiap.Health.Med.Schedule.Manager.Application/"]
RUN dotnet restore "src/Fiap.Health.Med.Schedule.Manager.Api/Fiap.Health.Med.Schedule.Manager.Api.csproj"
COPY . .
WORKDIR "/src/src/Fiap.Health.Med.Schedule.Manager.Api"
RUN dotnet build "Fiap.Health.Med.Schedule.Manager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Fiap.Health.Med.Schedule.Manager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
USER root
COPY src/wait-for-it.sh .
RUN chmod +x wait-for-it.sh
USER $APP_UID

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fiap.Health.Med.Schedule.Manager.Api.dll"]
