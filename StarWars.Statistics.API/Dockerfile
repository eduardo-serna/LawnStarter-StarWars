FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5005

ENV ASPNETCORE_URLS=http://starwarsStatistics-container:5005

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["StarWars.Statistics.API/StarWars.Statistics.API.csproj", "StarWars.Statistics.API/"]
RUN dotnet restore "StarWars.Statistics.API/StarWars.Statistics.API.csproj"
COPY . .
WORKDIR "/src/StarWars.Statistics.API"
RUN dotnet build "StarWars.Statistics.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "StarWars.Statistics.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StarWars.Statistics.API.dll"]
