# MiniBASIC Language Specification

Version: 0.1

Language style: Pascal-like, statically typed, imperative educational programming language.

## 1. Language Overview

MiniBASIC is a small statically typed imperative programming language intended for teaching compiler construction, interpreters, virtual machines, semantic analysis, and code generation.

This specification describes the v0.1 language. The first edition is intentionally minimal and deliberately excludes advanced language features that belong in future work.

The language intentionally contains a small number of concepts:

- variables with mandatory initialization
- automatic local type inference
- primitive value types
- expressions
- assignment
- if/else
- while
- simple output

MiniBASIC has no null value.

Every variable must contain a valid value immediately after its declaration.

## 1.1 Scope lock for v0.1

The first version is intentionally limited to:

- Number
- String
- Boolean
- `var` declarations with initialization
- assignment
- arithmetic and comparisons
- `if ... then ... else ... endif`
- `while ... do ... endwhile`
- `print(...)`

The following features are intentionally out of scope for v0.1 and should be treated as future work:

- arrays
- user-defined functions
- function declarations
- recursion
- modules
- inheritance or classes
- optimizer passes
- IR / bytecode VM architecture

This keeps the project focused on a teachable first compiler pipeline.

## 2. Primitive Types

MiniBASIC defines four primitive types:

- Integer
- Decimal
- String
- Boolean

### 2.1 Integer

Represents whole numbers.

Examples:

- 0
- 1
- -1
- 42
- 1000

Example declaration:

```text
var age := 42
```

The inferred type of `age` is:

- Integer

## 3. Decimal

Represents numbers containing a fractional component.

Examples:

- 1.0
- 1.25
- -45.75
- 0.001

Example:

```text
var price := 19.95
```

The inferred type is:

- Decimal

For the teaching implementation, define these concretely as:

- Integer = signed 64-bit integer
- Decimal = IEEE-754 64-bit floating point

That makes implementation straightforward in C#:

- Integer → `System.Int64`
- Decimal → `System.Double`

## 4. String

Strings are sequences of characters delimited by double quotation marks.

```text
var message := "Hello world"
var name := "Marko"
var empty := ""
```

Their type is:

- String

Basic escape sequences can be:

- `\"` double quote
- `\\` backslash
- `\n` newline
- `\t` tab

For example:

```text
var message := "Hello\nworld"
```

## 5. Boolean

Boolean has exactly two values:

- true
- false

Examples:

```text
var enabled := true
var finished := false
```

Both variables have type:

- Boolean

## 6. Arrays

This feature is intentionally not part of v0.1.

Arrays may be added in a later iteration of the language, but they are intentionally excluded from the first compiler milestone so the language remains small and easy to teach.

For v0.1, the project focuses on scalar values only:

- Number
- String
- Boolean

Array syntax and array semantics are marked as future work.

## 7. No Null Values

MiniBASIC has no null, nil, Nothing, or undefined value.

Therefore this is illegal:

```text
var name
```

as is:

```text
var name := null
```

Every declaration requires an initializer:

```text
var name := "Marko"
var age := 25
var valid := true
```

This enormously simplifies the first type system and runtime.

## 8. Variable Declaration

Variables are declared using:

```text
var identifier := expression
```

Examples:

```text
var count := 1
var price := 10.50
var message := "Hello"
var valid := true
```

Types are inferred from the initializer:

- count → Integer
- price → Decimal
- message → String
- valid → Boolean

Therefore:

```text
var count: Integer := 1
```

is unnecessary in version 0.1.

This is deliberate. We teach explicit types through function signatures, while local declarations demonstrate basic type inference.

## 9. Assignment

An existing variable is assigned using:

```text
identifier := expression
```

Example:

```text
var counter := 1

counter := counter + 1
```

The assigned expression must be type-compatible with the variable.

Valid:

```text
var a := 1
a := 25
```

Invalid:

```text
var a := 1
a := "Hello"
```

Compiler diagnostic:

```text
Type mismatch.
Variable 'a' has type Integer but the assigned expression has type String.
```

This establishes an important rule:

Variable types are inferred once at declaration and do not change.

## 10. Expressions

MiniBASIC expressions may contain:

- literals
- variables
- array access
- function calls
- unary operations
- binary operations
- parentheses

Example:

```text
a + b * 2
```

## 11. Arithmetic Operators

Supported operators:

- +
- -
- *
- /

Examples:

```text
var a := 10 + 20
var b := a * 2
var c := (a + b) / 2
```

Unary minus is also supported:

```text
var a := -10
```

## 12. Numeric Type Rules

For version 0.1:

- Integer op Integer → Integer
- Decimal op Decimal → Decimal
- Integer op Decimal → Decimal
- Decimal op Integer → Decimal

Therefore:

```text
var x := 10 + 20
```

produces Integer.

But:

```text
var x := 10 + 20.5
```

produces Decimal.

