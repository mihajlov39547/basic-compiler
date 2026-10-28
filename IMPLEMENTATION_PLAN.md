# MiniBASIC Implementation Plan

This plan is the project roadmap for an educational compiler. It should be followed in order, with each phase introducing a small, inspectable compiler concept before moving deeper into the pipeline.

## Core teaching goal

The compiler should be deliberately observable. Every phase should be able to emit a human-readable artifact so students can inspect how source text changes as it moves through the compiler pipeline.

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
Lowering
   ↓
ir.txt
   ↓
Optimizer
   ↓
optimized-ir.txt
   ↓
Bytecode Generator
   ↓
program.mbc
   ↓
Disassembler
   ↓
bytecode.txt
   ↓
MiniBASIC VM
   ↓
program output
```

This is a better teaching model than a single source → execute step.

---

## Phase 0: Definition of the compiler

### Goal
Freeze the first language version and keep the implementation small.

### MiniBASIC v0.1 language scope

Supported types:

- Number
- String
- Boolean
- Array<T>
- Void (for function returns when needed)

Supported syntax:

```text
var a := 10
var name := "Marko"
var values := [1, 2, 3]

if a = 10 || a = 20 then
    print(name)
else
    print("No")
endif

while a < 20 do
    a := a + 1
endwhile
```

Future functions:

```text
function add(a: Number, b: Number): Number
    return a + b
endfunction

var result := add(10, 20)
```

### Guardrail

Do not add classes, inheritance, generics, modules, objects, or other large production features in v0.1.

---

## Phase 1: Repository structure and project skeleton

### Goal
Create the project scaffold and build infrastructure before implementing the lexer.

### Required project structure

```text
MiniBasic/
    Program.cs
    Text/
        SourceText.cs
        SourcePosition.cs
        SourceSpan.cs
    Diagnostics/
        Diagnostic.cs
        DiagnosticBag.cs
    Lexing/
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
    Binding/
        Symbol.cs
        VariableSymbol.cs
        FunctionSymbol.cs
        Scope.cs
        Binder.cs
        Types.cs
    Lowering/
        Lowerer.cs
    IR/
        ...
    Optimization/
        ...
    Bytecode/
        OpCode.cs
        Instruction.cs
        BytecodeModule.cs
        BytecodeEmitter.cs
        Disassembler.cs
    Runtime/
        Value.cs
        VirtualMachine.cs
        CallFrame.cs
    Dumping/
        TokenDumper.cs
        AstDumper.cs
        SymbolDumper.cs
    tests/
        MiniBasic.Tests/
```

### Tasks

- [ ] Create a modern C# .NET console app
- [ ] Create a test project
- [ ] Create folder structure and namespaces
- [ ] Add a simple CLI entry point
- [ ] Read a source file from disk
- [ ] Print compiler version information
- [ ] Return meaningful exit codes
- [ ] Keep compiler components independent from console I/O

### Deliverable

A working skeleton with no lexer/parser implementation yet.

---

## Phase 2: Source text and source positions

### Goal
Provide the infrastructure every compiler phase needs for diagnostics and source tracking.

### Tasks

- [ ] Implement `SourceText`
  - filename
  - full source text
  - line mappings
  - character access
- [ ] Implement `SourcePosition`
- [ ] Implement `SourceSpan`
- [ ] Every later object can carry a source span
- [ ] Add tests for line/column handling

### Example diagnostic

```text
program.bas:4:12
Unexpected character '@'.
4 | var x := 12 @ 5
                 ^
