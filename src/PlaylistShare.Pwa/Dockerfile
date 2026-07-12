# -- Stage 1: Build -------------------------------------------
# Build context is the repository root (see docker-compose.yml), mirroring Flare's Gallery image.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /repo

# Restore dependencies first (layer-cached unless these .csproj files change).
# Directory.Build.props must be present so the projects pick up <TargetFramework>.
COPY ["Directory.Build.props", "./"]
COPY ["src/PlaylistShare.Shared/PlaylistShare.Shared.csproj", "src/PlaylistShare.Shared/"]
COPY ["src/PlaylistShare.Pwa/PlaylistShare.Pwa.csproj", "src/PlaylistShare.Pwa/"]
RUN dotnet restore "src/PlaylistShare.Pwa/PlaylistShare.Pwa.csproj"

# Copy the full source and publish the WebAssembly app.
COPY . .
RUN dotnet publish "src/PlaylistShare.Pwa/PlaylistShare.Pwa.csproj" \
    -c $BUILD_CONFIGURATION -o /publish --no-restore -p:MinVerSkip=true

# -- Stage 2: Serve -------------------------------------------
FROM nginx:alpine AS final
COPY --from=build /publish/wwwroot /usr/share/nginx/html
COPY src/PlaylistShare.Pwa/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
