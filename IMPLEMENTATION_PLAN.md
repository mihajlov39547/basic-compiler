# MiniBASIC Implementation Plan

This is the minimal, student-friendly roadmap for the first version of the project. The goal is to keep the compiler small, observable, and teachable.

## Core teaching goal

The compiler should be built in small, inspectable stages. Every phase should produce a readable artifact so students can see how their source program changes as it moves through the pipeline.

## Activities overview

Each phase below is intentionally written as a teaching activity. The goal is not just to implement features, but to make the compiler progression easy to explain and easy to inspect.

- Activity 1: Define the language and keep it small
- Activity 2: Create the project skeleton and repository structure
- Activity 3: Track source text and positions reliably
- Activity 4: Emit clear compiler diagnostics
- Activity 5: Convert source code into tokens
- Activity 6: Build a syntax tree from those tokens
- Activity 7: Bind names and validate symbol usage
- Activity 8: Check types before execution
- Activity 9: Execute the program with a reference interpreter
- Activity 10: Package the work into a readable CLI and validation flow

## Architecture choices

### Primary pattern: Pipeline / pass-based processing

// Activity: Keep the compiler as a straightforward pipeline of transformations.

This is the main design pattern for the project and it is already in use.

The compiler follows a direct sequence of transformations:

```text
source text
   ↓
lexer
   ↓
tokens
   ↓
parser
   ↓
AST
   ↓
resolver / binder
   ↓
type checker
   ↓
interpreter
```

This is a pipeline, not a Chain of Responsibility. Each stage has a clear input/output contract and the flow is intentionally simple and easy to teach.

### Other patterns to use when appropriate

// Activity: Add reusable patterns only when they make the compiler easier to teach and maintain.

- [ ] Visitor pattern for AST traversal and evaluation
  - useful for pretty-printing, interpretation, and type checking
- [ ] Strategy pattern for CLI commands
  - useful for `lex`, `parse`, `check`, and `interpret` command modes
- [ ] Factory pattern for AST / node creation when complexity grows
  - useful for parser construction and node instantiation without hardcoding object creation

These patterns are optional and should be introduced only when they add clarity to a phase, not before the project needs them.

Target progression:

```text
program.bas
   ↓
Lexer
   ↓
tokens.txt
   ↓
Parser
   ↓
ast.txt
   ↓
Name Resolver
   ↓
symbols.txt
   ↓
Type Checker
   ↓
typed-ast.txt
   ↓
Reference Interpreter
   ↓
program output
```

This project is about learning compiler concepts, not building a production language runtime.

---

## MiniBASIC v0.1 language scope

The first version should stay intentionally small.

### Supported types

- Number
- String
- Boolean

### Supported features

```text
var a := 10
var name := "Marko"

if a = 10 || a = 20 then
    print(name)
else
    print("No")
endif

while a < 20 do
    a := a + 1
endwhile
```

### Explicit non-goals for v0.1

Do not add these in the main implementation plan:

- arrays as a first-class feature
- functions and recursion
- modules or namespaces
- generics
- classes or objects
- optimizer passes
- IR lowering
- bytecode VM architecture
- advanced runtime machinery

These belong in a separate future-work document.

---

## Phase 0: Definition of the compiler

// Activity: Freeze the language so the rest of the compiler has a clear target.

### Goal
Freeze a small first-language version and keep the scope stable.

### Tasks

- [x] Define the supported syntax for v0.1
- [x] Define the supported types
- [x] Define the accepted statements and expressions
- [x] Keep feature scope intentionally small
- [x] Write examples that are easy to understand

### Deliverable

A clear v0.1 language definition that the rest of the project can implement.

### Status

Phase 0 is complete for the MVP scope. The project now has a single canonical v0.1 definition that intentionally omits arrays, function declarations, and other advanced features until later work.

---

## Phase 1: Repository structure and project skeleton

// Activity: Set up the project so each later phase has a stable place to live.

### Goal
Create a clean project layout before implementing compiler features.

### Required project structure

```text
BasicCompiler/
    BasicCompiler.csproj
    Program.cs
    Text/
        SourceText.cs
        SourcePosition.cs
        SourceSpan.cs
    Diagnostics/
        Diagnostic.cs
        DiagnosticBag.cs
    Lexer/
        Token.cs
        TokenKind.cs
        Lexer.cs
    Syntax/
        Parser.cs
        AST/
            SyntaxNode.cs
            Expression.cs
            Statement.cs
            ...
    Semantics/
        Symbol.cs
        Scope.cs
        Binder.cs
    Runtime/
        Value.cs
        Interpreter.cs
    Tests/
        BasicCompiler.Tests/
```

### Tasks

- [x] Create the .NET console app
- [x] Create the test project
- [x] Create the folder structure
- [x] Add a basic CLI entry point
- [x] Read a source file from disk
- [x] Print a compiler banner
- [x] Keep logic separate from console output

### Deliverable

