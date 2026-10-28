# Building a Tiny Language: What I Learned from Writing My Own Parser

## A practical guide to turning a few grammar rules into a working mini-language

I still remember the first time I looked at a compiler and thought: this is impossible to understand.

It looked like a black box. Source code went in. Executable output came out. Somewhere in between, a mysterious set of tools and rules turned text into behavior. I wanted to know what was really happening.

So I built a tiny language.

Not a full production language. Not a huge academic project. Just a small, teaching-friendly language with variables, conditionals, loops, and expressions. Enough to make the compiler pipeline visible. Enough to understand what a lexer, parser, AST, and interpreter actually do.

What surprised me most was that the hard part was not writing the code. The hard part was learning to think in layers.

A compiler is not one big magical algorithm. It is a sequence of deliberate transformations:

- source text becomes tokens
- tokens become a syntax tree
- the syntax tree becomes meaning
- meaning becomes runtime behavior

That simple idea changed how I understood programming languages.

---

## Why build a mini-language in the first place?

Most developers never see the intermediate stages of a compiler. They work with a final language, not the machinery behind it.

A mini-language exposes that machinery clearly.

It lets you see:

- how text is tokenized
- how operators create precedence rules
- how a parse tree expresses structure
- how names are resolved and bound
- how type rules prevent invalid programs
- how execution uses the language’s meaning

This is not just an academic exercise. It is a practical way to understand the foundations of language design and execution.

If you want to understand how real compilers work, building a small one is one of the clearest paths.

---

## Lesson 1: Start with the grammar, not the code

The first mistake I made was jumping to implementation before I had a clear language definition.

I wanted to write parser code immediately. That was the wrong order.

Before writing anything, I had to answer a few fundamental questions:

- What are the language keywords?
- What is a variable declaration?
- What does an assignment look like?
- What operators are allowed?
- What is the precedence of arithmetic and comparisons?
- What is the syntax of loops and conditionals?

Once those questions were answered, the grammar became the blueprint.

A simple version looked roughly like this:

```text
program      := statement*
statement    := varDecl | assignment | print | ifStmt | whileStmt
varDecl      := "var" identifier ":=" expression
assignment   := identifier ":=" expression
expression   := comparison
comparison   := additive (("==" | "!=" | "<" | "<=" | ">" | ">=") additive)?
additive     := multiplicative (("+" | "-") multiplicative)*
multiplicative := unary (("*" | "/") unary)*
```

This is not just syntax. It is design.

The grammar tells you what kind of program the compiler is allowed to understand. It prevents accidental complexity and reduces later confusion.

The lesson is simple:

> Define the language before writing parser code.

And keep it small.

---

## Lesson 2: The lexer is not “just scanning”

I used to think lexing was a trivial step: read characters, split them, move on.

It is not trivial.

The lexer is where the language becomes structured. It decides whether a piece of text is:

- a keyword
- an identifier
- a number
- a string
- punctuation
- an operator

It also handles cases that look small but are extremely important:

- whitespace skipping
- line and column tracking
- operators such as := versus =
- string literals with escaping
- invalid characters
- unterminated input

A messy lexer creates a messy parser.

If you define tokens incorrectly, every later step becomes harder to debug.

A good lexer should be deterministic and easy to test. Each token should carry enough information to be useful later:

- kind
- text
- source position
- semantic value when relevant

This matters because debugging a compiler is often a process of tracing a misread token back to a source location.

The lesson is practical:

> The lexer is the front door of a compiler. If it is messy, the entire pipeline suffers.

---

## Lesson 3: ASTs are where structure becomes visible

Once the lexer works, the parser turns tokens into an AST.

This is one of the most satisfying milestones in compiler development.

You start with plain text and end up with a tree that reflects the program’s structure.

For example:

```text
1 + 2 * 3
```

is not just a string. It becomes a tree that preserves precedence:

```text
Add
├── Literal(1)
└── Multiply
    ├── Literal(2)
    └── Literal(3)
```

That is the core idea of an AST: it captures the meaning of the code in a form that later passes can inspect and transform.

Without an AST, the compiler has no clear structure to work with.

The AST lets us do important work later:

- name resolution
- type checking
- interpretation
- optimization
- code generation

The lesson is that the AST is not “extra structure.” It is the compiler’s memory of the program.

---

