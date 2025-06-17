# RedisSharpLite
A super Lightweight Redis client library that aims to stay transparrent of Redis server versions, and libraries that have unique return types.

- Execute Redis commands as a single string.
- Get responses as a larger string that you can process yourself.
- Pass an empty string[], List<string>, List<int>, Dictionary<string, int>, ect to make organizing replies easier.
- Use with other Redis libraries such as RedisJson and avoid command or return type incompatibilities often found in other libraries.


This library functions as it is now, but may contain some breaking bugs.
A lot of cleaning, commenting, refactoring and fixing is on-going.

We're open to issues, and/or pull requests if you want to contribute.
If you do wish to make a pull request, we ask that you keep the "string" return type intact, as that is the goal of this library.
Others may be added as long as the original is kept intact.

# TO DO:
- YES
- Better Redis Error/Response handling
- ref/out Instead of returns? Use returns to indicate count/success?
- Persistent socket class
- Persistent socket thread?
- Async? (If I can ever figure that stuff out)
- Redis Auth
- TLS Support
- PowerShell cmdlet
