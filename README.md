MMMH — PASS3 Game (MonoGame DesktopGL)
=====================================

A small C# MonoGame (DesktopGL) project. This repository contains the project's source, project files, and raw content assets. The built binaries are not included — follow the instructions below to build and run the project locally.

Prerequisites
-------------
- Windows with Visual Studio (2017/2019/2022) or MSBuild installed.
- .NET Framework 4.5 (project targets `net45`).
- MonoGame DesktopGL (3.7 recommended to match the packages in the repo). Install the MonoGame SDK which includes the MGCB (Content) tools.
- NuGet (usually available through Visual Studio or the `nuget` CLI).

Repository layout (important files)
----------------------------------
- `PASS3.sln` — Visual Studio solution.
- `PASS3/` — C# project folder containing `PASS3.csproj`, `Game1.cs`, `Program.cs`, and `Content/` pointers.
- `Content/Content.mgcb` — MonoGame Content Builder file. Use the MGCB editor to build content.
- `packages/` — NuGet packages (MonoGame Framework package present).

Build and run (Visual Studio)
------------------------------
1. Open `PASS3.sln` in Visual Studio.
2. In Solution Explorer, right-click the solution and select `Restore NuGet Packages` (or use `Tools -> NuGet Package Manager -> Package Manager Console` and run `Update-Package -reinstall` if needed).
3. Ensure the project references are resolved (MonoGame.Framework.DesktopGL should resolve from `packages/`).
4. Build (Debug or Release) and run from Visual Studio (F5 or Ctrl+F5).

Build and run (command line)
----------------------------
Open a Developer Command Prompt for Visual Studio and run:

```powershell
cd "c:\Users\julia\Documents\School Projects\ICS4U\KimJ_PASS3\MMMH"
nuget restore PASS3.sln
msbuild PASS3.sln /p:Configuration=Debug
# or use /p:Configuration=Release
```

If `nuget` is not installed as a global CLI, you can restore packages from within Visual Studio or use the built-in restore functionality of msbuild/VS.

MonoGame Content (MGCB)
------------------------
MonoGame uses a content pipeline to produce `.xnb` files from source assets. This repository includes `Content/Content.mgcb`.

To build the content:

1. Install the MonoGame tools (MGCB editor) via the MonoGame installer.
2. Open the MGCB Editor (or run `mgcb-editor`) and open `Content/Content.mgcb`.
3. In the MGCB Editor, click `Build` to produce the `.xnb` files inside the appropriate `bin/` output. Alternatively, run the command-line mgcb tool:

```powershell
mgcb -@:"Content/Content.mgcb" -build
```

Note: paths and exact mgcb CLI flags can vary between MonoGame versions. Using the MGCB Editor is the most straightforward on Windows.

Running the built game
----------------------
After a successful build you can run the game from Visual Studio or execute the produced `.exe` from `PASS3/bin/Debug` or `PASS3/bin/Release`.

Troubleshooting
---------------
- If MonoGame assembly references are unresolved, ensure `packages/MonoGame.Framework.DesktopGL.*` is present and you restored NuGet packages.
- If content assets are missing at runtime, rebuild `Content/Content.mgcb` with the MGCB Editor and confirm the `Content/` output is copied to the project's output directory.
- If you get runtime native library errors (openal/SDL), ensure the `x86`/`x64` native folders are present next to the executable — they are included in the repo under `PASS3/bin/*` for convenience. Use the matching platform configuration when running.

Notes
-----
- This README assumes a Windows development environment and Visual Studio. The project uses DesktopGL and can be built cross-platform with Mono/MonoGame, but steps differ by OS.
- If you prefer a newer MonoGame version (3.8+), update the MonoGame package references and verify the content pipeline compatibility.

Credits
-------
Project created for a school assignment. See the repo for source files and assets.