This introduces one carefully controlled implicit conversion:

- Integer → Decimal

but never:

- Decimal → Integer

implicitly.

## 13. Comparison Operators

MiniBASIC supports:

- =
- !=
- <
- <=
- >
- >=

The result of every comparison is:

- Boolean

For example:

```text
var a := 10
var result := a < 20
```

`result` has type:

- Boolean

Note that we deliberately use Pascal-like:

```text
a = b
```

rather than C-like:

```text
a == b
```

Assignment cannot be confused with equality because assignment uses:

```text
:=
```

This is a nice language-design decision for the course.

## 14. Logical Operators

The language supports:

- &&
- ||
- !

Examples:

```text
var result := a > 10 && b < 20
```

and:

```text
var result := a = 1 || a = b
```

Negation:

```text
var result := !finished
```

Operands must be Boolean.

## 15. Operator Precedence

From highest to lowest:

1. ()
2. array indexing / function call
3. !
4. unary -
5. * /
6. + -
7. < <= > >=
8. = !=
9. &&
10. ||

Therefore:

```text
a = 1 || a = b && c > 10
```

means:

```text
(a = 1) || ((a = b) && (c > 10))
```

## 16. If Statement

The only conditional statement is if.

Syntax:

```text
if condition then

    statements

endif
```

Example:

```text
if age >= 18 then
    print("Adult")
endif
```

The condition must have type:

- Boolean

## 17. If-Else

The optional else branch is:

```text
if condition then

    statements

else

    statements

endif
```

Example:

```text
if a = 1 || a = b then
    print("Condition is true")
else
    print("Condition is false")
endif
```

There is deliberately no `elseif` in version 0.1.

Students can express it by nesting:

```text
if a = 1 then
    print("one")
else

    if a = 2 then
        print("two")
    else
        print("other")
    endif

endif
```

Later, `elseif` becomes an excellent example of syntactic sugar and desugaring.

## 18. While Loop

The language has only one looping construct:

- while

Syntax:

```text
while condition do

    statements

endwhile
```

Example:

```text
var a := 1

while a < 100 do
    print(a)
    a := a + 1
endwhile
```

Again:

- condition : Boolean

is mandatory.

There is deliberately no:

- for
- repeat
- do-while
- foreach

in v0.1.

Later in the course we can add:

```text
for i := 1 to 10 do
    print(i)
endfor
```

and teach desugaring by translating it internally into while.

## 19. Function Definition

This feature is intentionally not part of v0.1.

User-defined functions are future work and should not be part of the initial compiler milestone. The first language version focuses on variables, assignment, conditionals, loops, and simple output.

The plan reserves function declarations, parameter lists, return values, recursion, and call semantics for future study.

## 20. Function Parameters

This feature is intentionally not part of v0.1.

Parameter-binding rules, value semantics, and call-stack behavior are part of the advanced future-work track rather than the core compiler roadmap.

## 21. Parameter Passing

This feature is intentionally not part of v0.1.

The initial language does not include user-defined functions, so parameter passing is outside the current scope.

## 22. Function Return Values

This feature is intentionally not part of v0.1.

The first compiler milestone does not define return expressions or function contracts.

## 23. Function Scope

This feature is intentionally not part of v0.1.

Function-local scope and evaluation environments are intentionally deferred until the project has completed the core lexical, syntactic, semantic, and runtime pipeline.

## 24. Side Effects

This feature is intentionally not part of v0.1.

The initial language does not contain user-defined functions, so side-effect tracking and function purity rules are deferred to a later stage.

## 25. Function Calls

This feature is intentionally not part of v0.1.

Function calls are future work and should not be implemented before the core compiler is stable.

## 26. The Program Entry Point

The Pascal idea is adapted as follows.

Original Pascal style was approximately:

```text
program Hello;

begin
   ...
end.
```

For MiniBASIC, use:

```text
program MyProgram(args: String[]): Integer

    statements

    return 0

endprogram
```

So the program is conceptually a special function and execution entry point.

For example:

```text
program Hello(args: String[]): Integer

    print("Hello world")

    return 0

endprogram
```

By convention:

- 0 = successful completion
- non-0 = unsuccessful completion

This intentionally mirrors the useful operating-system idea of an exit code without tying the language specification to one particular OS.

## 27. Program Parameters

Because you want the program to behave like an ordinary function, allow:

```text
program MyProgram(args: String[]): Integer
```

as the canonical executable entry point.

But internally the compiler should treat the program almost exactly like:

```text
function MyProgram(args: String[]): Integer
```

with one additional property:

- `IsEntryPoint = true`

This will later be excellent for explaining the distinction between:

- language semantics
- runtime / ABI entry point

## 28. Complete MiniBASIC Program

A full example could now look like this:

