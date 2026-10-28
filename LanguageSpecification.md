# MiniBASIC Language Specification

Version: 0.1

Language style: Pascal-like, statically typed, imperative educational programming language.

## 1. Language Overview

MiniBASIC is a small statically typed imperative programming language intended for teaching compiler construction, interpreters, virtual machines, semantic analysis, and code generation.

The language intentionally contains a small number of concepts:

- variables with mandatory initialization
- automatic local type inference
- four primitive value types
- arrays
- functions
- one entry-point program
- expressions
- assignment
- if/else
- while
- function calls
- explicit return values

MiniBASIC has no null value.

Every variable must contain a valid value immediately after its declaration.

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

Arrays contain values of a single type.

Examples:

```text
var numbers := [1, 2, 3, 4]
```

Type:

- Integer[]

Similarly:

```text
var prices := [1.5, 2.5, 3.5]
```

has type:

- Decimal[]

and:

```text
var names := ["Ana", "Marko", "John"]
```

has type:

- String[]

An array cannot mix incompatible types:

```text
var values := [1, "hello", true]
```

Compile-time error.

For version 0.1 define:

- Integer[]
- Decimal[]
- String[]
- Boolean[]

but not nested arrays yet.

This keeps the first compiler manageable.

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

Functions operate entirely on their explicit parameters and local variables.

Suggested syntax:

```text
function name(parameterName: Type, ...): ReturnType

    statements

    return expression

endfunction
```

Example:

```text
function add(a: Integer, b: Integer): Integer
    var result := a + b
    return result
endfunction
```

Or simply:

```text
function add(a: Integer, b: Integer): Integer
    return a + b
endfunction
```

Another:

```text
function isPositive(value: Decimal): Boolean

    if value > 0.0 then
        return true
    else
        return false
    endif

endfunction
```

## 20. Function Parameters

Parameters must have explicitly declared types.

For example:

```text
function greet(name: String, times: Integer): String
```

This is intentionally different from local variables.

Local:

```text
var x := 10
```

uses inference.

Function parameter:

```text
x: Integer
```

is explicit.

This gives us a clean compile-time function contract.

## 21. Parameter Passing

For version 0.1 recommend a simple rule:

- Parameters are passed by value.

So:

```text
function increment(value: Integer): Integer
    value := value + 1
    return value
endfunction
```

does not modify the caller's variable.

This avoids introducing Pascal's `var` reference parameters yet.

## 22. Function Return Values

Every function has exactly one declared return type:

- Integer
- Decimal
- String
- Boolean

and, optionally if arrays are allowed as function values:

- Integer[]
- Decimal[]
- String[]
- Boolean[]

Example:

```text
function maximum(a: Integer, b: Integer): Integer

    if a > b then
        return a
    else
        return b
    endif

endfunction
```

For the initial language, make every ordinary function return a value. We therefore do not need a `void` type yet.

That creates a useful semantic-analysis problem:

Can every reachable function path return a value?

For example:

```text
function broken(a: Integer): Integer

    if a > 0 then
        return a
    endif

endfunction
```

should be rejected because there is an execution path without return.

That later gives us our first interesting control-flow analysis.

## 23. Function Scope

A function can access only:

- its parameters
- its locally declared variables
- other globally declared functions / built-in functions

It cannot access global mutable variables because MiniBASIC has none.

For example:

```text
function calculate(a: Integer): Integer

    var temp := a * 2

    return temp

endfunction
```

Both `a` and `temp` are local to `calculate`.

This is a very good restriction for an educational compiler because function dependencies become explicit.

## 24. Side Effects

There's one subtlety in the statement:

> "A function works exclusively with parameters it receives."

If interpreted absolutely, even:

```text
print("hello")
```

would violate it because `print` causes an external side effect.

Define the rule more precisely:

User-defined functions cannot read or modify global program state. They may use explicitly defined runtime functions such as `print`.

