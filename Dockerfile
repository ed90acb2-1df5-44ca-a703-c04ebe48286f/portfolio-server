FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Portfolio.Server/Portfolio.Server.csproj", "Portfolio.Server/"]
RUN dotnet restore "Portfolio.Server/Portfolio.Server.csproj"
COPY . .
WORKDIR "/src/Portfolio.Server"
RUN dotnet build "Portfolio.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Portfolio.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 1447
ENTRYPOINT ["dotnet", "Portfolio.Server.dll"]
