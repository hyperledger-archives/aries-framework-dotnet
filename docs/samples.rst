*******
Samples
*******

ASP.NET Core Agents
===================

A sample agent running in ASP.NET Core that runs the default agent middleware can be found in `samples/aspnetcore
<https://github.com/streetcred-id/agent-framework/tree/master/samples/aspnetcore>`_. This agent is also used in the Docker sample.

.. code-block:: bash

    dotnet run --project samples/aspnetcore/WebAgent.csproj

Running multiple instances
--------------------------

To run multiple agent instances that can communicate, you can specify the binding address and port by setting the ``ASPNETCORE_URLS`` environment variable

.. code-block:: bash

    # Unix/Mac:
    ASPNETCORE_URLS="http://localhost:5001" dotnet run --no-launch-profile --project samples/aspnetcore/WebAgent.csproj

    # Windows PowerShell:
    $env:ASPNETCORE_URLS="http://localhost:5001" ; dotnet run --no-launch-profile --project samples/aspnetcore/WebAgent.csproj

    # Windows CMD (note: no quotes):
    SET ASPNETCORE_URLS=http://localhost:5001 && dotnet run --no-launch-profile --project samples/aspnetcore/WebAgent.csproj

.. note:: The sample web agent doesn't use any functionality that requires a local indy node, but if you'd like to 
    extend the sample and test interaction with the ledger, you can run a local node using the instructions below.

Run a local Indy node with Docker
---------------------------------

The repo contains a docker image that can be used to run a local pool with 4 nodes.

.. code-block:: bash

    docker build -f docker/indy-pool.dockerfile -t indy_pool .
    docker run -itd -p 9701-9709:9701-9709 indy_pool

Mobile Agent with Xamarin Forms
===============================



Docker container example
========================

Running the example
-------------------

At the root of the repo run:

.. code-block:: bash

    docker-compose up

This will create an agent network with a pool and two identical agents able to communicate with each other in the network.
Navigate to http://localhost:7000/ and http://localhost:8000/ to create and accept connection invitations between the different agents.

Running the unit tests
----------------------

.. code-block:: bash

    docker-compose -f docker-compose.test.yaml up --build --remove-orphans --abort-on-container-exit --exit-code-from test-agent

Note: You may need to cleanup previous docker network created using `docker network prune`
