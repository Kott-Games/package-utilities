# Changelog

All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-10-19

First public release.

### Added
- Package metadata for public release: MIT license, README, CHANGELOG, `.editorconfig`.
- Declared dependencies on Newtonsoft JSON and uGUI (previously only present by accident in the host project).
- Test assemblies (`Kott.Utilities.Tests.Editor`, `Kott.Utilities.Tests`, `Kott.Utilities.Testing`) with noise and
  version tests.

### Fixed
- `NoiseValue` no longer needs `System.Runtime.CompilerServices.Unsafe`.
