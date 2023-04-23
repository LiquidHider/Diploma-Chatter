#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Chatter.Security.API/Chatter.Security.API.csproj", "Chatter.Security.API/"]
COPY ["Chatter.Security.BusinessLogic/Chatter.Security.Core.csproj", "Chatter.Security.BusinessLogic/"]
COPY ["Chatter.Security.DataAccess/Chatter.Security.DataAccess.csproj", "Chatter.Security.DataAccess/"]
COPY ["Chatter.Security.Common/Chatter.Security.Common.csproj", "Chatter.Security.Common/"]

RUN dotnet restore "Chatter.Security.API/Chatter.Security.API.csproj"
COPY . .
WORKDIR "/src/Chatter.Security.API"
RUN dotnet build "Chatter.Security.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chatter.Security.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["Chatter.Security.API/EmailMessages/welcome.html","EmailMessages/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chatter.Security.API.dll"]