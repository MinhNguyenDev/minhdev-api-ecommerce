dotnet restore src
cd src\Minhdev.Api.Ecommerce
dotnet lambda package --configuration release --framework netcoreapp2.1 --output-package bin/release/netcoreapp2.1/minhdev-api-ecommerce.zip
cd ..\..
