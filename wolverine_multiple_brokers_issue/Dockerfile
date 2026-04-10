FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["wolverine_multiple_brokers_issue/wolverine_multiple_brokers_issue.csproj", "wolverine_multiple_brokers_issue/"]
RUN dotnet restore "wolverine_multiple_brokers_issue/wolverine_multiple_brokers_issue.csproj"
COPY . .
WORKDIR "/src/wolverine_multiple_brokers_issue"
RUN dotnet build "./wolverine_multiple_brokers_issue.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./wolverine_multiple_brokers_issue.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "wolverine_multiple_brokers_issue.dll"]
