# MiniBASIC Project Guardrails

## 1. Primary goal

MiniBASIC exists to teach how source code moves through the major stages of a programming-language implementation.

The goal is understanding this pipeline:

```text
Source Code
     ↓
Lexer
     ↓
Tokens
     ↓
Parser
     ↓
AST
     ↓
Name Resolution
     ↓
Type / Semantic Checking
     ↓
Interpreter
     ↓
Simple Bytecode
     ↓
Simple VM
```

Later demonstrations may show:

```text
IR → Assembly → Object → Linker → Executable
```

but these are primarily for understanding, not for building an industrial compiler.

## 2. Explicit non-goals

The AI agent must not turn MiniBASIC into a production programming language.

Unless an assignment explicitly requests them, do not introduce:

- generics
- classes/inheritance
- interfaces
- modules/package systems
- async/await
- concurrency
- sophisticated GC
- macros
- reflection
- exceptions
- advanced type inference
- overload resolution
- template systems
- LLVM backend
- native x86/ARM backend
- production linker
- production debugger
- LSP implementation
- IDE
- package manager
- optimizer framework comparable to LLVM
- JIT compiler
- sophisticated SSA infrastructure
- security sandbox
- production-grade error recovery

These may be explained or demonstrated, but should not silently become implementation requirements.

That distinction is important:

- Understand JIT: YES
- Build a JIT: NO
- Understand SSA: YES
- Small SSA exercise: MAYBE
- Build SSA framework: NO
- Understand linker: YES
- Use nm/readelf: YES
- Build a linker: NO
- Understand LLVM: YES
- Small LLVM demo: MAYBE
- Depend on LLVM: NO

## 3. Language feature freeze

This is probably the most important guardrail.

Once the MiniBASIC specification is established, the AI agent must not invent language features simply because they make implementation more elegant.

The core language is intentionally small.

### Values

- Number
- String
- Boolean
- Array

Potentially:

- Void

for functions.

### Variables

```text
var a := 10
var name := "Marko"
var numbers := [1, 2, 3]
```

### Operators

Approximately:

- + - * /
- = !=
- < <= > >=
- && ||
- !

### Conditions

```text
if a = 10 then
    print("yes")
else
    print("no")
endif
```

### Loops

```text
while a < 10
    a := a + 1
endwhile
```

### Functions

Keep them simple:

```text
function add(a: Number, b: Number): Number
    return a + b
endfunction
```

That is enough.

The AI agent should not say:

> "This would be cleaner if we introduced classes, generics and pattern matching."

Those are precisely the kinds of suggestions we want to suppress.

## 4. Complexity budget

Each component should demonstrate one important idea well.

### Lexer teaches

- characters
- lexemes
- tokens

Not sophisticated Unicode processing.

### Parser teaches

- tokens
- grammar
- AST

Not every parsing algorithm in one implementation.

### Resolver teaches

- identifier
- declaration

Not industrial module resolution.

### Type checker teaches

- expression
- type
- valid / invalid

Not Hindley-Milner or dependent types.

### Interpreter teaches

- AST → execution

Not production runtime engineering.

### Bytecode compiler teaches

- AST → simple instructions

Not an optimized VM instruction set.

### VM teaches

- instruction → machine state transition

Not HotSpot or V8.

### Optimization teaches

- program → equivalent but simpler program

One or two passes are enough.

## 5. Correctness before sophistication

The AI agent should prefer:

- simple + explicit + testable

over:

- clever + optimized + abstract

For example, prefer:

```csharp
switch (token.Kind)
{
    ...
}
```

if that makes the parser understandable.

Do not automatically introduce:

- reflection
- dependency injection
- code generation
- complex generic abstractions
- expression trees
- metaprogramming
- large framework hierarchies

just because C# supports them.

The students are learning compiler architecture, not enterprise C# architecture.

## 6. Avoid premature abstractions

This is particularly important for an AI coding agent.

AI agents tend to see three similar classes and immediately invent:

- `IProcessor<TInput,TOutput>`
- `AbstractCompilerPass<T>`
- `ICompilerStageFactory`
- `PipelineContext<TState>`
- `NodeProcessingStrategy<T>`

