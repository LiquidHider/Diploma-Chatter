#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Chatter.Domain.API/Chatter.Domain.API.csproj", "Chatter.Domain.API/"]
COPY ["Chatter.Domain.BusinessLogic/Chatter.Domain.BusinessLogic.csproj", "Chatter.Domain.BusinessLogic/"]
COPY ["Chatter.Domain.DataAccess/Chatter.Domain.DataAccess.csproj", "Chatter.Domain.DataAccess/"]
COPY ["Chatter.Domain.Common/Chatter.Domain.Common.csproj", "Chatter.Domain.Common/"]
RUN dotnet restore "Chatter.Domain.API/Chatter.Domain.API.csproj"
COPY . .
WORKDIR "/src/Chatter.Domain.API"
RUN dotnet build "Chatter.Domain.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chatter.Domain.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chatter.Domain.API.dll"]