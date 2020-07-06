# How to add Hyperledger Aries OpenApi

## Add the Hyperledger.Aries.AspNetCore Nuget

```console
dotnet add package Hyperledger.Aries.AspNetCore
```

## Update Startup ConfigureServices

Aries OpenApi uses the FluentValidation nuget package to validate all requests sent to the endpoints.

FluentValidation must be configured prior to calling AddAriesOpenApi.

> Don't worry an exception will occur if you forget.

Example:

```csharp
services
  .AddMvc()
  .AddFluentValidation()
  .AddAriesOpenApi(a => a.UseSwaggerUi = true);
```

## Update Startup Configure

Configuring the pipeline to use the Aries Open Api is easily acomplished with a single exetension method.

```csharp
app.UseAriesOpenApi();
```
