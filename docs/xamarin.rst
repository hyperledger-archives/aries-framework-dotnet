**************************
Mobile Agents with Xamarin
**************************

Using Indy with Xamarin
=======================

When working with Xamarin, we can fully leverage the offical `Indy wrapper for dotnet`_, since the package is fully compatible with Xamarin runtime. The wrapper uses ``DllImport`` to invoke the native Indy library which exposes all functionality as C callable functions. 
In order to make the library work in Xamarin, we need to make `libindy` available for Android and iOS, which requires bundling static libraries of libindy and it's dependencies built for each platform.

.. _`Indy wrapper for dotnet`: https://github.com/hyperledger/indy-sdk/tree/master/wrappers/dotnet

Instructions for Android
========================

To setup Indy on Android you need to add the native libindy references and dependencies. The process is described in detail at the official Xamarin documentation `Using Native Libraries with Xamarin.Android`_.

.. _`Using Native Libraries with Xamarin.Android`: https://docs.microsoft.com/en-us/xamarin/android/platform/native-libraries

Below are a few additional things that are not covered by the documentation that are Indy specific.

Download static libraries
-------------------------

- Our repo (includes `libgnustl_shared.so`) - `samples/xamarin/libs-android`_ 
- Sovrin repo - https://repo.sovrin.org/android/libindy/

.. _`samples/xamarin/libs-android`: https://github.com/streetcred-id/agent-framework/tree/master/samples/xamarin/libs-android>

For Android the entire library and its dependencies are compiled into a single shared object (libindy.so). In order for ``libindy.so`` to be executable we must also include ``libgnustl_shared.so``.

.. note:: You can find ``libgnustl_shared.so`` in your ``android-ndk`` installation directory under ``\sources\cxx-stl\gnu-libstdc++\4.9\libs``.

Depending on the target abi(s) for the resulting app, not all of the artifacts need to be included, for ease of use below we document including all abi(s).

Setup native references
----------------------------

In Visual Studio (for Windows or Mac) create new Xamarin Android project. If you want to use Xamarin Forms, the instructions are the same. Apply the changes to your Android project in Xamarin Forms.

The required files can be added via your IDE by clicking Add-Item and setting the build action to ``AndroidNativeLibrary``. However when dealing with multiple ABI targets it is easier to manually add the references via the android projects .csproj. Note - if the path contains the abi i.e `..\x86\library.so` then the build process automatically infers the target ABI.

If you are adding all the target ABI's to you android project add the following snippet to your .csproj.

.. code-block:: xml

    <ItemGroup>
      <AndroidNativeLibrary Include="..\libs-android\armeabi\libindy.so" />
      <AndroidNativeLibrary Include="..\libs-android\arm64-v8a\libindy.so" />
      <AndroidNativeLibrary Include="..\libs-android\armeabi-v7a\libindy.so" />
      <AndroidNativeLibrary Include="..\libs-android\x86\libindy.so" />
      <AndroidNativeLibrary Include="..\libs-android\x86_64\libindy.so" />
      <AndroidNativeLibrary Include="..\libs-android\armeabi\libgnustl_shared.so" />
      <AndroidNativeLibrary Include="..\libs-android\arm64-v8a\libgnustl_shared.so" />
      <AndroidNativeLibrary Include="..\libs-android\armeabi-v7a\libgnustl_shared.so" />
      <AndroidNativeLibrary Include="..\libs-android\x86\libgnustl_shared.so" />
      <AndroidNativeLibrary Include="..\libs-android\x86_64\libgnustl_shared.so" />
    </ItemGroup>

.. note:: Paths listed above will vary project to project.

Load runtime dependencies
-------------------------

Load these dependencies at runtime. To do this add the following to your MainActivity.cs

.. code-block:: csharp

    JavaSystem.LoadLibrary("gnustl_shared");
    JavaSystem.LoadLibrary("indy");

Setup Android permissions
-------------------------

In order to use most of libindy's functionality, the following permissions must be granted to your app, you can do this by adjusting your AndroidManifest.xml, located under properties in your project.

.. code-block:: xml

    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.INTERNET" />

If you are running your android app at API level 23 and above, these permissions also must be requested at runtime, in order to do this add the following to your MainActivity.cs

