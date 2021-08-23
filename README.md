# Aries Framework for .NET

[![Build Status](https://dev.azure.com/Hyperledger/Aries/_apis/build/status/hyperledger.aries-framework-dotnet?branchName=master)](https://dev.azure.com/Hyperledger/Aries/_build/latest?definitionId=80&branchName=master)
[![Build Status](https://travis-ci.com/hyperledger/aries-framework-dotnet.svg?branch=master)](https://travis-ci.com/hyperledger/aries-framework-dotnet.svg?branch=master)
[![MyGet](https://img.shields.io/nuget/v/Hyperledger.Aries.svg)](https://www.nuget.org/packages/Hyperledger.Aries/)

Aries Framework for .NET is a comprehensive implementation of the Aries protocols. It's purpose is to provide a universal library for building SSI application for the cloud, mobile and IoT stack.

## Table of Contents <!-- omit in toc -->

- [Environment Setup](#environment-setup)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Quickstart Guide](#quickstart-Guide)
  - [Create new web application](#create-new-web-application)
  - [Add the framework dependencies](#add-the-framework-dependencies)
  - [Register the agent middleware](#register-the-agent-middleware)
- [Demo](#demo)
- [License](#license)

## Environment Setup

### Prerequisites

- Install [.NET Core](https://dotnet.microsoft.com/download)
- Install [libindy for your platform](https://github.com/hyperledger/indy-sdk/#installing-the-sdk)

### Installation

Aries Framework for .NET comes as a Nuget package available at [nuget.org](https://www.nuget.org/packages/AgentFramework/)

```bash
PM> Install-Package Hyperledger.Aries
```

If you are developing a web application, also install [Hyperledger.Aries.AspNetCore](https://www.nuget.org/packages/Hyperledger.Aries.AspNetCore/) package.

## Quickstart Guide

The framework fully leverages the [.NET Core hosting model](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0) with full integration of dependency injection, configuration and hosting services.

### Create new web application

Using your favorite editor, create new web project. You can also create a project from the console.

```bash
dotnet new web -o AriesAgent
```

To setup your agent use the `Startup.cs` file to configure the framework.

### Add the framework dependencies

Use the `IServiceCollection` extensions to add the dependent services to your application in the `ConfigureServices(IServiceCollection services)` method. Upon startup, the framework will create and configure your agent.

```c#
services.AddAriesFramework(builder =>
{
    builder.RegisterAgent(options =>
    {
        options.EndpointUri = "http://localhost:5000/";
    });
});
```

> Note: If you'd like your agent to be accessible publically, use Ngrok to setup a public host and use that as the `EndpointUri`.
> When changing the endpoints, make sure you clear any previous wallets with the old configuration. Wallet data files are located in `~/.indy_client/wallet`

For a list of all configuration options, check the [AgentOptions.cs](https://github.com/hyperledger/aries-framework-dotnet/blob/master/src/Hyperledger.Aries/Configuration/AgentOptions.cs) file.

### Register the agent middleware

When running web applications, register the agent middleware in the `Configure(IApplicationBuilder app, IWebHostEnvironment env)` method. This will setup a middleware in the AspNetCore pipeline that will respond to incoming agent messages.

```c#
app.UseAriesFramework();
```

That's it. Run your project.

## Demo

With [Docker](https://www.docker.com) installed, run

```lang=bash
docker-compose up
```

This will create an agent network with a pool of 4 indy nodes and 2 agents able to communicate with each other in the network.
Navigate to [http://localhost:7000](http://localhost:7000) and [http://localhost:8000](http://localhost:8000) to create and accept connection invitations between the different agents.


## Testing

To run the unit tests, the following dependencies also must be installed: 
- Docker

### Install libindy library
Follow the build instructions for your OS on the [Hyperledger Indy SDK](https://github.com/hyperledger/indy-sdk) Readme. 

For macOS, if you get a `'indy' DLL not found exception`, move the built `libindy.dylib` file to the `test/Hyperledger.Aries.Tests/bin/Debug/netcoreapp3.1/` directory to explicitly add it to the path. 


### Run an indy node pool on localhost
```
docker build --build-arg pool_ip=127.0.0.1 -f docker/indy-pool.dockerfile -t indy_pool docker/
docker run -itd -p 9701-9709:9701-9709 indy_pool
```

### Run an indy node pool on server
```
# replace <ip_address> with server IP address
docker build --build-arg pool_ip=<ip_address> -f docker/indy-pool.dockerfile -t indy_pool docker/
docker run -itd -p <ip_address>:9701-9709:9701-9709 indy_pool
```

### Run the tests
First, edit the keyword in the `scripts/tester.sh` file to select the tests you want to run. Then, run the script
```
scripts/tester.sh 
```

## License

[Apache License Version 2.0](https://github.com/hyperledger/aries-cloudagent-python/blob/master/LICENSE)
