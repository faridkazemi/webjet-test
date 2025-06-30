
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.Config", "."]

COPY ["Webjet-Movies-Backend.sln", "."]
COPY ["CinemaWorldDataSync/CinemaWorldDataSync.csproj", "CinemaWorldDataSync/"]
COPY ["RedisLibrary/RedisLibrary.csproj", "RedisLibrary/"]
COPY ["RedisLibrary.Test/RedisLibrary.Test.csproj", "RedisLibrary.Test/"]
COPY ["CinemaWorldDataSync.Test/CinemaWorldDataSync.Test.csproj", "CinemaWorldDataSync.Test/"]
COPY ["Webjet-Movies-Backend/Webjet-Movies-Backend.csproj", "Webjet-Movies-Backend/"]
RUN dotnet restore "Webjet-Movies-Backend.sln"

COPY ["CinemaWorldDataSync/", "CinemaWorldDataSync/"]
COPY ["RedisLibrary/", "RedisLibrary/"]
COPY ["Webjet-Movies-Backend/", "Webjet-Movies-Backend/"]

WORKDIR "/src/CinemaWorldDataSync"
RUN dotnet build "CinemaWorldDataSync.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CinemaWorldDataSync.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CinemaWorldDataSync.dll"]