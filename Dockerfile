FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./

RUN dotnet restore ./FileGate.Api/FileGate.Api.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish ./FileGate.Api/FileGate.Api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FileGate.Api.dll"]