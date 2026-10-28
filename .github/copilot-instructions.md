# GitHub Copilot Instructions for MiniBASIC

This repository is an educational compiler/interpreter project for a master-level course on compiler construction.

## Primary source of truth

Use the project instruction files in this repository as the canonical guidance:

- [AGENTS.md](../AGENTS.md)
- [AGENTS-guardrails.md](../AGENTS-guardrails.md)
- [LanguageSpecification.md](../LanguageSpecification.md)

## Project purpose

The purpose is to teach the journey of source code through the main phases of a programming language implementation:

```text
Source
-> Lexer
-> Tokens
-> Parser
-> AST
-> Name Resolution
-> Type / Semantic Checking
-> Interpreter
-> Simple Bytecode
-> Simple Virtual Machine
```

This project is not a production compiler and should not evolve into one without explicit course requirements.

## Guardrails

- Keep MiniBASIC intentionally small.
- Do not introduce production-language features unless explicitly requested.
- Prefer clarity, explicitness, and teachability over optimization or abstraction.
- Keep compiler stages clearly separated.
- Preserve source spans and diagnostic relevance.
- Prefer small, deterministic tests over large framework-heavy design.
- Avoid introducing abstractions before the course has motivated them.
- Keep representations inspectable by students.

## Non-goals

Do not add, unless explicitly required:

- generics
- classes/inheritance
- advanced module systems
- async/await
- concurrency
- LLVM-native backend
- JIT compiler
- production linker or debugger
- enterprise framework patterns without educational need

## Definition of done for changes

For meaningful changes, prefer a response that explains:

1. Problem
2. Compiler concept involved
3. Representation change
4. Minimal implementation
5. Test to validate it

## Workflow

- Keep the compiler pipeline explicit and phase-based.
- Favor small, educationally meaningful implementations over broad scope.
- Preserve the project’s learning goals and language specification.
- Add tests alongside new compiler behavior.

When in doubt, stay within the project’s main educational aim: show how a compiler moves from source text to execution, in a way students can inspect and understand.
