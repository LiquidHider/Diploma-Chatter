#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Chatter.Email.MessageReciever/Chatter.Email.MessageReciever.csproj", "Chatter.Email.MessageReciever/"]
COPY ["Chatter.Email.Core/Chatter.Email.Core.csproj", "Chatter.Email.Core/"]
COPY ["Chatter.Email.Common/Chatter.Email.Common.csproj", "Chatter.Email.Common/"]
RUN dotnet restore "Chatter.Email.MessageReciever/Chatter.Email.MessageReciever.csproj"
COPY . .
WORKDIR "/src/Chatter.Email.MessageReciever"
RUN dotnet build "Chatter.Email.MessageReciever.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chatter.Email.MessageReciever.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["Chatter.Email.MessageReciever/appsettings.json","/app/"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chatter.Email.MessageReciever.dll"]