We explicitly do not want that.

The rule should be:

> Do not introduce an abstraction until the course has introduced the problem that motivates that abstraction.

For example:

- Week 3: AST
- Week 4 introduces several operations over AST
- Now we can discuss Visitor

Not:

- Week 1 → create generic visitor framework because we'll need it later

Pedagogically, that destroys the reason for teaching the pattern.

## 7. Design patterns must solve visible problems

AI should not pattern-shop.

Every pattern introduced needs the answer:

> What compiler problem does this solve?

Examples:

- Composite → AST is hierarchical
- Visitor → many operations traverse AST
- Symbol Table / Environment → lexical scope and name resolution
- Interpreter → execute AST semantics
- Pipeline → compiler consists of ordered transformations
- Pass → multiple independent IR analyses/transformations
- Factory → maybe useful for AST construction, but only when justified

If a pattern does not solve an educationally visible problem, do not introduce it.

## 8. Keep representations inspectable

Every stage should have an output students can see.

For:

```text
var a := 2 + 3
```

the course should be able to show:

```text
Source
var a := 2 + 3

Tokens
VAR
IDENTIFIER(a)
ASSIGN
NUMBER(2)
PLUS
NUMBER(3)

AST
VariableDeclaration(a)
└── Add
    ├── Number(2)
    └── Number(3)

Semantic information
a : Number

Bytecode
PUSH_CONST 2
PUSH_CONST 3
ADD
STORE 0

VM observation
stack:
[]

PUSH 2
[2]

PUSH 3
[2,3]

ADD
[5]

STORE a
[]
```

This is a major AI-agent guardrail:

> Never hide an educational stage behind a library if that stage is currently being taught.

## 9. Don't solve future lessons early

Suppose we are in the lexer week.

The AI agent should help build only:

```text
Source → Tokens
```

It should not decide:

> "While we're here, I created Parser, AST, Visitor, interpreter and bytecode infrastructure."

Even if doing so would be efficient industrially.

Course progression matters more than coding efficiency.

So introduce a rule:

> Implement only concepts already introduced in the course, unless minimal scaffolding is technically unavoidable.

## 10. Library guardrail

During the educational implementation:

### Allowed

Standard .NET functionality:

- `List<T>`
- `Dictionary<TKey,TValue>`
- `Stack<T>`
- `StringBuilder`
- `File`
- `Console`

Testing libraries are fine.

### Generally avoid for core exercises

Libraries that solve the concept currently being taught.

For example during lexer/parser weeks:

- ANTLR
- Irony
- Sprache
- Pidgin
- Roslyn parser

because then:

- Student task: learn how parsing works
- Library: does parsing
- Result: student learns an API

Later, absolutely show parser generators as comparative technology.

## 11. Error handling should remain educational

Avoid giant production diagnostic infrastructure.

Something like this is sufficient:

```csharp
public sealed record Diagnostic(
    string Message,
    SourceSpan Span
);
```

And:

```text
program.bas:3:17

var result := text - 10
                   ^

error: operator '-' cannot be applied
       to String and Number
```

That's enough to teach source locations and diagnostics.

## 12. Performance guardrail

Unless the exercise specifically concerns optimization:

> Never optimize for performance at the expense of conceptual clarity.

For 100 lines of MiniBASIC, it does not matter whether name lookup takes:

- 2 μs
- 20 μs

What matters is understanding:

```text
current scope
     ↓
parent scope
     ↓
parent scope
     ↓
global scope
```

Later the course can explicitly ask:

> Could this representation scale?

That becomes a discussion rather than accidental complexity.

## 13. Testing guardrail

Every component should have simple observable contracts.

For example:

- Lexer test: source → expected tokens
- Parser test: tokens → expected AST
- Resolver test: AST → expected symbols/errors
- Type checker test: AST → expected types/errors
- Interpreter test: program → expected value/output
- Bytecode test: AST → expected instructions
- VM test: bytecode → expected output

The AI agent should prefer adding a small test over adding sophisticated architecture.

## 14. AI must explain before modifying

For this educational repository, impose an unusual but useful rule:

Before making a non-trivial change, the AI response should state:

