# Dockerfile para AdminConstruct.Web (Admin Panel)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto
COPY ["AdminConstruct.Web/AdminConstruct.Web.csproj", "AdminConstruct.Web/"]
RUN dotnet restore "AdminConstruct.Web/AdminConstruct.Web.csproj"

# Copiar solo el código del proyecto Web
COPY ["AdminConstruct.Web/", "AdminConstruct.Web/"]
WORKDIR "/src/AdminConstruct.Web"
RUN dotnet build "./AdminConstruct.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AdminConstruct.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminConstruct.Web.dll"]
