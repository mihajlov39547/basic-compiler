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

### Interpreter pattern

This project already has a real interpreter runtime pass, and it is a genuine application of the `Interpreter` pattern.

```text
Add
├── Number(10)
└── Number(20)
```

The interpreter evaluates the tree:

```text
Visit(Add)
  → Visit(left) = 10
  → Visit(right) = 20
  → 10 + 20 = 30
```

This is not an artificial toy example. It is an actual language runtime for the MiniBASIC AST. The pattern is meaningful because the AST represents the language grammar, and the interpreter evaluates those grammar nodes according to language semantics.

The project should teach it this way:

- `Composite` gives us the AST structure
- `Visitor` provides traversal over the tree
- `Interpreter` gives the tree runtime behavior

This is a genuine compiler pattern, but it is still a pass within the broader pipeline, not the entire compiler architecture.

### Scope lookup and the Chain of Responsibility analogy

The compiler also has nested scope chains, but the core concept is lexical scope, not the GoF pattern itself.

```text
Current Scope
  ↓
Parent Scope
  ↓
Global Scope
```

This is the real compiler mechanism: a name is resolved by walking outward through parent environments until the symbol is found or the chain ends. The lookup behavior is analogous to `Chain of Responsibility`, but the true educational concept is `lexical scoping` and the `nested symbol table / environment` model.

We should teach the analogy carefully:

- lexical scoping is the compiler concept
- scope lookup is the mechanism
- Chain of Responsibility is a helpful OO interpretation of the lookup behavior
- the pattern is not the origin of the language feature

### Symbol table and name binding

`SymbolTable` is a compiler-specific data structure rather than a classic GoF pattern.

```text
"a"       -> VariableSymbol #1 : Number
"message" -> VariableSymbol #2 : String
"print"   -> FunctionSymbol #3
```

The important semantic step is binding: the resolver transforms a raw identifier into a symbol-aware reference.

```text
VariableExpression("a")
    ↓ resolves in current scope
BoundVariableExpression(symbol = VariableSymbol#1)
```

This project models the same idea in a lightweight form by attaching a `BoundSymbol` to the variable expression after successful resolution.

### Diagnostic bag / collector

This is an engineering compiler idiom rather than a GoF pattern.

```text
Lexer ──────┐
Parser ─────┤
Resolver ───┼──→ DiagnosticBag
TypeChecker ┤
Lowerer ────┘
```

Each phase produces `Diagnostic` objects containing:

- code
- severity
- message
- source span

The important architectural rule is separation of concerns:

```text
compiler logic ≠ presentation of errors
```

The compiler should collect facts, warnings, and errors in a structured way without directly calling `Console.WriteLine` all over the codebase. Presentation is a later concern and can render the same diagnostic bag in CLI, test output, or IDE tooling.

The project should teach this progression explicitly:

1. `Composite` for the AST tree
2. `Visitor` for walk operations over the tree
3. nested `Scope` / environment resolution for lexical scoping
4. symbol table binding for identifier-to-symbol mapping
5. `DiagnosticBag` as the shared collector for all compiler phases
6. `Strategy` only where runtime behavior varies by mode or command
7. `Factory` only when object creation becomes a real source of complexity

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

## Advanced compiler topics for later study

This section is intentionally separate from the core beginner path. The project should remain small and understandable first, and only add these ideas when the compiler has a concrete reason to grow.

### Lowering / desugaring

A core compiler idea is that higher-level syntax can be lowered into a simpler internal form.

Example for MiniBASIC:

```text
for i := 1 to 10
    print(i)
next
```

can eventually become a simpler while-based form:

```text
var i := 1

while i <= 10
    print(i)
    i := i + 1
endwhile
```

This is not just syntax sugar for convenience; it is a fundamental compiler transformation pattern: rich syntax is rewritten into a more uniform internal representation.

The important teaching lesson is:

```text
AST
  ↓
lowered AST / IR
  ↓
bytecode or execution representation
```

This is the right place to teach `Lowering` and `Desugaring`, because the transformation lowers the conceptual burden for later passes.

### Pass-based compiler pipeline

Once optimization or analysis work grows, the compiler naturally turns into a sequence of passes.

```text
IR
  ↓
ConstantFoldingPass
  ↓
Verifier
  ↓
ConstantPropagationPass
  ↓
Verifier
  ↓
DeadCodeEliminationPass
```

The pattern is straightforward:

```text
IPass<T>
    Run(T input) -> T output
```

This is the natural extension of the project’s existing pipeline model and is a better teaching concept than forcing a large framework onto the compiler.

### Strategy for optimization and backend selection

When the compiler must vary behavior by mode or target, `Strategy` becomes useful.

```text
OptimizationStrategy
    ├── None
    ├── Basic
    └── Aggressive
```

The same idea applies later to backend selection:

```text
IBackend
    ├── BytecodeBackend
    ├── LLVMBackend
    └── WasmBackend
```

This is a real extensibility pattern when the project has multiple execution targets or optimization levels.

### Builder / emitter for bytecode

If a bytecode backend is added later, a `BytecodeEmitter` is a strong design choice.

```text
BytecodeEmitter
    ├── Emit()
    ├── EmitJump()
    ├── CreateLabel()
    ├── MarkLabel()
    └── PatchJump()
```

This is much cleaner than representing bytecode as raw arrays being patched by hand across many places. It matches the builder/emitter idea well without being required for the initial educational compiler.

### Command-like instruction model

A VM instruction set can be modeled as commands or instruction records rather than a one-class-per-opcode explosion.

```text
Instruction
    ├── opcode
    ├── operand(s)
    └── source location
```

This is conceptually useful and teaches the difference between a runtime instruction representation and a full object-oriented command hierarchy. The project should avoid creating dozens of tiny command classes unless that structure becomes genuinely necessary.

### VM state and stack frames

Later runtime work naturally introduces machine state:

```text
VirtualMachineState
    ├── instructionPointer
    ├── operandStack
    ├── callStack
    ├── locals
    └── currentFunction
```

This is a crucial concept for students, but it is a runtime concern, not a reason to force a GoF pattern onto the whole design. The important teaching idea is the state machine of execution, not the pattern name.

### Patterns to avoid forcing

The project should explicitly resist a few anti-patterns:

- `Singleton` for global compiler state
- `Factory` everywhere for every node creation
- “one design pattern per class” thinking
- turning every subsystem into a formal GoF pattern where a simple value or pass is enough

The right rule is simple:

> a pattern is a vocabulary for recurring engineering problems, not a checklist to satisfy.

### Recommended ordering

For this project, the deliberate teaching order remains:

1. `Composite` for the AST
2. `Visitor` for traversals
3. nested `Scope` and symbol binding for lexical scoping
4. `DiagnosticBag` as the shared reporting structure
5. `Interpreter` as the runtime evaluation pass
6. `Lowering` and compiler passes as the next scale-up topic
7. `Strategy`, `Builder`, and VM/runtime structures only when the project actually grows

## Guardrail summary

MiniBASIC is not the product. The observable journey from source code to execution is the product.

The agent must maintain this educational focus at all times: clear compiler stages, small language scope, explicit representations, and tests that reveal the behavior of each phase.
