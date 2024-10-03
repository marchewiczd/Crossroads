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
COPY ["Crossroads.WebUI/Crossroads.WebUI.csproj", "Crossroads.WebUI/"]
RUN dotnet restore "Crossroads.WebUI/Crossroads.WebUI.csproj"
COPY . .
WORKDIR "/src/Crossroads.WebUI"
RUN dotnet build "Crossroads.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Crossroads.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS container
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crossroads.WebUI.dll"]