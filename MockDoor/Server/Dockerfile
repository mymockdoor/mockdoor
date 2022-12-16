#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MockDoor/Abstractions/MockDoor.Abstractions.csproj", "MockDoor/Abstractions/"]
COPY ["MockDoor/Api/MockDoor.Api.csproj", "MockDoor/Api/"]
COPY ["MockDoor/Client/MockDoor.Client.csproj", "MockDoor/Client/"]
COPY ["MockDoor/Data/MockDoor.Data.csproj", "MockDoor/Data/"]
COPY ["MockDoor/Data.Sqlite/Mockdoor.Data.Sqlite.csproj", "MockDoor/Data.Sqlite/"]
COPY ["MockDoor/Data.SqlServer/Mockdoor.Data.SqlServer.csproj", "MockDoor/Data.SqlServer/"]
COPY ["MockDoor/Server/MockDoor.Server.csproj", "MockDoor/Server/"]
COPY ["MockDoor/Services/MockDoor.Services.csproj", "MockDoor/Services/"]
COPY ["MockDoor/Shared/MockDoor.Shared.csproj", "MockDoor/Shared/"]
RUN dotnet restore "MockDoor/Server/MockDoor.Server.csproj"
COPY . .
WORKDIR "/src/MockDoor/Server"
RUN dotnet build "MockDoor.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MockDoor.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MockDoor.Server.dll"]