```text
function sum(values: Integer[]): Integer

    var index := 0
    var total := 0

    while index < length(values) do

        total := total + values[index]
        index := index + 1

    endwhile

    return total

endfunction


function isPositive(value: Integer): Boolean

    if value > 0 then
        return true
    else
        return false
    endif

endfunction


program Example(args: String[]): Integer

    var numbers := [10, 20, 30]
    var result := sum(numbers)

    if isPositive(result) then
        print("Result is positive")
        print(result)
    else
        print("Result is not positive")
    endif

    return 0

endprogram
```

This tiny program already gives the compiler nearly everything we want to teach:

```text
                     SOURCE
                        │
                        ▼
                      lexer
                        │
              keywords / literals
                        │
                        ▼
                      parser
                        │
                        ▼
                       AST
                        │
        ┌───────────────┼──────────────┐
        ▼               ▼              ▼
       scope        name resolution   types
        │               │              │
        └───────────────┼──────────────┘
                        ▼
               semantic analysis
                        │
                        ▼
                    lowering
                        │
                        ▼
                       IR
                        │
            ┌───────────┴───────────┐
            ▼                       ▼
        interpreter            bytecode compiler
                                    │
                                    ▼
                                    VM
```

## 29. Keywords

For v0.1 freeze the reserved words as:

- program
- endprogram
- function
- endfunction
- var
- return
- if
- then
- else
- endif
- while
- do
- endwhile
- true
- false

Notice that:

- Integer
- Decimal
- String
- Boolean

could either be reserved keywords or type identifiers.

For the teaching compiler, make them keywords because that makes the type grammar clearer.

So complete reserved set becomes:

- program
- endprogram
- function
- endfunction
- var
- return
- if
- then
- else
- endif
- while
- do
- endwhile
- Integer
- Decimal
- String
- Boolean
- true
- false

Later we can deliberately discuss the alternative design.

## 30. What the language deliberately does NOT contain

This is just as important as what it contains.

MiniBASIC v0.1 has no:

- null
- classes
- objects
- interfaces
- inheritance
- exceptions
- pointers
- references
- structs
- enums
- generics
- lambdas
- closures
- global variables
- dynamic typing
- operator overloading
- implicit String/Number conversion
- for loop
- do-while loop
- switch/case
- goto
- async/await
- threads

That limitation is deliberate.

We want students studying compiler construction, not spending the semester implementing language features.

## 31. Formal top-level structure

The v0.1 compiler uses a very small top-level grammar:

```text
compilationUnit
    = statement* EOF ;
```

This means the project is a sequence of statements rather than a module system or a function-based program model.

## 32. Statements

```text
statement
    = variableDeclaration
    | assignment
    | ifStatement
    | whileStatement
    | printStatement ;

variableDeclaration
    = "var" identifier ":=" expression ;

assignment
    = identifier ":=" expression ;

ifStatement
    = "if" expression "then"
        statement*
      ("else"
        statement*)?
      "endif" ;

whileStatement
    = "while" expression "do"
        statement*
      "endwhile" ;

printStatement
    = "print" "(" expression ")" ;
```

This grammar is the normative core for the student project.

## 33. Expression grammar

```text
expression
    = logicalOr ;

logicalOr
    = logicalAnd ("||" logicalAnd)* ;

logicalAnd
    = equality ("&&" equality)* ;

equality
    = comparison (("=" | "!=") comparison)* ;

comparison
    = additive (("<" | "<=" | ">" | ">=") additive)* ;

additive
    = multiplicative (("+" | "-") multiplicative)* ;

multiplicative
    = unary (("*" | "/") unary)* ;

unary
    = ("!" | "-") unary
    | primary ;

primary
    = INTEGER_LITERAL
    | DECIMAL_LITERAL
    | STRING_LITERAL
    | "true"
    | "false"
    | identifier
    | "(" expression ")" ;
```

## 34. The AST naturally follows from the specification

From this minimal language, the AST stays small and teachable:

```text
Program
└── Statement[]
```

Statements:

```text
Statement
├── VariableDeclaration
├── Assignment
├── IfStatement
├── WhileStatement
└── PrintStatement
```

Expressions:

```text
Expression
├── IntegerLiteral
├── DecimalLiteral
├── StringLiteral
├── BooleanLiteral
├── VariableExpression
├── UnaryExpression
├── BinaryExpression
└── ParenthesizedExpression
```

For the initial milestone, the grammar remains intentionally small. The project should support:

- lexical tokens for keywords, identifiers, literals, operators, and punctuation
- parser for variable declarations, assignments, expressions, if/else, and while loops
- AST nodes for core statements and expressions
- symbol table and scope resolution for local variables
- static type checking for numbers, strings, booleans, and simple assignments
- interpreter for evaluating the AST at runtime
- deterministic xUnit tests covering each compiler phase

This keeps the project faithful to the educational goal: teaching how a compiler moves from source text to structured representation, semantics, and execution without prematurely introducing advanced runtime and function machinery.
