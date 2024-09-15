FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app

# HTTP
EXPOSE 8080 
# HTTPS
EXPOSE 8081 


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Crossroads.Utils/Crossroads.Utils.csproj", "Crossroads.Utils/"]
COPY ["Crossroads.Web/Crossroads.Web.csproj", "Crossroads.Web/"]
RUN dotnet restore "Crossroads.Web/Crossroads.Web.csproj"
COPY . .
WORKDIR "/src/Crossroads.Web"
RUN dotnet build "Crossroads.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Crossroads.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS container
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crossroads.Web.dll"]

