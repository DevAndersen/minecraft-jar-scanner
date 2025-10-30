# minecraft-jar-scanner

A quickly botched together project that scans for what might be Minecraft `.jar` files. Application output requires manual validation.

This is intended to look for files not currently indexed by Omniarchive.

It will look for any `.jar` files it can find in the specified directory, and also look inside of `.zip` files (and `.zip` files inside of `.zip files`).

Disclaimer: The code quality is low, and it's not meant to be particularly flexible or nice to work with. But it should work. Hopefully.

## How to use

- Compile the code (targeting .NET 10 (in preview as of writing) or later).
- Create text file called `hash.txt` in the same directory as the compiled executable (`MinecraftJarScanner.exe`).
- Add the MD5 hashes of all files listed on the Omniarchive Index ([link](https://docs.google.com/spreadsheets/d/1OCxMNQLeZJi4BlKKwHx2OlzktKiLEwFXnmCrSdAFwYQ)).
- Run the executable, and input the directory you want to search through.
  - This can take a long time, depending on the number and size of files to look through, as well as the kind of storage (HDDs are gonna be slow).
- Once the file scan is done, it will create `filelist.txt` which will contain the list of all `.jar` and `.zip` files it came across.
  - This file is effectively a cache. If it exists when the application is executed, the file scan will be skipped. So make sure to delete this file if you want to scan another directory.
- The application will now look analyze each of the files in the file list, in order to determine the following:
  - What's the file's MD5 hash?
  - Does the MD5 hash exist in `hash.txt`?
  - Does it look like a Minecraft client jar?
  - Does it have the expected `META-INF` folder of a Minecraft client jar?
  - Does it look like a Minecraft server jar?
  - Does it have the expected `META-INF` folder of a Minecraft server jar?
- If the hash is unique, and if either the client checks or the server checks both returned true, the file will be considered a potential candidate.
- The results of each file analysis will be written to the console, and also written to `output.txt`.
- When it's all done, the application should produce an audible beep noise, and then terminate.
- You now have to manually look through the `output.txt`. Search for `Candidate:  True`, and check if each of those files might be of interest.

The output of each file analysis will look as follows:

        D:\Gaming\Minecraft\SomeOldJunkFolder\minecraft_server.jar
                   MD5:        5844FA239B1F3B0C0D1F3BFF3D8ED905
                   IsClient    False
                   ClientMeta: False
                   IsServer    True
                   ServerMeta: True
                   Known:      False
                   Candidate:  True

## Disclaimers

- This is a botched together project. The code is ugly, but it gets the job done.
- The code is not guaranteed to work.
- I am not affiliated with neither Microsoft, Mojang, or Omniarchive. I don't claim any ownership of any trademarks, don't sue me if this project causes any harm, etc etc... You know, the usual disclaimers.

## License

[MIT license](./LICENSE)
