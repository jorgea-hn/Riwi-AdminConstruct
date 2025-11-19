#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["AdminConstruct.Ryzor/AdminConstruct.Ryzor.csproj", "AdminConstruct.Ryzor/"]
#RUN dotnet restore "AdminConstruct.Ryzor/AdminConstruct.Ryzor.csproj"
#COPY . .
#WORKDIR "/src/AdminConstruct.Ryzor"
#RUN dotnet build "./AdminConstruct.Ryzor.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./AdminConstruct.Ryzor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "AdminConstruct.Ryzor.dll"]
# ============================
# 1) BUILD
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /publish

# ============================
# 2) RUNTIME
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "adminConstruct.dll"]
