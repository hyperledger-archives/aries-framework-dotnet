********
Payments
********

Payments are a core feature of the framework and are implemented as optional module.
Working with payments requires an implementation of the ``IPaymentService`` for that specific payment method. Currently, there's support for Sovrin Token payments.

Installation
============

Packages supporting Sovrin Token payments can be found on nuget.org

**Package Manager CLI:**

.. code-block:: bash

    Install-Package AgentFramework.Payments.SovrinToken

**.NET CLI:**

.. code-block:: bash

    dotnet add package AgentFramework.Payments.SovrinToken

Add libsovtoken static library
------------------------------
    

Configuration
=============

Records and services
====================

Working with payments
=====================

Create and set default payment address
--------------------------------------

Check balance at address
------------------------

Attaching payments to agent messages
------------------------------------

Making payments
---------------

Attaching payment receipt to agent messages
-------------------------------------------

Using libnullpay for development
================================