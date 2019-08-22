********************************
Agent services with ASP.NET Core
********************************

Installation
============

A package with extensions and default implementations for use with ASP.NET Core is available.

Configure required services
===========================

Inside your ``Startup.cs`` fine in ``ConfigureServices(IServiceCollection services)`` use the extension methods to add all dependent services and optionally pass configuration data.

.. code-block:: csharp
    :emphasize-lines: 4

    public void ConfigureServices(IServiceCollection services)
    {
        // other configuration
        services.AddAgent();
    }

Configure options manually
--------------------------

You can customize the wallet and pool configuration options using

.. code-block:: csharp

    services.AddAgent(config =>
    {
        config.SetPoolOptions(new PoolOptions { GenesisFilename = Path.GetFullPath("pool_genesis.txn") });
        config.SetWalletOptions(new WalletOptions
        {
            WalletConfiguration = new WalletConfiguration { Id = "MyAgentWallet" },
            WalletCredentials = new WalletCredentials { Key = "SecretWalletEncryptionKeyPhrase" }
        });
    });

Use options pattern
-------------------

Alternatively, options be configured using APS.NET Core ``IOptions<T>`` pattern.

.. code-block:: csharp

    services.Configure<PoolOptions>(Configuration.GetSection("PoolOptions"));
    services.Configure<WalletOptions>(Configuration.GetSection("WalletOptions"));

Set any fields you'd like to configure in your ``appsettings.json``.

.. code-block:: xml

    {
        // config options
        "WalletOptions": {
            "WalletConfiguration": { 
                "Id": "MyAgentWallet",
                "StorageConfiguration": { "Path": "[path to wallet storage]" }
            },
            "WalletCredentials": { "Key": "SecretWalletEncryptionKeyPhrase" }
        },
        "PoolOptions": {
            "GenesisFilename": "[path to genesis file]",
            "PoolName": "DefaultPool",
            "ProtocolVersion": 2
        }
    }

Initialize agent middleware
===========================

In ``Configure(IApplicationBuilder app, IHostingEnvironment env)`` start the default agent middleware

.. code-block:: csharp
    :emphasize-lines: 4

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        // Endpoint can be any address you'd like to bind to this middleware
        app.UseAgent("http://localhost:5000/agent"); 

        // .. other services like app.UseMvc()
    }

The default agent middleware is a simple implementation. You can `create your middleware
<https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-2.2>`_ and use that instead if you'd like to customize the message handling.

.. code-block:: csharp

    app.UseAgent<CustomAgentMiddlware>("http://localhost:5000/agent");

See `AgentMiddleware.cs
<https://github.com/streetcred-id/agent-framework/blob/master/src/AgentFramework.AspNetCore/Middleware/AgentMiddleware.cs>`_ for example implementation.

.. tip:: In ASP.NET Core, the order of middleware registration is important, so you might want to add the agent middleware before any other middlewares, like MVC.

Calling services from controllers
=================================

Use dependency injection to get a reference to each service in your controllers.

.. code-block:: csharp

    public class HomeController : Controller
    {
        private readonly IConnectionService _connectionService;
        private readonly IWalletService _walletService;
        private readonly WalletOptions _walletOptions;

        public HomeController(
            IConnectionService connectionService, 
            IWalletService walletService,
            IOptions<WalletOptions> walletOptions)
        {
            _connectionService = connectionService;
            _walletService = walletService;
            _walletOptions = walletOptions.Value;
        }

        // ...
    }