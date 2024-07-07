FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NexusGPT.WebApplication/NexusGPT.WebApplication.csproj", "NexusGPT.WebApplication/"]
COPY ["NexusGPT.Adapter.Out/NexusGPT.Adapter.Out.csproj", "NexusGPT.Adapter.Out/"]
COPY ["NexusGPT.Entities/NexusGPT.Entities.csproj", "NexusGPT.Entities/"]
COPY ["NexusGPT.SeedWork/NexusGPT.SeedWork.csproj", "NexusGPT.SeedWork/"]
COPY ["NexusGPT.UseCase/NexusGPT.UseCase.csproj", "NexusGPT.UseCase/"]
COPY ["NexusGPT.EventBus/NexusGPT.EventBus.csproj", "NexusGPT.EventBus/"]
COPY ["NexusGPT.MainComponent/NexusGPT.MainComponent.csproj", "NexusGPT.MainComponent/"]
COPY ["NexusGPT.Adapter.Out.ImageStorage.Local/NexusGPT.Adapter.Out.ImageStorage.Local.csproj", "NexusGPT.Adapter.Out.ImageStorage.Local/"]
COPY ["NexusGPT.Adapter.Out.ImageStorage.S3/NexusGPT.Adapter.Out.ImageStorage.S3.csproj", "NexusGPT.Adapter.Out.ImageStorage.S3/"]
RUN dotnet restore "NexusGPT.WebApplication/NexusGPT.WebApplication.csproj"
COPY . .
WORKDIR "/src/NexusGPT.WebApplication"
RUN dotnet build "NexusGPT.WebApplication.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NexusGPT.WebApplication.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p Images
ENTRYPOINT ["dotnet", "NexusGPT.WebApplication.dll"]
