FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/published-app
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
ENTRYPOINT ["dotnet", "/app/UserServiceAPI.dll"]