FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /src
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 
WORKDIR /app
COPY --from=build-env /app/publish .
COPY --from=build-env /src/wait-for-it.sh .
RUN chmod +x ./*.sh
ENTRYPOINT ["dotnet", "HardwareShop.WebApi.dll"]