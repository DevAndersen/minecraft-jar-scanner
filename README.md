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

## Q&A

**Q:** Why is the program file so large?

- **A:** Rather than requiring you to install the [.NET runtime](https://dotnet.microsoft.com) separately, the program file includes the necessary runtime. This was done to avoid requiring the .NET runtime to be installed separately on your computer, but does mean that the program file is correspondingly larger.

**Q:** Why is the user interface accessed via a web browser?

- **A:** I mostly work with [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) these days, which I also chose to use for this application. I simply felt it was the easiest choice for me to writing a user interface in. This application does not send or receive any data via the Internet.

**Q:** Does this application change any of my files?

- **A:** No, it simply reads through the directories you tell it to search through (including checking inside of .zip files). It does not change any of the files it looks at.

**Q:** Does this application gather any personal data or telemetry?

- **A:** No, none of your personal data is gathered or sent anywhere. Do note though, that if you save a log file after a scan, it will contain the full paths of the files it found, which may contain things like your local username that might be considered sensitive data. You should therefore read these files through before sending them to anyone, in order to make sure that you are not sending any information that you might not wish to share with others.

## Disclaimer

This project is in no way affiliated with Mojang Studios, Microsoft, or any of their partners.

This project does not contain any of Minecraft's source code or assets. I do not claim any legal ownership over Minecraft, its assets, or any other related content.

## License

[MIT license](./LICENSE)
