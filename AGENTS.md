# AGENTS.md

This file provides guidance to Codex (Codex.ai/code) when working with code in this repository.

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

**`DSoft.System.Helpers`** — Core library, multi-targeted (`netstandard2.1; net8.0; net9.0; net10.0; net11.0`). Contains:
- `Extensions/PropertyMapper.cs` — Reflection-based object mapper exposed in the `System` namespace
- `Extensions/StringExtensions.cs` — Base64 encode/decode helpers
- `Extensions/ExceptionExtensions.cs` — Exception-to-JSON serialisation
- `Models/` — `ExceptionInfo`, `ExceptionSource`, `UnhandledExceptionReport`, `UnhandledExceptionReportEventArgs`

**`DSoft.System.Helpers.Maui`** — MAUI library that depends on the core library. Targets `net10.0` and `net11.0` (the `net9.0` MAUI targets were dropped: the .NET 10/11 SDK rejects the out-of-support net9 MAUI workloads with NETSDK1202) with the `-android`, `-ios` and `-maccatalyst` platforms, plus `-windows10.0.19041.0` when built on Windows. `Microsoft.Maui.Controls` is 11.0.10 for `net11.0-*` and 10.0.130 for `net10.0-*`; `net11.0-android` needs a minimum Android API of 24. Overrides the shared `TargetFrameworks` from `Directory.Build.props` because MAUI projects require platform TFMs. Contains:
- `GlobalExceptionHandler.cs` — Static class that hooks all unhandled exception sources with platform-specific handling (`#if ANDROID / IOS / MACCATALYST / WINDOWS`). Exposes two events: `UnhandledException` (raw) and `UnhandledExceptionOccurred` (rich `UnhandledExceptionReport`). Call `GlobalExceptionHandler.Init()` style by referencing its static constructor — it self-initialises on first access.
- `Platforms/` — Platform-specific partial implementations

**`SampleMAUI`** — Reference app demonstrating the MAUI package.

## Key Configuration

- **SDK version** is set in `global.json` to `10.0.100` with `rollForward: latestMajor`, so the .NET 11 SDK is used when installed. Build with the .NET 10 and .NET 11 SDKs.
- **Shared build settings** (`Directory.Build.props`) apply to all projects: multi-targeting, nullable/implicit usings, strong-name signing, NuGet metadata, and SourceLink (Release only). `GeneratePackageOnBuild=true` means every `Release` build produces `.nupkg` files.
- **Strong-name signing**: each project directory contains its own `DSoft.snk` key file — do not remove these.
- **Package version** is not set in any csproj; it is injected by the GitHub Actions release workflow at build time as `1.0.<yyMM>.<run number>` via `/p:Version`.

## Branching

- `development` is the working branch: branch from it and open pull requests into it. Never commit or open a pull request directly against `main`.
- `main` is the release branch. Every push to `main` runs `release.yml`, which publishes the packages to nuget.org and creates a GitHub release, so only merge `development` into `main` when a release is intended, and only when asked to release.

## CI

GitHub Actions (`.github/workflows/`) on `windows-latest`, with the MAUI workloads restored by `dotnet workload restore`:

- `ci.yml` builds Release for pull requests into `main` and `development`; it publishes nothing.
- `release.yml` runs on every push to `main` (changes only to Markdown or workflow files are skipped; run it by hand to test a workflow change). It builds Release as `1.0.<yyMM>.<run number>` plus `RELEASE_SUFFIX` (empty for a stable version), pushes `DSoft.System.Helpers` and `DSoft.System.Helpers.Maui` to nuget.org with Trusted Publishing (OIDC, `NUGET_USER` secret, `nuget` environment), then tags the commit `v<version>` and creates a GitHub release with the packages attached.