```

### Deliverable

Source positions and source text should exist before any parsing logic.

---

## Phase 3: Diagnostics

### Goal
Make compiler errors structured, informative, and educational.

### Tasks

- [ ] Implement `Diagnostic`
  - severity
  - error code
  - message
  - source span
- [ ] Implement `DiagnosticBag`
- [ ] Support multiple errors in one compilation
- [ ] Add CLI diagnostic formatting
- [ ] Add tests for diagnostic formatting

### Deliverable

A diagnostic system that is simple yet useful for teaching source locations.

---

## Phase 4: Lexer

### Goal
Turn source text into a token stream.

### Tasks

- [ ] Implement `TokenKind`
- [ ] Implement `Token`
  - kind
  - text
  - value
  - source span
- [ ] Implement basic lexer behavior
  - whitespace skipping
  - identifiers
  - keywords
  - numbers
- [ ] Add operator lexing
  - `+ - * /`
  - `:=`
  - `=`
  - `!=`
  - `< <=`
  - `> >=`
  - `&& ||`
- [ ] Add string literal lexing and escapes
- [ ] Add comments
- [ ] Add invalid character handling
- [ ] Add unterminated string handling
- [ ] Create first parse artifact: `tokens.txt`

### Example output

```text
0000 VAR        "var"           @ 1:1
0001 IDENTIFIER "a"             @ 1:5
0002 ASSIGN     ":="            @ 1:7
0003 NUMBER     "10"     10     @ 1:10
0004 EOF
```

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

---

## Phase 5: Parser and AST

### Goal
Turn tokens into a syntax tree.

### Tasks

- [ ] Define a base `SyntaxNode`
- [ ] Define `Expression` and `Statement`
- [ ] Create AST nodes for:
  - `NumberExpression`
  - `StringExpression`
  - `BooleanExpression`
  - `VariableExpression`
  - `BinaryExpression`
  - `UnaryExpression`
  - `VariableDeclaration`
  - `AssignmentStatement`
  - `PrintStatement`
  - `BlockStatement`
- [ ] Implement expression precedence correctly
- [ ] Implement statement parsing
  - variable declaration
  - assignment
  - print
  - block
  - if/else
  - while
- [ ] Retain source spans on AST nodes
- [ ] Create AST dump output: `ast.txt`

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

---

## Phase 6: Name resolution and symbol table

### Goal
Bind identifiers to declarations and detect undefined or duplicate names.

### Tasks

- [ ] Implement `Symbol`
- [ ] Implement `VariableSymbol`
- [ ] Implement `FunctionSymbol`
- [ ] Implement `Scope`
- [ ] Implement binder / resolver
- [ ] Track references to symbols
- [ ] Implement scope lookup by parent chain
- [ ] Detect duplicate declarations
- [ ] Detect unresolved names
- [ ] Produce `symbols.txt`

### Example

```text
GLOBAL SCOPE
  a : Variable #1
  message : Variable #2

IF BLOCK
  b : Variable #3
