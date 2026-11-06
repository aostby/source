# Kolibri.Kino.KMDb (OMDb's API on the IMDb database). Build from the repository root (the folder with Kolibri.Kino.slnx):
#   docker buildx build --platform linux/arm64 -f src/Kolibri.Kino.KMDb/Dockerfile -t kolibri-kmdb .
# deploy/build-nas-image.ps1 -App kmdb does this and saves the image as a file for the NAS.

# Same pinned SDK as the Kino image (see src/Kolibri.Kino.Blazor/Dockerfile for why it is 10.0.402).
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0.402 AS build
ARG TARGETARCH
WORKDIR /src

COPY src/Kolibri.Kino.Core/Kolibri.Kino.Core.csproj src/Kolibri.Kino.Core/
COPY src/Kolibri.Kino.KMDb/Kolibri.Kino.KMDb.csproj src/Kolibri.Kino.KMDb/
RUN dotnet restore src/Kolibri.Kino.KMDb/Kolibri.Kino.KMDb.csproj -a $TARGETARCH

COPY src/Kolibri.Kino.Core/ src/Kolibri.Kino.Core/
COPY src/Kolibri.Kino.KMDb/ src/Kolibri.Kino.KMDb/
RUN dotnet publish src/Kolibri.Kino.KMDb/Kolibri.Kino.KMDb.csproj -a $TARGETARCH -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# The database connection and the API key come from the environment (deploy/compose.yaml, values in .env).
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER app
ENTRYPOINT ["dotnet", "Kolibri.Kino.KMDb.dll"]
