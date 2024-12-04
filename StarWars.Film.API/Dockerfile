FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5003

ENV ASPNETCORE_URLS=http://starwarsfilm-container:5003

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["StarWars.Film.API/StarWars.Film.API.csproj", "StarWars.Film.API/"]
RUN dotnet restore "StarWars.Film.API/StarWars.Film.API.csproj"
COPY . .
WORKDIR "/src/StarWars.Film.API"
RUN dotnet build "StarWars.Film.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "StarWars.Film.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StarWars.Film.API.dll"]
