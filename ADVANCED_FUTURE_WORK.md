# Advanced / Future Work for MiniBASIC

This document contains ideas that are valuable for the project, but they are intentionally not part of the initial v0.1 roadmap.

The goal is to keep the main implementation plan focused, simple, and teachable. Advanced topics can be introduced later as separate learning milestones.

---

## 1. Arrays and collections

Possible future work:

- `Array<T>` type
- literal array syntax such as `[1, 2, 3]`
- index expressions like `values[0]`
- bounds checking
- array assignment and mutation

Why this is advanced:

- it requires deeper type rules,
- richer AST nodes,
- additional runtime memory model,
- and more semantic checks.

---

## 2. Functions and procedural abstraction

Possible future work:

- function declarations
- parameter binding
- return statements
- call expressions
- recursion
- call frames
- stack-based evaluation

Why this is advanced:

- it requires scope and symbol management beyond simple variables,
- function calls affect runtime models,
- and it introduces the idea of execution frames.

---

## 3. Intermediate representation (IR)

Possible future work:

- `IRFunction`
- `IRBlock`
- `IRInstruction`
- `IRLabel`
- lowering from typed AST into stack or SSA-like instructions

Why this is valuable:

- IR makes compiler stages more explicit,
- it separates analysis from code generation,
- it simplifies optimization.

Why it is not in the core plan:

- it adds a second representation layer that students must understand,
- and it is conceptually harder than the initial parser and runtime pipeline.

---

## 4. Optimization passes

Possible future work:

- constant folding
- constant propagation
- dead code elimination
- simple pass pipeline

Why it matters:

- optimization is one of the most important compiler ideas,
- but it is best introduced after the base compiler works.

---

## 5. Bytecode and virtual machine

Possible future work:

- `OpCode` enum
- instruction encoding
- constant pool
- operand stack
- local variables
- branch instructions
- print and halt instructions
- disassembler

Why this is valuable:

- it connects the compiler to a machine-like execution model,
- it demonstrates how high-level code becomes runnable instructions.

Why it is not in the core plan:

- it adds a new layer of semantics,
- the bytecode model can distract from the primary educational goal when the language itself is still being learned.

---

## 6. More advanced type features

Possible future work:

- type inference improvements
- union types
- nullable values
- explicit conversions
- array element type validation
- user-defined record-like data structures

These are useful, but they are not necessary for the first compiler milestone.

---

## 7. Better tooling and feedback

Possible future work:

- pretty-printer for AST
- code formatting
- error recovery in the parser
- source-aware formatting for diagnostics
- incremental compilation
- AST / symbol dump exporters

These are good quality-of-life improvements, but they are not required for the initial curriculum.

---

## 8. Full compiler pipeline experiments

Possible future work:

- compare AST interpreter output with VM output
- emit all intermediate artifacts to a folder
- build a command-line driver with stages like `lex`, `parse`, `check`, `run`
- produce artifact snapshots for educational demos

These are excellent capstone tasks once the core language is working.

---

## Suggested order for future work

Once the v0.1 MVP is complete, the natural next additions are:

1. arrays
2. functions
3. better symbol and type modeling
4. IR and lowering
5. optimization
6. bytecode generation and VM
7. advanced tooling and end-to-end compiler demos

This sequence keeps the learning curve steady and avoids overwhelming students prematurely.

---

## Final rule

Advanced compiler concepts should only be introduced when students can already explain the previous stage clearly.

The main goal of the project is to teach the pipeline, not to jump straight into an industrial compiler architecture.
