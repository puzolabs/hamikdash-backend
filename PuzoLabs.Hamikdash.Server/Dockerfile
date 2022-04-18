#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["PuzoLabs.Hamikdash.Server/PuzoLabs.Hamikdash.Server.csproj", "PuzoLabs.Hamikdash.Server/"]
RUN dotnet restore "PuzoLabs.Hamikdash.Server/PuzoLabs.Hamikdash.Server.csproj"
COPY . .
WORKDIR "/src/PuzoLabs.Hamikdash.Server"
RUN dotnet build "PuzoLabs.Hamikdash.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PuzoLabs.Hamikdash.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PuzoLabs.Hamikdash.Server.dll"]