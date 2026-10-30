# Contributing

Thanks for your interest in improving this compiler teaching project.

## Prerequisites

- .NET 10 SDK
- Git
- a local clone of this repository

## Local setup

```bash
git clone <your-repository-url>
cd BasicCompiler
dotnet restore
dotnet build
dotnet test
```

## Project structure

- `BasicCompiler/` — main compiler project
- `BasicCompiler.Tests/` — unit tests
- `examples/` — sample programs
- `output/` — generated artifacts from compiler stages
- `sample.bas` — main example program

## Working on changes

1. Create a feature branch.
2. Keep changes focused and educational.
3. Run the relevant tests before opening a pull request.
4. Prefer small, readable, compiler-stage-oriented changes.

## Validation

Use the following commands before creating a PR:

```bash
dotnet build
dotnet test --nologo
```

## Pull requests

Please keep PRs clear and focused:

- explain the problem and the fix
- mention any new compiler artifacts or generated outputs
- include screenshots or sample output when helpful

## Notes

This project is intentionally designed as a teaching compiler, so clarity and understandability are just as important as correctness.
