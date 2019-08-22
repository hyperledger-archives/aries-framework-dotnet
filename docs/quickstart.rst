******************************
Configuration and provisioning
******************************

Services overview
=================

- ``IProvisioningService`` - used to provision new agents and access the provisioning configuration that contains endpoint data, ownerhip info, service endpoints, etc.
- ``IConnectionService`` - manage connection records, create and accept invitations
- ``ICredentialService`` - manage credential records, create offer, issue, revoke and store credentials
- ``IProofService`` - send proof requests, provide and verify proofs
- ``IWalletRecordService`` - utility service used to manage custom application records that are stored in the wallet
- ``ISchemaService`` - create and manage schemas and credential definitions

Dependency injection
====================

When using ASP.NET Core, you can use the extension methods to configure the agent. This will add all required dependencies to the service provider.
Additionaly, the AgentFramework depends on the Logging extensions. These need to be added as well.

If using other tool, you will have to add each required service or message handler manually.

Example if using Autofac

.. code-block:: csharp

    // .NET Core dependency collection
    var services = new ServiceCollection();
    services.AddLogging();

    // Autofac builder
    var builder = new ContainerBuilder();

    // Register all required services
    builder.RegisterAssemblyTypes(typeof(IProvisioningService).Assembly)
        .Where(x => x.Namespace.StartsWith("AgentFramework.Core.Runtime", 
            StringComparison.InvariantCulture))
        .AsImplementedInterfaces()
        .SingleInstance();

    // If using message handler package, you can add all handlers
    builder.RegisterAssemblyTypes(typeof(IMessageHandler).Assembly)
        .Where(x => x.IsClass && x is IMessageHandler)
        .AsSelf()
        .SingleInstance();

    builder.Populate(services);

Check the `Xamarin Sample
<https://github.com/streetcred-id/agent-framework/blob/master/samples/xamarin-forms/AFMobileSample/App.xaml.cs>`_ for example registration.

Provisioning an Agent
=====================

The process of provisioning agents will create and configure an agent wallet and initialize the agent configuration.
The framework will generate a random Did and Verkey, unless you specify ``AgentSeed`` which is used if you need determinism. 
Length of seed must be 32 characters.

.. code-block:: csharp

    await _provisioningService.ProvisionAgentAsync(
        new ProvisioningConfiguration
        {
            EndpointUri = "http://localhost:5000",
            OwnerName = "My Agent"
        });

Check the `ProvisioningConfiguration.cs
<https://github.com/streetcred-id/agent-framework/blob/master/src/AgentFramework.Core/Models/Wallets/ProvisioningConfiguration.cs>`_
for full configuration details. You can retrieve the generated details like agent Did and Verkey using

.. code-block:: csharp

    var provisioning = await _provisioningService.GetProvisioningAsync(wallet);

Trust Anchor requirement
------------------------

If an agent is intended to act as an issuer, i.e. be able to issue credentials, their DID must be registered on the ledger with the `TRUST_ANCHOR` role.
Additionally, when provisioning the agent, set the ``ProvisioningConfiguration.CreateIssuer`` propety to true. If you already have a seed for creating the issuer DID
set the ``ProvisioningConfiguration.IssuerSeed`` to that value. Otherwise, a random DID will be generated. This DID must be added to the ledger as `TRUST_ANCHOR`.

.. tip::  If you are using the development indy node docker image, use ``000000000000000000000000Steward1`` as issuer seed. This will create a DID that has all required permissions.

***************
Agent Workflows
***************

Before you begin reading any of the topics below, please familiarize youself with the core principles behind Hyperledger Indy.
We suggest that you go over the `Indy SDK Getting Started Guide
<https://github.com/hyperledger/indy-sdk/blob/master/doc/getting-started/getting-started.md>`_.

Models and states
=================

The framework abstracts the main workflows of Indy into a state machine model.
The following models and states are defined:

Connections
-----------

Represented with a ``ConnectionRecord``, this entity describes the pairwise relationship with another party.
The states for this record are:

- ``Invited`` - initially, when creating invitations to connect, the record will be set to this state.
- ``Negotating`` - set after accepting an invitation and sending a request to connect
- ``Connected`` - set when both parties have acknowledged the connection and have a pairwise record of each others DID's

Credentials
-----------

Represented wih a ``CredentialRecord``, this entity holds a reference to issued credential.
While only the party to whom this credential was issued will have the actual credential in their wallet, both the issuer and the holder will
have a CredentialRecord with the associated status for their reference. Credential states:

- ``Offered`` - initial state, when an offer is sent to the holder
- ``Requested`` - the holder has sent a credential request to the issuer
- ``Issued`` - the issuer accepted the credential request and issued a credential
- ``Rejected`` - the issuer rejected the credential request
- ``Revoked`` - the issuer revoked a previously issued credential

Proofs
------

