#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FileMan.Api/FileMan.Api.csproj", "FileMan.Api/"]
COPY ["FileMan.Library.ClamAv/FileMan.Library.ClamAv.csproj", "FileMan.Library.ClamAv/"]
COPY ["FileMan.Library.Gotenberg/FileMan.Library.Gotenberg.csproj", "FileMan.Library.Gotenberg/"]
COPY ["FileMan.Business/FileMan.Business.csproj", "FileMan.Business/"]
COPY ["Common.Observability/Common.Observability.csproj", "Common.Observability/"] 
RUN --mount=type=secret,id=nuget_config_file dotnet restore "FileMan.Api/FileMan.Api.csproj" --configfile "/run/secrets/nuget_config_file"
COPY . .
WORKDIR "/src/FileMan.Api"
RUN dotnet build "FileMan.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileMan.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileMan.Api.dll"]