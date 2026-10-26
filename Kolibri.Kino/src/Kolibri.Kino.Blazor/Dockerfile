# Kolibri Kino web app. Build from the repository root (the folder with Kolibri.Kino.slnx):
#   docker buildx build --platform linux/arm64 -f src/Kolibri.Kino.Blazor/Dockerfile -t kolibri-kino .
# deploy/build-nas-image.ps1 does this and saves the image as a file for the NAS.

# The SDK runs on the build machine's own CPU and cross-compiles for the target (arm64 for the UGREEN NAS),
# so there's no slow emulation.
# Pinned to 10.0.402: in the buildx builder, sdk:10.0 (10.0.403, 2026-10-22) unpacks with empty files (dotnet is 0 bytes),
# so restore/publish "succeed" in a second without doing anything and COPY --from=build /app fails. Go back to sdk:10.0
# when a newer image builds again (the runtime image below is fine).
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0.402 AS build
ARG TARGETARCH
WORKDIR /src

# Project files first, so the package restore is cached until a project file changes.
COPY src/Kolibri.Kino.Core/Kolibri.Kino.Core.csproj src/Kolibri.Kino.Core/
COPY src/Kolibri.Kino.Data/Kolibri.Kino.Data.csproj src/Kolibri.Kino.Data/
COPY src/Kolibri.Kino.Controllers/Kolibri.Kino.Controllers.csproj src/Kolibri.Kino.Controllers/
COPY src/Kolibri.Kino.Blazor/Kolibri.Kino.Blazor.csproj src/Kolibri.Kino.Blazor/
RUN dotnet restore src/Kolibri.Kino.Blazor/Kolibri.Kino.Blazor.csproj -a $TARGETARCH

COPY src/Kolibri.Kino.Core/ src/Kolibri.Kino.Core/
COPY src/Kolibri.Kino.Data/ src/Kolibri.Kino.Data/
COPY src/Kolibri.Kino.Controllers/ src/Kolibri.Kino.Controllers/
COPY src/Kolibri.Kino.Blazor/ src/Kolibri.Kino.Blazor/
RUN dotnet publish src/Kolibri.Kino.Blazor/Kolibri.Kino.Blazor.csproj -a $TARGETARCH -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# The library (SilverScreen.db + SilverScreen.imgdb) is mounted at /data; see deploy/compose.yaml.
ENV ASPNETCORE_HTTP_PORTS=8080 \
    Kino__LiteDbPath=/data/SilverScreen.db \
    DataProtectionKeysPath=/data/keys
VOLUME /data
EXPOSE 8080

# Runs as the image's non-root "app" user unless compose.yaml sets "user" (to the NAS user owning /data).
USER app
ENTRYPOINT ["dotnet", "Kolibri.Kino.Blazor.dll"]
