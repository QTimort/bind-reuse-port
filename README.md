# bind-reuse-port
Provide a way to bind an address already in use on Linux with C#

Currently C# Linux doesn't allow you to bind to an address already in use.
The solution provided is to call Linux native functions through a C library to set "reuse" flags and to bind the socket.

It is confirmed working with:
- dotnetcore 2.1 on Linux Ubuntu 18.04 (Bionic)
- mono 2.1.4 on Linux Ubuntu 16.04
- Windows (Obviously...)

# Instruction
First compile the C library
- cd c_lib

  - gcc -c -fpic bindReusePort.c && gcc -shared -o libbindreuseport.so bindReusePort.o && rm bindReusePort.o
  
  OR
  
  - cmake CMakeLists.txt
  - make
  
- cp libbindreuseport.so ../example

Then compile and run the C# example either with your IDE or with the command line like below
- cd ../example
- dotnet build
- dotnet run

Wait some seconds...

- (in another terminal) echo "Hello world" > /dev/udp/127.0.0.1/5353

If you see the message "received: " after using the command above then it mean that the solution is working.

# Possible improvement
I tried to call setsockopt C function from C# but the function returned -1 which mean that an error occured. That is why I made a library that call those function from a "C context" which work as opposed to direct C call from C# for unknown reason(s).

I was able to close a socket using the C function close.

You can try to call C function by declaring them like this:
  [DllImport("libc.so.6")]
  private static extern int setsockopt(int socket, int level, int option_name, void *ptr, int optlen);
  
   [DllImport("libc.so.6")]
  private static extern int close(int fd);
  
  ...
