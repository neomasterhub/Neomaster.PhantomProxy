FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Neomaster.PhantomProxy.Api/Neomaster.PhantomProxy.Api.csproj", "Neomaster.PhantomProxy.Api/"]
RUN dotnet restore "Neomaster.PhantomProxy.Api/Neomaster.PhantomProxy.Api.csproj"
COPY . .
RUN dotnet publish "Neomaster.PhantomProxy.Api/Neomaster.PhantomProxy.Api.csproj" -c Release -o /app/publish
RUN cp /src/Neomaster.PhantomProxy.Api/appsettings.Release.json /app/publish/appsettings.json
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Neomaster.PhantomProxy.Api.dll"]
