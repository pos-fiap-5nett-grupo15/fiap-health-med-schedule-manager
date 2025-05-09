FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Fiap.Health.Med.Schedule.Manager.Worker/Fiap.Health.Med.Schedule.Manager.Worker.csproj", "src/Fiap.Health.Med.Schedule.Manager.Worker/"]
COPY ["src/Fiap.Health.Med.Schedule.Manager.Application/Fiap.Health.Med.Schedule.Manager.Application.csproj", "src/Fiap.Health.Med.Schedule.Manager.Application/"]
COPY ["src/Fiap.Health.Med.Schedule.Manager.Domain/Fiap.Health.Med.Schedule.Manager.Domain.csproj", "src/Fiap.Health.Med.Schedule.Manager.Domain/"]
COPY ["src/Fiap.Health.Med.Schedule.Manager.Infrastructure/Fiap.Health.Med.Schedule.Manager.Infrastructure.csproj", "src/Fiap.Health.Med.Schedule.Manager.Infrastructure/"]
RUN dotnet restore "src/Fiap.Health.Med.Schedule.Manager.Worker/Fiap.Health.Med.Schedule.Manager.Worker.csproj"
COPY . .
WORKDIR "/src/src/Fiap.Health.Med.Schedule.Manager.Worker"
RUN dotnet build "./Fiap.Health.Med.Schedule.Manager.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fiap.Health.Med.Schedule.Manager.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
USER root
COPY infrastructure/docker/wait-for-it.sh .
RUN chmod +x wait-for-it.sh
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fiap.Health.Med.Schedule.Manager.Worker.dll"]
