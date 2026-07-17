# -- Stage 1: Build -------------------------------------------
# Build context is the repository root (see docker-compose.yml), mirroring Flare's Gallery image.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /repo

# Restore dependencies first (layer-cached unless these files change).
# Directory.Build.props must be present so the projects pick up <TargetFramework>.
COPY ["Directory.Build.props", "./"]
COPY ["src/PlaylistShare.Shared/PlaylistShare.Shared.csproj", "src/PlaylistShare.Shared/"]
# Сборки миграций: Api ссылается на них ради MigrationsAssembly, restore без них не пройдёт.
COPY ["src/PlaylistShare.Database/PlaylistShare.Database.csproj", "src/PlaylistShare.Database/"]
COPY ["src/PlaylistShare.Database.SqlServer/PlaylistShare.Database.SqlServer.csproj", "src/PlaylistShare.Database.SqlServer/"]
COPY ["src/PlaylistShare.Database.Postgres/PlaylistShare.Database.Postgres.csproj", "src/PlaylistShare.Database.Postgres/"]
COPY ["src/PlaylistShare.Api/PlaylistShare.Api.csproj", "src/PlaylistShare.Api/"]
RUN dotnet restore "src/PlaylistShare.Api/PlaylistShare.Api.csproj"

# Copy the full source and publish.
COPY . .
RUN dotnet publish "src/PlaylistShare.Api/PlaylistShare.Api.csproj" \
    -c $BUILD_CONFIGURATION -o /publish --no-restore /p:UseAppHost=false -p:MinVerSkip=true

# -- Stage 2: Run ---------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
# Data Protection key ring directory, created up front and owned by the non-root app user so the
# volume mounted here (see docker-compose.prod.yml) is writable. Bake the path in as a default so a
# bare `docker run` also works; compose/VS can still override DataProtection__KeysPath.
RUN mkdir -p /keys && chown $APP_UID /keys
ENV DataProtection__KeysPath=/keys
USER $APP_UID
EXPOSE 8080
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "PlaylistShare.Api.dll"]
