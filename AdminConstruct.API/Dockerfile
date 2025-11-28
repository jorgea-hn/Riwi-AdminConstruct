FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto
COPY ["AdminConstruct.API/AdminConstruct.API.csproj", "AdminConstruct.API/"]
COPY ["AdminConstruct.Web/AdminConstruct.Web.csproj", "AdminConstruct.Web/"]
RUN dotnet restore "AdminConstruct.API/AdminConstruct.API.csproj"

# Copiar código fuente de ambos proyectos (API depende de Web)
COPY ["AdminConstruct.API/", "AdminConstruct.API/"]
COPY ["AdminConstruct.Web/", "AdminConstruct.Web/"]

WORKDIR "/src/AdminConstruct.API"
RUN dotnet build "./AdminConstruct.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Remove Web project's appsettings to avoid conflicts during publish
RUN rm -f /src/AdminConstruct.Web/appsettings*.json
RUN dotnet publish "./AdminConstruct.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminConstruct.API.dll"]