## Lesson 4: Grammar is design, not just syntax

This was one of the biggest lessons for me.

A language definition is not just a list of rules. It is a design decision.

If you allow too many features too early, your parser becomes brittle. If you separate declarations from assignments and keep the language simple, everything becomes easier.

I learned that small design decisions ripple through the entire compiler.

For example:

- Should declarations and assignments be separate grammar rules?
- Should expressions allow side effects?
- Should conditional syntax be explicit or compact?
- Should loops have begin/end markers or braces?

These decisions determine parser complexity, semantic checking, and runtime behavior.

The best approach is to keep the language narrow at first and only add features once the base model is stable.

This is a discipline worth learning early.

---

## Lesson 5: Diagnostics are part of the compiler, not an afterthought

When I first built the parser, I focused on successful programs. I treated errors as something to fix later.

That was a mistake.

A compiler is not just a code converter. It is also a system for identifying invalid input and explaining why it is invalid.

Good diagnostics should tell the programmer:

- what went wrong
- where it happened
- why it matters

Examples include:

- unexpected token
- missing expression
- undefined name
- duplicate declaration
- invalid assignment type

A parser without good diagnostics is frustrating to use. A compiler without informative diagnostics is hard to trust.

This taught me something important:

> Errors are not a side feature. They are part of the language contract.

---

## Lesson 6: Scope and binding are where semantics begin

Once the AST exists, the compiler asks a more meaningful question:

What does each identifier refer to?

This is the beginning of semantic analysis.

When a variable is declared, the compiler binds it to a symbol. Later, when it is used, the compiler must resolve that name to the correct declaration.

This is where lexical scoping becomes important.

For example, a variable declared in one block should not accidentally resolve to an outer variable with the same name if the language rules say otherwise.

This stage is where the compiler stops describing syntax and starts describing meaning.

That is a huge conceptual leap.

---

## Lesson 7: Type checking is a form of discipline

Once names resolve, the compiler can start checking types.

Type checking asks questions like:

- Is this arithmetic operation valid?
- Is this comparison legal?
- Is the condition truly boolean?
- Can this value be assigned here?

This is often the point where a language becomes reliable.

Without type checking, the runtime may accept garbage values and fail unpredictably. With type checking, the compiler catches invalid programs earlier.

This is not only a technical improvement. It is a design choice that makes the language more coherent and easier to teach.

---

## Lesson 8: The interpreter is the clearest proof that the pipeline works

The interpreter is one of the best parts of a teaching compiler.

It turns all the earlier stages into a concrete result. It answers the question:

Does the language actually do what we think it does?

When the interpreter runs, you can verify:

- tokenization is correct
- parsing is correct
- names resolve correctly
- type rules are enforced
- execution matches the intended behavior

If the interpreter is wrong, the bug is almost always earlier in the pipeline.

This makes the interpreter a debugging tool, not just a runtime feature.

It is one of the strongest teaching tools in a mini-language project.

---

## Lesson 9: Small is a feature, not a limitation

This is the most important lesson of all.

A tiny language is not a toy. It is a well-scoped teaching artifact.

A small compiler can still teach the core ideas behind real compiler engineering:

- lexing
- parsing
- ASTs
- binding
- type checking
- execution
- diagnostics

That is enough to understand the foundations of how larger languages work.

The temptation is to add features immediately. Arrays, functions, modules, recursion, optimizations, bytecode, and backend support all sound exciting. But if you add them too soon, you lose the clarity that made the project valuable in the first place.

Small is not a compromise. It is a design decision that keeps the system understandable.

---

## Final takeaway

Building a mini-language parser taught me that compilers are not mysterious machines. They are small, layered systems built from explicit decisions.

The real lessons were not about a particular language. They were about:

- designing a grammar
- understanding tokens
- modeling structure with ASTs
- enforcing valid meaning with binding and types
- validating behavior through execution
- debugging by following the pipeline

That is why I recommend this approach to anyone who wants to understand programming languages more deeply.

Start small. Make the pipeline visible. Keep the rules understandable. And build a language that teaches the ideas instead of hiding them.

That is how you turn an abstract compiler concept into something concrete.

---

## A final thought

If I had to summarize the project in one sentence, it would be this:

> A compiler is not a single invention; it is a series of small transformations that turn text into meaning and meaning into behavior.

And that is a lesson worth building a language to learn.