```

### Tests

- [ ] simple variable resolution
- [ ] duplicate declaration rejection
- [ ] undefined variable detection
- [ ] shadowing behavior

---

## Phase 7: Type checking

### Goal
Add static typing to expressions and declarations.

### Tasks

- [ ] Define type system
  - Number
  - String
  - Boolean
  - Array
  - Void
  - Error
- [ ] Infer variable types from initializers
- [ ] Check assignment compatibility
- [ ] Validate boolean conditions
- [ ] Validate array element access
- [ ] Produce `typed-ast.txt`
- [ ] Write explicit type errors with source spans

### Examples

- `10 + 20` => Number
- `"hello" - 20` => error
- `var x := 10` => `x : Number`

### Tests

- [ ] numeric type rules
- [ ] boolean type rules
- [ ] incompatible assignment error
- [ ] comparison types
- [ ] array element type validation

---

## Phase 8: Reference AST interpreter

### Goal
Provide the simplest working execution model before lowering to bytecode.

### Tasks

- [ ] Define runtime values
  - NumberValue
  - StringValue
  - BooleanValue
  - ArrayValue
  - VoidValue
- [ ] Implement `RuntimeEnvironment`
- [ ] Implement expression evaluation
  - literals
  - variables
  - arithmetic
  - comparisons
  - boolean operations
- [ ] Implement statements
  - assignment
  - print
  - if/else
  - while
- [ ] Create `interpret` command path

### Key rule

The AST interpreter is a reference implementation. Later, bytecode execution should produce the same result for the same program.

### Tests

- [ ] arithmetic evaluation
- [ ] variable assignment
- [ ] if execution
- [ ] while execution
- [ ] output correctness

---

## Phase 9: Lowering to IR

### Goal
Add an intermediate representation between typed AST and bytecode.

### Tasks

- [ ] Define IR model
  - `IRFunction`
  - `IRBlock`
  - `IRInstruction`
  - `IRValue`
  - `IRLabel`
- [ ] Lower expressions to instructions
- [ ] Lower if/else to branch instructions
- [ ] Lower while to branch instructions
- [ ] Produce `ir.txt`

### Example

```text
%0 = const 10
%1 = const 20
%2 = add %0, %1
store x, %2
```

### Tests

- [ ] expression lowering
- [ ] if lowering
- [ ] while lowering
- [ ] IR golden output

---

## Phase 10: Optimization passes

### Goal
Show how a compiler can transform IR while preserving semantics.

### Tasks

- [ ] Create a simple `ICompilerPass` interface or equivalent minimal abstraction
- [ ] Implement constant folding
- [ ] Implement constant propagation
- [ ] Implement dead code elimination
- [ ] Add pass verification steps
- [ ] Produce `optimized-ir.txt`

### Example

Before:

```text
%0 = const 2
%1 = const 3
%2 = add %0, %1
print %2
```

After:

```text
%2 = const 5
print %2
```

### Tests

- [ ] constant folding
- [ ] dead code elimination
- [ ] optimizer preserves program behavior

---

## Phase 11: Bytecode generation

### Goal
Compile the optimized IR into a simple, teachable instruction set.

### Tasks

- [ ] Define `OpCode` enum
- [ ] Define `Instruction`
- [ ] Define constant pool logic
- [ ] Emit load/store instructions
- [ ] Emit arithmetic instructions
- [ ] Emit comparison instructions
- [ ] Emit branch/jump instructions
- [ ] Emit print and return instructions
- [ ] Produce `bytecode.txt`
- [ ] Produce `program.mbc`

### Example bytecode

```text
PUSH_CONST 10
STORE_LOCAL 0
LOAD_LOCAL 0
PUSH_CONST 20
ADD
PRINT
HALT
```

### Tests

- [ ] expression bytecode output
- [ ] if translation
- [ ] loop translation
- [ ] bytecode golden tests

---

## Phase 12: Disassembler

### Goal
Make bytecode readable to humans.

### Tasks

- [ ] Implement disassembler for the bytecode format
- [ ] Output human-readable `bytecode.txt`
- [ ] Distinguish between raw bytecode and readable assembly output

### Deliverable

A program that can read `program.mbc` and print a readable representation.

---

## Phase 13: Virtual machine

### Goal
Execute the generated bytecode.

### Tasks

- [ ] Implement VM state
  - instruction pointer
  - operand stack
  - call stack
  - local variables
  - constant pool
- [ ] Implement fetch/decode/execute loop
- [ ] Implement arithmetic instructions
- [ ] Implement conditionals and jumps
- [ ] Implement print and return
- [ ] Run whole programs through the VM

### Tests

- [ ] stack operations
- [ ] arithmetic
- [ ] branching
- [ ] loops
- [ ] full program execution

---

## Phase 14: Functions, calls, and runtime frames

### Goal
Add the most important language feature needed for realistic programs.

### Tasks

- [ ] Add `FunctionSymbol`
- [ ] Add function declaration AST
- [ ] Add parameter binding
- [ ] Add return type validation
- [ ] Add call expressions
- [ ] Add `CallFrame`
- [ ] Implement recursion
- [ ] Validate function returns are present on all valid paths

### Tests

- [ ] function declaration parsing
- [ ] function call typing
- [ ] recursion
- [ ] runtime stack behavior

---

## Phase 15: Full compiler pipeline and verification

### Goal
Finish the compiler as a coherent educational system.

### Tasks

- [ ] Create a compiler driver that runs all stages in order
- [ ] Support commands like:
  - `lex`
  - `parse`
  - `check`
  - `interpret`
  - `compile`
  - `disasm`
  - `run`
- [ ] Support `--emit-all` mode to output all intermediate artifacts
- [ ] Add differential tests
  - AST interpreter output == VM output
- [ ] Add end-to-end tests

### Example output directory

```text
artifacts/
    factorial/
        00-source.bas
        01-tokens.txt
        02-ast.txt
        03-symbols.txt
        04-typed-ast.txt
        05-ir.txt
        06-optimized-ir.txt
        07-bytecode.txt
        factorial.mbc
```

### Final evaluation

The project should demonstrate that:

- the lexer works,
- the parser builds the AST,
- the resolver tracks names,
- the type checker validates semantics,
- the interpreter computes output,
- the bytecode compiler lowers to machine-like instructions,
- the VM executes them correctly,
- and the same program behaves consistently across runtimes.

---

## Important architectural recommendation

Introduce an explicit educational IR between typed AST and bytecode.

Without IR:

```text
AST -> Bytecode
```

works technically, but students lose a critical compiler concept:

A compiler progressively changes the representation of a program so the next stage becomes easier to solve.

With IR:

```text
AST
  -> Typed AST
  -> IR
  -> Optimized IR
  -> Bytecode
```

students can observe the program becoming simpler and more machine-oriented at each step.

This is a major educational advantage and should be treated as a first-class part of the project.

---

## Final rule

Do not add language features or infrastructure beyond the current educational stage unless the assignment explicitly calls for it.

The product is not the language itself. The product is the observable path from source code through every compiler stage to execution.
