**************************
Common Errors and Problems
**************************

System.DllNotFoundException
===========================

Problem
  Runtime exception thrown

.. code-block:: bash

    System.DllNotFoundException : Unable to load shared library 'libindy' or one of its dependencies. In order to help diagnose loading problems, consider setting the DYLD_PRINT_LIBRARIES environment variable: dlopen(libsovtoken, 1): image not found

Solution
  Missing static library. Check the installation section for guidance on how to add static libraries to your environment.