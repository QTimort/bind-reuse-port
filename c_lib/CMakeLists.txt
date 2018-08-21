cmake_minimum_required(VERSION 3.12)
project(BindReusePort C)

set(CMAKE_C_STANDARD 11)

add_library(bindreuseport SHARED bindReusePort.c)