FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["MockCrmApi.csproj", "./"]
RUN dotnet restore "MockCrmApi.csproj"
COPY . .
RUN dotnet publish "MockCrmApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MockCrmApi.dll"]