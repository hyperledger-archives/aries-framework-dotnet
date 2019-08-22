
******************************
Installation and configuration
******************************

Using NuGet
===========

To use the agent framework in your project, add the nuget packages.

If using the package manager:

.. code-block:: bash

    Install-Package AgentFramework.Core -Source https://www.myget.org/F/agent-framework/api/v3/index.json

If using the .NET CLI:

.. code-block:: bash

    dotnet add package AgentFramework.Core -s https://www.myget.org/F/agent-framework/api/v3/index.json
    
Available packages:

- ``AgentFramework.Core`` - core framework package
- ``AgentFramework.AspNetCore`` - simple middleware and service extensions to easily configure and run an agent
- ``AgentFramework.Core.Handlers`` - provides a framework for registering custom message handlers and extending the agent functionality


The framework will be moved to nuget.org soon. For the time being, stable and pre-release packages are available at ``https://www.myget.org/F/agent-framework/api/v3/index.json``.
You can add `nuget.config
<nuget.config>`_ anywhere in your project path with the myget.org repo.

.. code-block:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
        <packageSources>
            <add key="myget.org" value="https://www.myget.org/F/agent-framework/api/v3/index.json" />
            <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
        </packageSources>
    </configuration>

Setting up development environment
==================================

Agent Framework uses Indy SDK wrapper for .NET which requires platform specific native libraries of libindy to be available in the running environment.
Check the `Indy SDK project page
<https://github.com/hyperledger/indy-sdk>`_ for details on installing libindy for different platforms or read the brief instructions below.

Make sure you have `.NET Core SDK
<https://dotnet.microsoft.com/download>`_ installed for your platform.

Windows
-------

You can download binaries of libindy and all dependencies from the `Sovrin repo
<https://repo.sovrin.org/windows/libindy/>`_. The dependencies are under ``deps`` folder and ``libindy`` under one of streams (rc, master, stable). There are two options to link the DLLs

- Unzip all files in a directory and add that to your PATH variable (recommended for development)
- Or copy all DLL files in the publish directory (recommended for published deployments)

More details at the `Indy documentation for setting up Windows environment
<https://github.com/hyperledger/indy-sdk/blob/master/docs/build-guides/windows-build.md>`_.

MacOS
-----

Check `Setup Indy SDK build environment for MacOS
<https://github.com/hyperledger/indy-sdk/blob/master/docs/build-guides/mac-build.md>`_.

Copy ``libindy.a`` and ``libindy.dylib`` to the ``/usr/local/lib/`` directory.

Linux
-----

Build instructions for `Ubuntu based distros
<https://github.com/hyperledger/indy-sdk/blob/master/docs/build-guides/ubuntu-build.md>`_ and `RHEL based distros
<https://github.com/hyperledger/indy-sdk/blob/master/doc/build-guides/rhel-build.md>`_.