A working project scaffold with no language logic yet.

### Status

Phase 1 is complete for the current repo. The implementation follows the intended skeleton closely, with the main difference being that the project currently keeps `SourceSpan` under `Diagnostics` instead of a dedicated `Text` folder. That is acceptable for an initial scaffold and does not block the next phase.

The project already reflects the pipeline architecture in practice: the entry point calls the lexer, parser, and interpreter in order. This pattern is the correct primary design for the compiler and should remain the default structure for the first milestones.

---

## Phase 2: Source text and source positions

// Activity: Teach the compiler how to locate errors and track positions in source code.

### Goal
Give every compiler stage a reliable way to track source positions.

### Tasks

- [x] Implement `SourceText`
  - filename
  - full source text
  - line mapping
  - character indexing
- [x] Implement `SourcePosition`
- [x] Implement `SourceSpan`
- [x] Add tests for line/column calculations

### Deliverable

The compiler can report where errors happened in source files.

### Status

Phase 2 is complete for the current MVP. The project now has a minimal `SourceText`/`SourcePosition` layer that tracks line starts and character offsets, and a `SourceSpan` structure that can anchor later diagnostics to source locations.

---

## Phase 3: Diagnostics

// Activity: Turn raw errors into useful messages with meaningful locations.

### Goal
Support helpful and structured compiler errors.

### Tasks

- [x] Implement `Diagnostic`
  - severity
  - message
  - source span
- [x] Implement `DiagnosticBag`
- [x] Support multiple errors in one compilation
- [x] Format diagnostics for console output
- [x] Add tests for diagnostic formatting

### Deliverable

A simple diagnostic system with precise source locations.

### Status

Phase 3 is complete for the MVP. The project now has a minimal diagnostic accumulator with severity, span, and formatted output suitable for teaching and later compiler front-end error reporting.

---

## Phase 4: Lexer

// Activity: Convert text into tokens so the parser can reason about the program.

### Goal
Turn source text into tokens.

### Tasks

- [x] Implement `TokenKind`
- [x] Implement `Token`
  - kind
  - text
  - position
- [x] Skip whitespace
- [x] Recognize identifiers and keywords
- [x] Recognize numbers and strings
- [x] Recognize operators
  - `+ - * /`
  - `:=`
  - `=`
  - `!=`
  - `< <=`
  - `> >=`
  - `&& ||`
- [x] Handle invalid characters
- [x] Handle unterminated strings
- [x] Emit `EOF`

### Tests

- [x] single keyword
- [x] identifier
- [x] integer number
- [x] decimal number
- [x] string
- [x] whitespace handling
- [x] operators
- [x] multiple lines
- [x] source positions
- [x] invalid character diagnostic
- [x] unterminated string diagnostic

### Deliverable

A working token stream for the language.

### Status

Phase 4 is complete for the current MVP. The lexer is implemented and validated in the project, including keywords, operators, strings, numbers, whitespace skipping, invalid character handling, and EOF generation. The current implementation keeps token positions as a simple integer offset instead of a full `SourceSpan` object on each token, which is acceptable for this educational compiler and does not block the parser or AST phases.

---

## Phase 5: Parser and AST

// Activity: Build a syntax tree so the language structure becomes explicit and inspectable.

### Goal
Build a syntax tree from the token stream.

### Tasks

- [x] Define `Expression` and `Statement`
- [x] Add AST nodes for:
  - literal expressions
  - variable expressions
  - binary expressions
  - unary expressions
  - assignment
  - variable declaration
  - block
  - print
  - if/else
  - while
- [x] Respect expression precedence
- [x] Dump AST to a readable text form
- [x] Keep the parser and AST intentionally simple and educational
- [x] Consider `Visitor` pattern for later traversal work
  - `PrettyPrintVisitor`
  - `InterpreterVisitor`
  - `TypeCheckVisitor`

### Design note

The project currently uses `Expression` and `Statement` as the AST base types instead of a separate `SyntaxNode` root. That is a valid simplification for a teaching compiler and keeps the design smaller without losing clarity. The `Visitor` pattern remains a future improvement when more passes are added, but it is not required for the current MVP.

### Parsing precedence

- logical OR
- logical AND
- equality
- comparison
- additive
- multiplicative
- unary
- primary

### Tests

- [x] arithmetic precedence
- [x] assignment
- [x] variable declaration
- [x] if statement
- [x] while loop
- [x] AST dump / readability checks

### Deliverable

A parser that builds a meaningful syntax tree from valid MiniBASIC code.

### Status

Phase 5 is complete for the current MVP. The project has a working parser and AST pipeline, including precedence-aware expression parsing, assignment and declaration handling, conditional and loop statements, and a clear text-based AST dump for educational inspection. The current implementation intentionally avoids more advanced compiler abstractions like a separate `SyntaxNode` hierarchy or a full visitor framework until the project needs them.

---

## Phase 6: Name resolution and symbol table

