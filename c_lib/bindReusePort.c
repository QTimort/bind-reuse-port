#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>
#include <memory.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>

#ifdef _WIN32
    #define Export __declspec( dllexport )
#else
    #define Export __attribute__((visibility("default")))
#endif


// Not tested but you can uncomment ipAddress if you want to specify one
Export int bindReusePort(int fd, int family, /*const char *ipAddress,*/ u_int16_t port, bool verbose) {
    struct sockaddr_in address;

    if (setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, &(int){1}, sizeof(int)) < 0) {
        if (verbose) perror("setsockopt failed");
        return (1);
    }
    if (setsockopt(fd, SOL_SOCKET, SO_REUSEPORT, &(int){1}, sizeof(int)) < 0) {
        if (verbose) perror("setsockopt failed");
        return (2);
    }

    address.sin_family = (sa_family_t) family;
    /*if (ipAddress != NULL && strlen(ipAddress) > 0)
        address.sin_addr.s_addr = inet_addr(ipAddress);
    else*/
    address.sin_addr.s_addr = INADDR_ANY;
    address.sin_port = htons(port);

    if (bind(fd, (struct sockaddr *)&address, sizeof(address)) < 0)
    {
        if (verbose) perror("bind failed");
        return (3);
    }
    return (0);
}