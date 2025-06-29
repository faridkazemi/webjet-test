#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
#EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Webjet-Movies-Backend/Webjet-Movies-Backend.csproj", "Webjet-Movies-Backend/"]
COPY ["RedisLibrary/RedisLibrary.csproj", "RedisLibrary/"]
RUN dotnet restore "./Webjet-Movies-Backend/Webjet-Movies-Backend.csproj"
COPY . .
WORKDIR "/src/Webjet-Movies-Backend"
RUN dotnet build "./Webjet-Movies-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Webjet-Movies-Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Webjet-Movies-Backend.dll"]