// Activity: Bind names to declarations so the compiler knows what each identifier refers to.

### Goal
Bind names to declarations and catch duplicate or undefined variables.

### Tasks

- [x] Implement `Symbol`
- [x] Implement `VariableSymbol`
  - this is a small, educational refinement that keeps the symbol table explicit without adding extra complexity
- [x] Implement `Scope`
- [x] Implement binder / resolver
- [x] Ensure nested scopes resolve properly
- [x] Detect duplicate declarations
- [x] Detect unresolved names
- [x] Produce a simple symbol dump

### Tests

- [x] resolve simple variable
- [x] duplicate declaration rejection
- [x] undefined variable detection
- [x] nested scope behavior

### Deliverable

The compiler can determine which names are valid and where they are declared.

### Status

Phase 6 is complete for the MVP. The project now has a working `Scope`, `Symbol`, and `NameResolver` that handle the core educational scenario: duplicate declarations, unresolved names, nested block scopes, and a simple symbol dump for inspection. The optional `VariableSymbol` refinement remains a future improvement if the project later grows beyond the minimalist teaching runtime.

---

## Phase 7: Type checking

// Activity: Validate variable and expression types before execution.

### Goal
Validate variable and expression types before execution.

### Tasks

- [x] Define type system
  - Number
  - String
  - Boolean
  - Error represented as diagnostics / unknown in the MVP
- [x] Infer types from declarations and literals
- [x] Check assignments
- [x] Validate boolean conditions
- [x] Report invalid type operations

### Examples

- `10 + 20` => Number
- `"hello" + " world"` => String
- `if "x" then` => error

### Tests

- [x] numeric type rules
- [x] boolean type rules
- [x] incompatible assignment error
- [x] comparison semantics

### Deliverable

The compiler catches type mismatches before execution.

### Status

Phase 7 is complete for the MVP. The project has a working `TypeChecker` that infers basic types (`number`, `string`, `bool`), validates assignments, checks `if`/`while` conditions, and emits diagnostics for invalid arithmetic, comparison, and boolean operations. The implementation keeps the model lightweight and educational by using string-based type names rather than introducing a full compiler `Type` abstraction too early.

---

## Phase 8: Reference interpreter

// Activity: Run the language in the simplest possible way to prove the pipeline works.

### Goal
Provide the simplest working execution model.

### Tasks

- [ ] Define runtime values
  - NumberValue
  - StringValue
  - BooleanValue
- [ ] Implement a runtime environment
- [ ] Evaluate expressions
  - literals
  - variables
  - arithmetic
  - comparisons
  - boolean logic
- [ ] Execute statements
  - assignment
  - print
  - if/else
  - while
- [ ] Add a command path like `interpret`

### Key rule

This interpreter is a teaching reference. It should be easy to read and easy to debug.

### Tests

- [ ] arithmetic evaluation
- [ ] variable assignment
- [ ] if execution
- [ ] while loop execution
- [ ] output correctness

### Deliverable

A working compiler pipeline from source to runtime result.

---

## Phase 9: CLI and end-to-end validation

// Activity: Package the compiler as a readable tool that can validate the full workflow.

### Goal
Turn the project into a usable compiler tool.

### Tasks

- [ ] Add commands such as:
  - `lex`
  - `parse`
  - `check`
  - `interpret`
- [ ] Support reading a source file and outputting each stage
- [ ] Add end-to-end validation against sample programs
- [ ] Confirm each stage produces inspectable output
- [ ] Introduce `Strategy` pattern for CLI command dispatch
  - `LexCommand`
  - `ParseCommand`
  - `CheckCommand`
  - `InterpretCommand`

### Design note

The CLI is a natural place to apply the Strategy pattern once multiple commands exist. This keeps command logic separate and avoids a large `if/else` or `switch` in the entry point.

### Tests

- [ ] sample program executes successfully
- [ ] compiler emits tokens, AST, and symbols
- [ ] diagnostics print correctly

### Deliverable

The project behaves like a teaching compiler front end with a working runtime.

---

## Factory pattern and future extensibility

// Activity: Keep future patterns in mind without introducing them too early.

The project should not over-engineer early, but it should keep a few reusable patterns in mind for future phases.

- [ ] Factory pattern for AST/object construction when parsing becomes more elaborate
- [ ] Keep pipeline as the default architecture and do not introduce unnecessary abstraction layers too early
- [ ] Add new patterns only when the next phase actually benefits from them

## Final MVP success criteria

The project is complete enough when all of the following work:

- source text can be loaded,
- tokens are produced correctly,
- AST is generated correctly,
- names are resolved,
- type rules are enforced,
- programs run through an interpreter,
- and outputs are shown in a way students can inspect.

This is the core product of the project.

---

## Important rule

The main roadmap must stay intentionally small.

Do not add advanced compiler concepts to the core implementation plan unless they are needed to teach a specific stage. Any larger architecture should live in a separate future-work document.
