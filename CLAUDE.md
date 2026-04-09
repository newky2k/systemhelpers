# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Restore and build entire solution
dotnet restore
dotnet build

# Build in Release (also produces NuGet packages)
dotnet build --configuration Release

# Build a single project
dotnet build DSoft.System.Helpers/DSoft.System.Helpers.csproj
dotnet build DSoft.System.Helpers.Maui/DSoft.System.Helpers.Maui.csproj
```

There are no tests in this solution. The MAUI build requires platform workloads (installed in CI via `dotnet workload install android ios maccatalyst tvos macos maui wasm-tools`).

## Architecture

This repo produces two NuGet packages plus a sample app:

**`DSoft.System.Helpers`** — Core library, multi-targeted (`netstandard2.1; net8.0; net9.0; net10.0`). Contains:
- `Extensions/PropertyMapper.cs` — Reflection-based object mapper exposed in the `System` namespace
- `Extensions/StringExtensions.cs` — Base64 encode/decode helpers
- `Extensions/ExceptionExtensions.cs` — Exception-to-JSON serialisation
- `Models/` — `ExceptionInfo`, `ExceptionSource`, `UnhandledExceptionReport`, `UnhandledExceptionReportEventArgs`

**`DSoft.System.Helpers.Maui`** — MAUI library that depends on the core library. Targets `net9.0-android`, `net9.0-ios`, `net9.0-maccatalyst`, and optionally `net9.0-windows`. Overrides the shared `TargetFrameworks` from `Directory.Build.props` because MAUI projects require platform TFMs. Contains:
- `GlobalExceptionHandler.cs` — Static class that hooks all unhandled exception sources with platform-specific handling (`#if ANDROID / IOS / MACCATALYST / WINDOWS`). Exposes two events: `UnhandledException` (raw) and `UnhandledExceptionOccurred` (rich `UnhandledExceptionReport`). Call `GlobalExceptionHandler.Init()` style by referencing its static constructor — it self-initialises on first access.
- `Platforms/` — Platform-specific partial implementations

**`SampleMAUI`** — Reference app demonstrating the MAUI package.

## Key Configuration

- **SDK version** is pinned in `global.json` to `10.0.105` with `rollForward: disable`.
- **Shared build settings** (`Directory.Build.props`) apply to all projects: multi-targeting, nullable/implicit usings, strong-name signing, NuGet metadata, and SourceLink (Release only). `GeneratePackageOnBuild=true` means every `Release` build produces `.nupkg` files.
- **Strong-name signing**: each project directory contains its own `DSoft.snk` key file — do not remove these.
- **Package version** is not set in any csproj; it is injected by the Azure Pipelines CI at build time via `/p:Version=$(Build.BuildNumber)`.

## CI Pipelines

- `azure-pipelines-mergetest.yml` — Triggered manually (PR validation); builds Release, no publish.
- `azure-pipelines-release.yml` — Triggers on `main`; builds Release and publishes `.nupkg` artifacts from `**/DSoft.*.nupkg`.
