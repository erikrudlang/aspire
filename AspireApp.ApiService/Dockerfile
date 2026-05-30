# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY ["AspireApp.sln", "./"]
COPY ["AspireApp.ApiService/AspireApp.ApiService.csproj", "AspireApp.ApiService/"]
COPY ["AspireApp.ServiceDefaults/AspireApp.ServiceDefaults.csproj", "AspireApp.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "AspireApp.ApiService/AspireApp.ApiService.csproj"

# Copy the rest of the source code
COPY ["AspireApp.ApiService/", "AspireApp.ApiService/"]
COPY ["AspireApp.ServiceDefaults/", "AspireApp.ServiceDefaults/"]

# Build the application
WORKDIR "/src/AspireApp.ApiService"
RUN dotnet build "AspireApp.ApiService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AspireApp.ApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspireApp.ApiService.dll"]
