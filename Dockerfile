# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy backend project and restore dependencies
COPY ["abdm Backend/*.csproj", "./"]
RUN dotnet restore

# Copy backend source code and publish
COPY ["abdm Backend/", "./"]
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AbdmWrapperNet.dll"]
