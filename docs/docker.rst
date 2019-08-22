***********************************
Hosting agents in docker containers
***********************************

Hosting agents in docker container is the easiest way to ensure your running environment has all dependencies required by the framework.
We provide images with libindy and dotnet-sdk preinstalled.

Usage
=====

.. code-block:: docker
    :name: Docker

    FROM streetcred/dotnet-indy:latest

The images are based on `ubuntu:16.04`. You can check `the docker repo
<https://github.com/streetcred-id/docker>`_ if you want to build your own image or require specific version of .NET Core or libindy.

Example build
=============

Check the `web agent docker file
<https://github.com/streetcred-id/agent-framework/blob/master/docker/web-agent.dockerfile>`_ for an example of building and running ASP.NET Core project inside docker container with libindy support.