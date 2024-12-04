FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

ENV ASPNETCORE_URLS=http://starwarspeople-container:5002

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["StarWars.People.API/StarWars.People.API.csproj", "StarWars.People.API/"]
RUN dotnet restore "StarWars.People.API/StarWars.People.API.csproj"
COPY . .
WORKDIR "/src/StarWars.People.API"
RUN dotnet build "StarWars.People.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "StarWars.People.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StarWars.People.API.dll"]
