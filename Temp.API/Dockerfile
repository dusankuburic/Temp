FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Temp.API/Temp.API.csproj", "Temp.API/"]
COPY ["Temp.Services/Temp.Services.csproj", "Temp.Services/"]
COPY ["Temp.Domain/Temp.Domain.csproj", "Temp.Domain/"]
COPY ["Temp.Database/Temp.Database.csproj", "Temp.Database/"]


RUN dotnet restore "Temp.API/Temp.API.csproj"
COPY . .
WORKDIR "/src/Temp.API"
RUN dotnet build "Temp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Temp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/certs
COPY Temp.API/certs/aspnetapp.pfx /app/certs

ENTRYPOINT ["dotnet", "Temp.API.dll"]