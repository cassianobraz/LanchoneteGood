FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Lanchonete.Api/Lanchonete.Api.csproj", "src/Lanchonete.Api/"]
RUN dotnet restore "src/Lanchonete.Api/Lanchonete.Api.csproj"

COPY . .
RUN dotnet publish "src/Lanchonete.Api/Lanchonete.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Lanchonete.Api.dll"]