This leaves a very clean name resolution model without unnecessarily turning the language into a purely functional language.

## 25. Function Calls

Functions are called using:

```text
name(arguments)
```

Example:

```text
var result := add(10, 20)
```

or:

```text
var positive := isPositive(-3.5)
```

The compiler verifies:

- function exists
- argument count is correct
- argument types are compatible
- returned type is appropriate for its context

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

Define a MiniBASIC source file approximately as:

```text
compilationUnit
    = functionDeclaration* programDeclaration EOF ;

functionDeclaration
    = "function"
      identifier
      "(" parameterList? ")"
      ":"
      type
      statement*
      "endfunction" ;

programDeclaration
    = "program"
      identifier
      "(" parameterList? ")"
      ":"
      "Integer"
      statement*
      "endprogram" ;
```

Notice the intentional rule:

There is exactly one program declaration and it must be the final top-level declaration.

That keeps parsing and compilation deterministic and easy to explain.

## 32. Statements

```text
statement
    = variableDeclaration
    | assignment
    | ifStatement
    | whileStatement
    | returnStatement
    | expressionStatement ;

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

returnStatement
    = "return" expression ;
```

## 33. Expression grammar

A clean starting point:

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
    = term (("<" | "<=" | ">" | ">=") term)* ;

term
    = factor (("+" | "-") factor)* ;

factor
    = unary (("*" | "/") unary)* ;

unary
    = ("!" | "-") unary
    | postfix ;

postfix
    = primary
      ("[" expression "]"
      | "(" argumentList? ")")* ;

primary
    = INTEGER_LITERAL
    | DECIMAL_LITERAL
    | STRING_LITERAL
    | "true"
    | "false"
    | identifier
    | arrayLiteral
    | "(" expression ")" ;

arrayLiteral
    = "[" (expression ("," expression)*)? "]" ;
```

This grammar should be the normative core specification for the student project.

## 34. The AST naturally follows from the specification

From this language we know almost exactly which AST nodes are needed:

```text
Program
│
├── FunctionDeclaration*
└── ProgramDeclaration
```

Statements:

```text
Statement
├── VariableDeclaration
├── Assignment
├── IfStatement
├── WhileStatement
├── ReturnStatement
└── ExpressionStatement
```

Expressions:

```text
Expression
├── IntegerLiteral
├── DecimalLiteral
├── StringLiteral
├── BooleanLiteral
├── ArrayLiteral
├── VariableExpression
├── UnaryExpression
├── BinaryExpression
├── CallExpression
└── IndexExpression
```

And signatures:

```text
CallableDeclaration
├── name
├── parameters
├── returnType
└── body
```

with:

- `FunctionDeclaration`
- `ProgramDeclaration`

as two specializations.

This is a very nice language for the course because the grammar remains sufficiently small to implement by hand, while the semantic model is already rich enough to introduce ASTs, Visitor, symbol tables, scopes, type checking, control-flow analysis, interpreter, stack VM, bytecode, function calls, activation records, linking concepts and eventually JIT/LLVM.

One design decision to lock immediately: the program should internally be treated as a special function returning `Integer`, rather than inventing an unrelated execution mechanism for it. That simplification will pay off repeatedly when implementing the parser, AST, semantic analysis, runtime and compiler.

## 35. What should the project support?

The project should support the following minimum milestone for version 0.1:

- lexical tokens for keywords, identifiers, literals, operators, and punctuation
- parser for variable declarations, assignments, expressions, if/else, while loops, and function/program declarations
- AST nodes for all core statements and expressions
- symbol table and scope resolution for local variables and functions
- static type checking for numbers, strings, booleans, arrays, and function signatures
- interpreter for evaluating the AST at runtime
- optional bytecode lowering and VM execution for later phases
- deterministic xUnit tests covering each compiler phase

This guarantees that the project remains faithful to the educational goal: teaching how a compiler moves from source text to structured representation, semantics, execution, and later lower-level code generation.
