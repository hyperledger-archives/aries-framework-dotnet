# Agent Framework

## .NET Core library for building Sovrin agents

[![Build Status](https://dev.azure.com/streetcred/Agent%20Framework/_apis/build/status/Agent%20Framework%20-%20Build?branchName=master)](https://dev.azure.com/streetcred/Agent%20Framework/_build/latest?definitionId=10?branchName=master)
[![Build Status](https://travis-ci.com/streetcred-id/agent-framework.svg?branch=master)](https://travis-ci.com/streetcred-id/agent-framework)
[![MyGet](https://img.shields.io/myget/agent-framework/v/AgentFramework.Core.svg)](https://www.myget.org/feed/agent-framework/package/nuget/AgentFramework.Core)

Agent Framework is a .NET Core library for building Sovrin interoperable agent services.
It is an abstraction on top of Indy SDK that provides a set of API's for managing agent workflows.
The framework runs .NET Standard (2.0+), including ASP.NET Core and Xamarin.

### Documentation

- [Installation and configuration](https://agent-framework.readthedocs.io/en/latest/installation.html)
- [Agent Workflows](https://agent-framework.readthedocs.io/en/latest/quickstart.html)
- [Mobile Agents with Xamarin](https://agent-framework.readthedocs.io/en/latest/xamarin.html)
- [Web Agent services with ASP.NET Core](https://agent-framework.readthedocs.io/en/latest/aspnetcore.html)
- [Hosting agents in docker containers](https://agent-framework.readthedocs.io/en/latest/docker.html)
- [Samples and demos](https://agent-framework.readthedocs.io/en/latest/samples.html)

### A very quick demo

With [Docker](https://www.docker.com) installed, run

```lang=bash
docker-compose up
```

This will create an agent network with a pool of 4 indy nodes and 2 agents able to communicate with each other in the network.
Navigate to [http://localhost:7000](http://localhost:7000) and [http://localhost:8000](http://localhost:8000) to create and accept connection invitations between the different agents.