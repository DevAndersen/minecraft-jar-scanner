# Minecraft Jar Scanner

A tool to help you search for Minecraft-related .jar files.

## Overview

Minecraft Jar Scanner is an application designed to scan your computer for Minecraft related .jar files.

The primary goal is to help search for missing versions of Minecraft's client and server .jar files.

When scanning, this application will look for all .jar files in the specified directory. This includes .jar files located inside .zip files. It then performs some simple assertions to determine if the .jar file looks like a Minecraft client or server .jar file, and if so, lists them in the output.

The application uses a web-based user interface (using [Bootstrap](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)), hosted by a locally run web server (using [Blazor](https://aka.ms/blazor)). It does not send or receive any information over the Internet, it simply uses your web browser as its frontend.

Note: The speed of the scan will depend on how many files are being scanned, as well as the physical storage media. Scanning for files on an [SSD](https://en.wikipedia.org/wiki/Solid-state_drive) will be signfiicantly faster than doing them same on a traditional [HDD](https://en.wikipedia.org/wiki/Hard_disk_drive).

## Build from source

To build this project from source, you need the [.NET SDK](https://dotnet.microsoft.com/en-us/download) installed (.NET 10 or later).

*=== TODO ===*

## Disclaimer

This project is in no way affiliated with Mojang Studios, Microsoft, or any of their partners.

This project does not contain any of Minecraft's source code or assets. I do not claim any legal ownership over Minecraft, its assets, or any other related content.

## License

[MIT license](./LICENSE)