- Problem: What are we trying to demonstrate?
- Concept: Which compiler concept solves it?
- Representation: What data structure changes?
- Minimal change: What is the smallest implementation?
- Test: How do we know that it works?

For example:

```text
Problem:
Variables can only be accessed globally.

Concept:
Lexical scope.

Representation:
Each Scope contains a Dictionary<string, Symbol>
and an optional Parent.

Minimal change:
Introduce Scope and resolve identifiers by
walking the parent chain.

Test:
A variable declared inside an if block cannot
be accessed outside that block.
```

This keeps AI focused on pedagogy rather than feature generation.

## 15. Scope creep rule

This can be extremely simple:

> If a requested implementation needs more than one new major compiler concept that hasn't yet been taught, stop at the currently taught boundary and explain the remaining pipeline conceptually.

For example:

Student: "Make MiniBASIC call arbitrary C++ libraries."

AI should not suddenly build:

- FFI
- ABI marshaling
- native codegen
- dynamic loading
- type conversion layer

Instead:

```text
MiniBASIC
   ↓
Compiler
   ↓
native call representation
   ↓
ABI/FFI boundary
   ↓
C/C++
```

and explain that ABI/FFI is beyond the current project scope.

## 16. Explicit AI "Definition of Done"

For every component:

### Lexer DONE means

- valid source → correct token sequence
- invalid source → useful lexical diagnostic

Not production Unicode compliance.

### Parser DONE means

- valid tokens → correct AST
- invalid syntax → useful parser diagnostic

Not industrial recovery.

### Resolver DONE means

- identifier → correct declaration
- bad identifier → diagnostic

### Type checker DONE means

- valid operations → type
- invalid operations → diagnostic

### Interpreter DONE means

- valid AST → expected output

### Bytecode compiler DONE means

- AST → correct educational bytecode

### VM DONE means

- bytecode → same observable result as interpreter

That last one is particularly valuable:

```text
              AST
             /   \
            /     \
   Interpreter   Compiler
       │            │
       │         Bytecode
       │            │
       │            VM
       │            │
       └──── result ┘
```

If both produce the same result on the test suite, students have a powerful correctness oracle.

## 17. Ready-to-use AI agent instruction

Place something close to the following in the project's AI instructions:

```text
You are assisting with an educational MiniBASIC compiler project.

PRIMARY PURPOSE

The purpose of this repository is to teach how source code passes
through the major stages of a programming language implementation.

The educational pipeline is:

Source
→ Lexer
→ Tokens
→ Parser
→ AST
→ Name Resolution
→ Type/Semantic Analysis
→ AST Interpreter
→ Simple Bytecode
→ Simple Virtual Machine

Advanced compiler concepts may be demonstrated separately but must
not make the core implementation unnecessarily complex.

SCOPE RULES

1. Keep MiniBASIC intentionally small.
2. Do not add language features unless explicitly requested.
3. Do not turn the project into a production compiler.
4. Prefer clarity over performance.
5. Prefer explicit code over generic frameworks.
6. Do not introduce abstractions before they solve a demonstrated
   problem.
7. Do not implement concepts scheduled for later stages unless
   explicitly requested.
8. Do not replace educational compiler stages with libraries that
   perform the concept being taught.
9. Keep every compiler representation inspectable and printable.
10. Every non-trivial change should include a small test.
11. Explain which compiler concept a proposed change demonstrates.
12. Prefer the smallest implementation that demonstrates that concept.

NON-GOALS

Do not introduce unless explicitly required:

- classes or inheritance in MiniBASIC
- generics in MiniBASIC
- modules or package managers
- macros
- concurrency or async
- reflection
- sophisticated garbage collection
- advanced type inference
- LLVM as a required dependency
- native x86/ARM backend
- production linker
- JIT compiler
- production debugger
- IDE or LSP
- sophisticated optimization framework

DESIGN PRINCIPLE

For every proposed feature or architectural change ask:

"What compiler concept does this help the student understand?"

If there is no clear educational answer, do not add it.

When multiple solutions exist, prefer the one that makes the compiler
pipeline and intermediate representations easiest for students to see.

Do not proactively expand the project scope.
```

MiniBASIC is not the product. The observable journey from source code to execution is the product.
