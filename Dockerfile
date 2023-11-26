FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "FuncionarioSano.dll"]
