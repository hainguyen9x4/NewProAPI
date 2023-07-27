#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ProAPI_ScanGet/Pro.APIScanGet.csproj", "ProAPI_ScanGet/"]
COPY ["Pro.Commons/Pro.Common/Pro.Common.csproj", "Pro.Commons/Pro.Common/"]
COPY ["Pro.Commons/Pro.FileManager/Pro.FileManager.csproj", "Pro.Commons/Pro.FileManager/"]
COPY ["Pro.Model/Pro.Model/Pro.Model.csproj", "Pro.Model/Pro.Model/"]
COPY ["Pro.Service/Pro.Service/Pro.Service.csproj", "Pro.Service/Pro.Service/"]
COPY ["Pro.Data/Pro.Data/Pro.Data.csproj", "Pro.Data/Pro.Data/"]
RUN dotnet restore "ProAPI_ScanGet/Pro.APIScanGet.csproj"
COPY . .
WORKDIR "/src/ProAPI_ScanGet"
RUN dotnet build "Pro.APIScanGet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pro.APIScanGet.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pro.APIScanGet.dll"]