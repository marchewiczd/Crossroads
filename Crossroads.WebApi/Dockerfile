FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Crossroads.Utils/Crossroads.Utils.csproj", "Crossroads.Utils/"]
COPY ["Crossroads.WebApi/Crossroads.WebApi.csproj", "Crossroads.WebApi/"]
RUN dotnet restore "Crossroads.WebApi/Crossroads.WebApi.csproj"
COPY . .
WORKDIR "/src/Crossroads.WebApi"
RUN dotnet build "Crossroads.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Crossroads.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS container
WORKDIR /app
# get CURL
RUN apt-get update && apt-get install curl -y
# get docker installation script
RUN curl -fsSL https://get.docker.com -o get-docker.sh
# run the script
RUN sh get-docker.sh
# clean redundant packages
RUN apt-get clean
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crossroads.WebApi.dll"]
