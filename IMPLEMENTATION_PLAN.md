# MiniBASIC Implementation Plan

This is the minimal, student-friendly roadmap for the first version of the project. The goal is to keep the compiler small, observable, and teachable.

## Core teaching goal

The compiler should be built in small, inspectable stages. Every phase should produce a readable artifact so students can see how their source program changes as it moves through the pipeline.

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

---

## Phase 2: Source text and source positions

### Goal
Give every compiler stage a reliable way to track source positions.

### Tasks

- [ ] Implement `SourceText`
  - filename
  - full source text
  - line mapping
  - character indexing
- [ ] Implement `SourcePosition`
- [ ] Implement `SourceSpan`
- [ ] Add tests for line/column calculations

### Deliverable

The compiler can report where errors happened in source files.

---

## Phase 3: Diagnostics

### Goal
Support helpful and structured compiler errors.

### Tasks

- [ ] Implement `Diagnostic`
  - severity
  - message
  - source span
- [ ] Implement `DiagnosticBag`
- [ ] Support multiple errors in one compilation
- [ ] Format diagnostics for console output
- [ ] Add tests for diagnostic formatting

### Deliverable

A simple diagnostic system with precise source locations.

---

## Phase 4: Lexer

### Goal
Turn source text into tokens.

### Tasks

- [ ] Implement `TokenKind`
- [ ] Implement `Token`
  - kind
  - text
  - value
  - source span
- [ ] Skip whitespace
- [ ] Recognize identifiers and keywords
- [ ] Recognize numbers and strings
- [ ] Recognize operators
  - `+ - * /`
  - `:=`
  - `=`
  - `!=`
  - `< <=`
  - `> >=`
  - `&& ||`
- [ ] Handle invalid characters
- [ ] Handle unterminated strings
- [ ] Emit `EOF`

### Tests

- [ ] single keyword
- [ ] identifier
- [ ] integer number
- [ ] decimal number
- [ ] string
- [ ] whitespace handling
- [ ] operators
- [ ] multiple lines
- [ ] source positions
- [ ] invalid character diagnostic
- [ ] unterminated string diagnostic

### Deliverable

A working token stream for the language.

---

## Phase 5: Parser and AST

### Goal
Build a syntax tree from the token stream.

### Tasks

- [ ] Define `SyntaxNode`
- [ ] Define `Expression` and `Statement`
- [ ] Add AST nodes for:
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
- [ ] Respect expression precedence
- [ ] Keep spans on AST nodes
- [ ] Dump AST to a readable text form

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

- [ ] arithmetic precedence
- [ ] assignment
- [ ] variable declaration
- [ ] if statement
- [ ] while loop
- [ ] AST golden tests

### Deliverable

A parser that builds a meaningful syntax tree from valid MiniBASIC code.

---

## Phase 6: Name resolution and symbol table

### Goal
Bind names to declarations and catch duplicate or undefined variables.

### Tasks

- [ ] Implement `Symbol`
- [ ] Implement `VariableSymbol`
- [ ] Implement `Scope`
- [ ] Implement binder / resolver
- [ ] Ensure nested scopes resolve properly
- [ ] Detect duplicate declarations
- [ ] Detect unresolved names
- [ ] Produce a simple symbol dump

### Tests

- [ ] resolve simple variable
- [ ] duplicate declaration rejection
- [ ] undefined variable detection
- [ ] nested scope behavior

### Deliverable

The compiler can determine which names are valid and where they are declared.

---

## Phase 7: Type checking

### Goal
Validate variable and expression types before execution.

### Tasks

- [ ] Define type system
  - Number
  - String
  - Boolean
  - Error
- [ ] Infer types from declarations and literals
- [ ] Check assignments
- [ ] Validate boolean conditions
- [ ] Report invalid type operations

### Examples

- `10 + 20` => Number
- `"hello" + " world"` => String
- `if "x" then` => error

### Tests

- [ ] numeric type rules
- [ ] boolean type rules
- [ ] incompatible assignment error
- [ ] comparison semantics

### Deliverable

The compiler catches type mismatches before execution.

---

## Phase 8: Reference interpreter

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

### Tests

- [ ] sample program executes successfully
- [ ] compiler emits tokens, AST, and symbols
- [ ] diagnostics print correctly

### Deliverable

The project behaves like a teaching compiler front end with a working runtime.

---

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
