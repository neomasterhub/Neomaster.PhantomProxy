FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Neomaster.PhantomProxy.Api/Neomaster.PhantomProxy.Api.csproj", "Neomaster.PhantomProxy.Api/"]
RUN dotnet restore "Neomaster.PhantomProxy.Api/Neomaster.PhantomProxy.Api.csproj"
COPY . .
RUN dotnet publish "Neomaster.PhantomProxy.Api/Neomaster.PhantomProxy.Api.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE 80
ENTRYPOINT ["dotnet", "Neomaster.PhantomProxy.Api.dll"]