Represented with a ``ProofRecord``, this entity references a proof flow between the holder and verifier. The ``ProofRecord`` contains
information about the proof request as well as the disclosed proof by the holder. Proof states:

- ``Requested`` - initial state when the verifier sends a proof request
- ``Accepted`` - the holder has provided a proof
- ``Rejected`` - the holder rejected providing proof for the request

Schemas and definitions
=======================

Before an issuer can create credentials, they need to register a credential definition for them on the ledger.
Credential definition requires a schema, which can also be registered by the same issuer or it can already be
present on the ledger.

.. code-block:: csharp

    // creates new schema and registers the schema on the ledger
    var schemaId = await _schemaService.CreateSchemaAsync(
        _pool, _wallet, "My-Schema", "1.0", new[] { "FirstName", "LastName", "Email" });

    // to lookup an existing schema on the ledger
    var schemaJson = await _schemaService.LookupSchemaAsync(_pool, schemaId);

Once a ``schemaId`` has been established, an issuer can send their credential definition on the ledger.

.. code-block:: csharp

    var definitionId = await _schemaService.CreateCredentialDefinitionAsync(_pool, _wallet, 
        schemaId, supportsRevocation: true, maxCredentialCount: 100);

The above code will create ``SchemaRecord`` and ``DefinitionRecord`` in the issuer wallet that can be looked up using the
``ISchemaService``.

.. warning:: Creating schemas and definition requires an issuer. See the `Trust Anchor requirement`_ above.

To retrieve all schemas or definitions registered with this agent, use:

.. code-block:: csharp

    var schemas = await _schemaService.ListSchemasAsync(_wallet);
    var definitions = await _schemaService.ListCredentialDefinitionsAsync(_wallet);

    // To get a single record
    var definition = await _schemaService.GetCredentialDefinitionAsync(wallet, definitionId);

Establishing secure connection
==============================

Before two parties can exchange agent messages, a secure connection must be established between them. The agent connection workflow defines this handshake process by exchanging a connection request/response message.

Sending invitations
-------------------

Connection invitations are exchanged over a previously established trusted protocol such as email, QR code, deep link, etc. When Alice wants to establish a connection to Bob, she can create an invitation:

.. code-block:: csharp

    // Alice creates an invitation
    var invitation = await connectionService.CreateInvitationAsync(aliceWallet);

She sends this invitation to Bob using the above described methods.

Negotating connection
---------------------

Once Bob received the invitation from Alice, they can accept that invitation and initiate the negotiation process

.. code-block:: csharp

    // Bob accepts invitation and sends a message request
    await connectionService.AcceptInvitationAsync(bobWallet, invitation);

If you are using the default message handlers, no other step in needed - connection between Alice and Bob has been established. Use ``IConnectionService.ListAsync`` to fetch the connection records. 
Established connections will have the ``State`` property set to ``Connected``.

.. tip:: If you decide to use custom handlers and want more control over the negotiation process, the connection service provides methods to work with the connections message flows, such as processing and accepting requests/responses. 
    A full step by step code is available in the `unit tests project
    <https://github.com/streetcred-id/agent-framework/blob/master/test/AgentFramework.Core.Tests/Scenarios.cs>`_ in ``EstablishConnectionAsync``.

Credential issuance
===================

An issuer may use the ``ICredentialService`` to issue new credentials. A credential issuance starts with a credential offer.

.. code-block:: csharp

    var offerConfig = new OfferConfiguration()
    {
        // the id of the connection record to which this offer will be sent
        ConnectionId = connectionId, 
        CredentialDefinitionId = definitionId
    };
    
    // Send an offer to the holder using the established connection channel
    var credentialRecordId = await credentialService.SendOfferAsync(issuerWallet, offerConfig);

When credential offer is sent, new ``CredentialRecord`` will be created and it's state set to ``Offered``. You can list all credential records using

.. code-block:: csharp

    var credentials = await credentialService.ListAsync();

Issuing credential
------------------

.. code-block:: csharp

    var values = new Dictionary<string, string>
    {
        {"FirstName", "Jane"},
        {"LastName", "Doe"},
        {"Email", "no@spam"}
    };

    // Issuer accepts the credential requests and issues a credential
    await credentialService.IssueCredentialAsync(pool, issuerWallet, credentialRecordId, values);

An issuer can issue a credential only if the credential record state is ``Requested``. This means that the holder has accepted the offer 
and sent back a credential request message.

Storing issued credential
-------------------------

If using the default handlers, once a credential has been issued and received by the holder's agent, it will be automatically stored and available in the wallet.

Revocation
----------

If the credential definition supports revocation (can only be set when creating the definition), an issuer may decide to revoke a credential.

.. code-block:: csharp

    // Revokes a credential, updates the tails file and sends the delta to the ledger
    await credentialService.RevokeCredentialAsync(pool, wallet, credentialRecordId)

Proof verification
==================

Proof requests
--------------

Preparing proof
---------------

Verification
------------