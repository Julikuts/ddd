# OpenApi (вызывать из папки DeliveryApp.Api/Adapters/Http/Contract)
```
openapi-generator-cli generate -i https://gitlab.com/microarch-ru/microservices/dotnet/system-design/-/raw/main/services/delivery/contracts/openapi.yml -g aspnetcore -o . --package-name Api --additional-properties classModifier=abstract --additional-properties operationResultTask=true
```
# БД
```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Newtonsoft.Json
dotnet add package Swashbuckle.AspNetCore.Annotations
dotnet add package Swashbuckle.AspNetCore.Newtonsoft
dotnet add package Swashbuckle.AspNetCore.SwaggerUI
```
[Подробнее про dotnet cli](https://learn.microsoft.com/ru-ru/ef/core/cli/dotnet)

# Миграции
```
dotnet ef migrations add Init --startup-project ./DeliveryApp.Api --project ./DeliveryApp.Infrastructure
dotnet ef database update --startup-project ./DeliveryApp.Api --connection "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=delivery;"
```
# Docker
```
docker build .
```