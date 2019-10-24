FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app/dotnetapp

COPY ./productService/. ./
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /out ./
EXPOSE 5000
ENTRYPOINT ["dotnet", "productService.dll"]
