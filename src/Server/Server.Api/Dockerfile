FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Server/Server.Api/Server.Api.csproj", "src/Server/Server.Api/"]
COPY ["src/Server/Server.Domain/Server.Domain.csproj", "src/Server/Server.Domain/"]
COPY ["src/Server/Server.Infrastructure/Server.Infrastructure.csproj", "src/Server/Server.Infrastructure/"]
COPY ["src/Server/Server.Application/Server.Application.csproj", "src/Server/Server.Application/"]
COPY ["src/Common/Common.Messages/Common.Messages.csproj", "src/Common/Common.Messages/"]

RUN dotnet restore "src/Server/Server.Api/Server.Api.csproj"
COPY . .
WORKDIR "/src/src/Server/Server.Api"
RUN dotnet build "Server.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Server.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Server.Api.dll"]
