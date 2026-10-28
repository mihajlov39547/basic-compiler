# AGENTS.md

## Purpose

This repository is an educational C# compiler/interpreter project for a master-level course on compiler construction. The goal is to teach how source code moves through the major phases of a language implementation without turning the project into a production compiler.

MiniBASIC is intentionally small, explicit, and inspectable. The project must remain faithful to the educational pipeline:

```text
Source Code
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

This file is the main entry point for AI agents working in this repository. The project guardrails below are part of the same policy and should be followed as a single instructional contract.

## Architecture guardrails

- Keep compiler phases explicitly separated: lexing, parsing, semantics, runtime, bytecode, VM.
- Do not merge all phases into one monolithic `Compiler` class.
- Keep each stage with a clear input and output.
- Preserve source positions and spans for diagnostics.
- Prefer simple, inspectable data structures over generically abstract frameworks.
- Use directory and namespace boundaries to reflect compiler phases.
- Avoid introducing abstractions before the project has introduced the problem that motivates them.
- Prefer clarity and teaching value over industrial performance or framework sophistication.
- Do not replace the core educational stage with a library that performs the concept being taught.
- Treat the lexer as a cursor-based scanner with a current/peek/advance model. This is iterator-like in teaching terms, but it is not a requirement to force `IEnumerator` or other .NET iterator abstractions onto the project.

## Feature freeze

The MiniBASIC language is intentionally small. Do not invent additional language features just because they make implementation more elegant.

Allowed core concepts:

- Number, String, Boolean, Array
- `var x := value`
- assignment with `:=`
- arithmetic operators: `+ - * /`
- comparison: `= != < <= > >=`
- logical operators: `&& || !`
- `if ... then ... else ... endif`
- `while ... do ... endwhile`
- simple functions with explicit parameter types and return values

Do not add, unless explicitly required by an assignment:

- generics
- classes and inheritance
- interfaces
- modules or package systems
- async/await
- concurrency
- advanced GC
- macros
- reflection
- exceptions as normal control flow
- extensive type inference
- LLVM backend
- JIT compiler
- production linker
- production debugger
- IDE/LSP tooling
- industrial optimizer framework

## AI workflow rules

Before making a non-trivial change, the agent should state:

1. Problem: what are we trying to demonstrate?
2. Concept: which compiler concept solves it?
3. Representation: what data structure changes?
4. Minimal change: what is the smallest implementation?
5. Test: how do we know it works?

Additional rules:

- Favor simple + explicit + testable over clever + optimized + abstract.
- Do not solve future lessons early. Implement only concepts already introduced unless the scaffold is technically unavoidable.
- Keep educational stages inspectable and printable.
- Do not add design patterns just because they are fashionable. Only add them when a clear compiler problem motivates them.
- Keep the project aligned with the course progression. The objective is understanding the compiler pipeline, not building a product compiler.

## Testing rules

Testing is part of the compiler architecture.

Every component should have small, deterministic tests that validate observable contracts:

- Lexer: source → expected tokens
- Parser: tokens → expected AST
- Resolver: AST → expected symbols/errors
- Type checker: AST → expected types/errors
- Interpreter: program → expected output/value
- Bytecode compiler: AST → expected instructions
- VM: bytecode → expected observable result

Rules:

- Prefer small tests over large architectural test harnesses.
- Keep tests deterministic and phase-focused.
- Prefer adding a test when adding a new concept or behavior.
- Do not treat tests as an afterthought; they are part of the educational contract.

## Artifact format contract

The project must keep every intermediate artifact precise and stable. A raw `ToString()` dump is not sufficient for teaching because it hides the difference between the programmer’s lexeme, the compiler’s semantic value, and the source location.

### 01-tokens.txt contract

Each emitted token must have a deterministic row with the following columns:

- `#`: sequential token index starting at 0.
- `Kind`: normalized token classification such as `Var`, `Identifier`, `Number`, `String`, `Assign`, `Plus`, `If`, `Then`, `EOF`.
- `Text`: exact lexeme as it appears in the source file.
- `Value`: semantic value when applicable; `-` for non-value tokens.
- `Span`: zero-based half-open interval `[start..end)`.
- `Location`: one-based source location in the form `line:column`.

Lexer rules:

- Whitespace is consumed but not emitted as tokens.
- Newlines are tracked for source locations but are not normally emitted as tokens.
- Comments are consumed but not emitted as tokens.
- EOF is always emitted as the final token.
- EOF has zero width and its text is empty.
- Number tokens include their parsed numeric value in `Value`.
- String tokens include the decoded semantic value in `Value`, while `Text` preserves the original source lexeme.
- Identifier values are `-`; the spelling is already represented by `Text`.
- Token ordering must exactly match source order.
- Output must be deterministic and suitable for golden-file tests.

This distinction is especially important for strings:

```text
var message := "Hello\nWorld"
```

The compiler should treat:

- `Text` as the characters written by the programmer
- `Value` as the value understood by the compiler

### 02-ast.txt contract

AST dumps should similarly show:

- node kind
- relevant values or identifiers
- tree hierarchy
- important typed structure where applicable

The goal is not merely “pretty printing”; it is making the tree inspectable and explainable to students.

## Build and validation commands

Run from the project directory containing the `.csproj` file:

```bash
dotnet build
dotnet run --project .
dotnet test --nologo
```

## GoF pattern lecture sequence

The first explicit GoF pattern in this project should be `Composite`, because the AST is the canonical recursive tree structure of a compiler.

```text
Expression
├── LiteralExpression
├── VariableExpression
├── UnaryExpression
├── BinaryExpression
├── ArrayExpression
└── CallExpression
```

A `BinaryExpression` is a branch node that contains child `Expression` values:

```text
BinaryExpression
├── Left: Expression
└── Right: Expression
```

This is exactly the Composite pattern: leaves and composites share the same abstraction, and the tree is recursively composed.

The next natural GoF step is `Visitor`.

```text
One AST
  └── many traversals
       ├── AstPrinter
       ├── NameResolver
       ├── TypeChecker
       ├── Interpreter
       └── Lowerer
```

Without `Visitor`, the AST nodes would accumulate unrelated responsibilities such as printing, resolution, type checking, interpretation, and lowering. The visitor pattern lets one tree support many operations without turning every node class into an enormous god object.

The project should teach this progression explicitly:

1. `Composite` for the AST tree
2. `Visitor` for walk operations over the tree
3. `Strategy` only where runtime behavior varies by mode or command
4. `Factory` only when object creation becomes a real source of complexity

## Notes for future work

The repository should evolve in a teaching-friendly sequence:

1. Lexer and token stream
2. Parser and AST
3. Name resolution and type checking
4. AST interpreter
5. Lowering and IR
6. Bytecode and virtual machine
7. Optional optimization and backend work

This sequence should stay visible in the project structure so students can trace the path from source code to execution.

## Guardrail summary

MiniBASIC is not the product. The observable journey from source code to execution is the product.

The agent must maintain this educational focus at all times: clear compiler stages, small language scope, explicit representations, and tests that reveal the behavior of each phase.
