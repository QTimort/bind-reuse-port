# bind-reuse-port
Provide a way to bind an address already in use on Linux with C#

Currently C# Linux doesn't allow you to bind to an address already in use.
The solution provided is to call Linux native functions through a C library to set "reuse" flags and to bind the socket.

It has been tested with dotnetcore 2.1 on Linux Ubuntun 18.04 (Bionic)

# Instruction
First compile the C library
- cd c_lib
- cmake CMakeLists.txt
- make
- cp libbindreuseport.so ../example

Then compile and run the C# example either with your IDE or with the command line like below
- cd ../example
- dotnet build
- dotnet run