.. code-block:: csharp

    if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
    {
        RequestPermissions(new[] { Manifest.Permission.ReadExternalStorage }, 10);
        RequestPermissions(new[] { Manifest.Permission.WriteExternalStorage }, 10);
        RequestPermissions(new[] { Manifest.Permission.Internet }, 10);
    }

Instructions for iOS
====================

To setup Indy on iOS you need to add the native libindy references and dependencies. 
The process is described in detail at the official Xamarin documentation `Native References in iOS, Mac, and Bindings Projects
<https://docs.microsoft.com/en-us/xamarin/cross-platform/macios/native-references>`_.

Below are a few additional things that are not covered by the documentation that are Indy specific.

Download static libraries
-------------------------

In order to enable the Indy SDK package to recognize the `DllImport` calls to the native static libraries, we need to include them in our solution.

These includes the following static libraries:

- libindy.a
- libssl.a
- libsodium.a
- libcrypto.a
- libzmq.a

Pre-built libraries
```````````````````

Can be found in the `iOS sample project
<https://github.com/streetcred-id/agent-framework/tree/master/samples/xamarin/libs-ios>`_.

Build your own libs
```````````````````

The Indy team doesn't provide static libraries for all of the dependencies for iOS. 
Here are some helpful instructions on building the dependencies for iOS should you decide to build your own.

- `Open SSL for iOS`_
- `Build ZeroMQ library`_
- `libsodium script of iOS`_

.. _`Open SSL for iOS`: https://github.com/x2on/OpenSSL-for-iPhone

.. _`Build ZeroMQ library`: https://www.ics.com/blog/lets-build-zeromq-library

.. _`libsodium script of iOS`: https://github.com/jedisct1/libsodium/blob/master/dist-build/ios.sh

The above links should help you build the 4 static libraries that libindy depends on. To build libindy for iOS, check out the offical Indy SDK repo or [download the library from the Sovrin repo](https://repo.sovrin.org/ios/libindy/).

Setup native references
-----------------------

In Visual Studio (for Windows or Mac) create new Xamarin iOS project. If you want to use Xamarin Forms, the instructions are the same. Apply the changes to your iOS project in Xamarin Forms.

Add each library as native reference, either by right clicking the project and Add Native Reference, or add them directly in the project file.

.. note:: Make sure libraries are set to ``Static`` in the properties window and ``Is C++`` is selected for ``libzmq.a`` only.

The final project file should look like this (paths will vary per project):

.. code-block:: xml

    <ItemGroup>
      <NativeReference Include="..\libs-ios\libcrypto.a">
        <Kind>Static</Kind>
      </NativeReference>
      <NativeReference Include="..\libs-ios\libsodium.a">
        <Kind>Static</Kind>
      </NativeReference>
      <NativeReference Include="..\libs-ios\libssl.a">
        <Kind>Static</Kind>
      </NativeReference>
      <NativeReference Include="..\libs-ios\libzmq.a">
        <Kind>Static</Kind>
        <IsCxx>True</IsCxx>
      </NativeReference>
      <NativeReference Include="..\libs-ios\libindy.a">
        <Kind>Static</Kind>
      </NativeReference>
    </ItemGroup>

Update MTouch arguments
-----------------------

In your project options under `iOS Build` add the following to `Additional mtouch arguments`

.. code-block:: bash
  
    -gcc_flags -dead_strip -v

If you prefer to add them directly in the project file, add the following line:

.. code-block:: xml

    <MtouchExtraArgs>-gcc_flags -dead_strip -v</MtouchExtraArgs>

.. warning:: This step is mandatory, otherwise you won't be able to build the project. 
    It prevents linking unused symbols in the static libraries. Make sure you add these arguments for all configurations. See `example project file
    <https://github.com/streetcred-id/agent-framework/blob/771aaff84b6059a3a7d83e6d9ce1e01fefd10b64/samples/xamarin/AFMobileSample.iOS/AFMobileSample.iOS.csproj#L18>`_.

Install NuGet packages
----------------------

Install the Nuget packages for Indy SDK and/or Agent Framework and build your solution. Everything should work and run just fine.

.. code-block:: bash

    dotnet add package AgentFramework.Core --source https://www.myget.org/F/agent-framework/api/v3/index.json

----

If you run into any errors or need help setting up, please open an issue in this repo.

Finally, check the `Xamarin Sample
<https://github.com/streetcred-id/agent-framework/tree/master/samples/xamarin-forms>`_ we have included for a fully configured project.
