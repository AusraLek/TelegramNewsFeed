#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TelegramNewsFeed.QueueProcessor/TelegramNewsFeed.QueueProcessor.csproj", "TelegramNewsFeed.QueueProcessor/"]
COPY ["TelegramNewsFeed.Common/TelegramNewsFeed.Common.csproj", "TelegramNewsFeed.Common/"]
RUN dotnet restore "TelegramNewsFeed.QueueProcessor/TelegramNewsFeed.QueueProcessor.csproj"
COPY . .
WORKDIR "/src/TelegramNewsFeed.QueueProcessor"
RUN dotnet build "TelegramNewsFeed.QueueProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramNewsFeed.QueueProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramNewsFeed.QueueProcessor.dll"]