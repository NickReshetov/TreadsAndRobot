# Threads

Implement a console application with 2 separate threads apart 
from the main thread that continually write to the console. 

The first thread should write the text Thread1 to the console 
in for example cyan color every 0.1s and the other thread 
should write Thread2 to the console in grey color every 0.2s. 

While these texts are being printed to the console window 
the main thread should still be responsive and waiting for 
input from the user.

As soon as the user starts to type something the application 
should interrupt and cancel the 2 threads gracefully. 
The main thread should then wait/sleep for 1 second and 
then exit the application. 

## Prerequisites

To build and run this code, you will need the .NET Core SDK 
version 3.0.100, which can be downloaded from
https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-3.0.100-windows-x64-installer.
