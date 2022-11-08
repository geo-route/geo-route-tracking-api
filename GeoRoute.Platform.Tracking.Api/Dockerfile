FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /build

COPY . .
RUN dotnet restore -r linux-x64 GeoRoute.Platform.Tracking.Api/GeoRoute.Platform.Tracking.Api.csproj
RUN dotnet publish -c Release -o /build/binaries -r linux-x64 --no-restore GeoRoute.Platform.Tracking.Api/GeoRoute.Platform.Tracking.Api.csproj

FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY --from=build-env /build/binaries /app
COPY GeoRoute.Platform.Tracking.Api/appsettings.json /app/appsettings.json

EXPOSE 80
EXPOSE 81
EXPOSE 8080
WORKDIR /app
ENTRYPOINT ["/app/tracking-api"]
