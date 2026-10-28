# Advanced / Future Work for MiniBASIC

This document collects ideas that are valuable for the project, but intentionally not part of the initial educational path.

The core project should remain small, explicit, and inspectable. Advanced topics are best introduced only after students can explain the previous stage clearly.

---

## 1. Lowering and desugaring

Possible future work:

- transform `for` loops into while-style loops
- desugar `if`/`else` constructs into branch-based control flow
- lower user-friendly syntax into a simpler internal form
- introduce a small IR layer for later passes

Why this is important:

- lowering is a central compiler transformation concept,
- it shows that front-end syntax is often rewritten into a uniform machine-friendly form,
- and it prepares the project for optimization and code generation.

This is one of the first genuinely advanced topics worth teaching after the base compiler works.

---

## 2. Compiler passes and optimization pipeline

Possible future work:

- constant folding
- constant propagation
- dead code elimination
- verification passes that check invariants
- a pass pipeline with explicit input/output contracts

Why this matters:

- real compiler work is usually a sequence of passes,
- not one monolithic block of logic,
- and the project already follows a pass-based pipeline in spirit.

The teaching model should stay:

```text
AST
  ↓
resolved and typed AST
  ↓
IR or lowered form
  ↓
optimization passes
  ↓
execution or code generation
```

---

## 3. Arrays and collections

Possible future work:

- `array` type support
- literal arrays such as `[1, 2, 3]`
- index expressions like `values[0]`
- bounds checking
- mutation and assignment semantics

Why this is advanced:

- arrays require richer type rules,
- a more expressive AST,
- and a runtime storage model beyond simple scalar variables.

This is a natural next step after the project has a stable symbol table and type checker.

---

## 4. Functions and procedural abstraction

Possible future work:

- function declarations
- parameters and returns
- recursive calls
- call expressions
- call frames and runtime stack behavior

Why this is advanced:

- function binding introduces more complex scope management,
- runtime evaluation requires activation records,
- and the program model becomes more realistic.

This is an excellent follow-up to lexical scoping and symbol binding.

---

## 5. Bytecode and virtual machine

Possible future work:

- `OpCode` enum
- constant pool
- stack-based execution
- local variable storage
- jumps and branches
- simple VM instruction set
- disassembler or debugging output

Why this is valuable:

- it shows how a high-level language becomes executable instructions,
- and it connects the compiler to a more machine-like runtime.

Why it is not in the core plan:

- it adds another substantial execution layer,
- and it can distract from the original educational goal if introduced too early.

---

## 6. Backend strategy and optimization modes

Possible future work:

- interpreter backend
- bytecode backend
- optimized backend
- explicit `OptimizationStrategy` object

Why it matters:

- once the compiler has multiple execution modes, the selection logic becomes a real design concern,
- and `Strategy` is a clean fit for that problem.

This is a meaningful pattern for a later stage, not the first stage of the course.

---

## 7. Better tooling and feedback

Possible future work:

- pretty-printed AST output
- richer diagnostics with source context
- parser recovery and error continuation
- artifact snapshots for each compiler phase
- incremental compilation support

Why it belongs later:

- these features improve the developer experience,
- but they are not required to understand the compiler itself.

---

## 8. Patterns to teach later, not earlier

The project should not force every idea into a formal GoF pattern. The right advanced topics are:

- lowering / desugaring
- compiler pass composition
- strategy for backend or optimization choice
- emitter / builder for bytecode output
- runtime state and stack frames

These are meaningful additions to the course, but they should be introduced at the point when the underlying compiler concepts are already understood.

---

## Suggested order for future work

Once the base compiler is stable, the most natural next steps are:

1. lowering and IR
2. optimization passes
3. arrays
4. functions and call frames
5. bytecode and VM
6. backend strategy and tooling improvements

This sequence keeps the learning curve steady and avoids overwhelming students before they understand the main pipeline.

---

## Final rule

Advanced compiler concepts should be introduced only when the earlier stages are already clear.

The project’s real product is the educational journey from source text to execution, not a premature jump into a full industrial compiler architecture.
