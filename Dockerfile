FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY productService/*.csproj ./productService/
RUN dotnet restore

# copy everything else and build app
COPY productService/. ./productService/
WORKDIR /app/productService
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS runtime
WORKDIR /app

COPY --from=build /app/productService/out ./
EXPOSE 80

ENTRYPOINT ["dotnet", "productService.dll"]