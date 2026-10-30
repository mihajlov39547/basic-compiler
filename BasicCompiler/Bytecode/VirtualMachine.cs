namespace BasicCompiler.Bytecode;

public sealed class VirtualMachine
{
    public object? Run(CompiledProgram program)
    {
        var trace = RunWithTrace(program);
        return trace.FinalValue;
    }

    public VmTrace RunWithTrace(CompiledProgram program)
    {
        var stack = new Stack<object?>();
        var environment = new Dictionary<string, object?>(StringComparer.Ordinal);
        var trace = new List<string>();
        object? lastValue = null;

        for (var i = 0; i < program.Instructions.Count; i++)
        {
            var instruction = program.Instructions[i];

            switch (instruction.OpCode)
            {
                case OpCode.LoadConstant:
                    var loadedConstant = program.Constants[(int)(instruction.Operand ?? 0)];
                    stack.Push(loadedConstant);
                    lastValue = loadedConstant;
                    trace.Add($"{i:00}: LOAD_CONSTANT {instruction.Operand} -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.LoadVariable:
                    var variableValue = environment[(string)instruction.Operand!];
                    stack.Push(variableValue);
                    lastValue = variableValue;
                    trace.Add($"{i:00}: LOAD_VARIABLE {instruction.Operand} -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.StoreVariable:
                    var storedValue = stack.Pop();
                    environment[(string)instruction.Operand!] = storedValue;
                    lastValue = storedValue;
                    trace.Add($"{i:00}: STORE_VARIABLE {instruction.Operand} = {storedValue} -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Add:
                    var addResult = ApplyBinary(stack, (left, right) => Convert.ToDouble(left) + Convert.ToDouble(right));
                    stack.Push(addResult);
                    lastValue = addResult;
                    trace.Add($"{i:00}: ADD -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Subtract:
                    var subtractResult = ApplyBinary(stack, (left, right) => Convert.ToDouble(left) - Convert.ToDouble(right));
                    stack.Push(subtractResult);
                    lastValue = subtractResult;
                    trace.Add($"{i:00}: SUBTRACT -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Multiply:
                    var multiplyResult = ApplyBinary(stack, (left, right) => Convert.ToDouble(left) * Convert.ToDouble(right));
                    stack.Push(multiplyResult);
                    lastValue = multiplyResult;
                    trace.Add($"{i:00}: MULTIPLY -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Divide:
                    var divideResult = ApplyBinary(stack, (left, right) => Convert.ToDouble(left) / Convert.ToDouble(right));
                    stack.Push(divideResult);
                    lastValue = divideResult;
                    trace.Add($"{i:00}: DIVIDE -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Equals:
                    var equalsResult = ApplyBinary(stack, (left, right) => Equals(left, right));
                    stack.Push(equalsResult);
                    lastValue = equalsResult;
                    trace.Add($"{i:00}: EQUALS -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.NotEquals:
                    var notEqualsResult = ApplyBinary(stack, (left, right) => !Equals(left, right));
                    stack.Push(notEqualsResult);
                    lastValue = notEqualsResult;
                    trace.Add($"{i:00}: NOT_EQUALS -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.LessThan:
                    var lessThanResult = ApplyBinary(stack, (left, right) => Convert.ToDouble(left) < Convert.ToDouble(right));
                    stack.Push(lessThanResult);
                    lastValue = lessThanResult;
                    trace.Add($"{i:00}: LESS_THAN -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.GreaterThan:
                    var greaterThanResult = ApplyBinary(stack, (left, right) => Convert.ToDouble(left) > Convert.ToDouble(right));
                    stack.Push(greaterThanResult);
                    lastValue = greaterThanResult;
                    trace.Add($"{i:00}: GREATER_THAN -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.LessThanOrEqual:
                    var lessThanOrEqualResult = ApplyBinary(stack, (left, right) => Convert.ToDouble(left) <= Convert.ToDouble(right));
                    stack.Push(lessThanOrEqualResult);
                    lastValue = lessThanOrEqualResult;
                    trace.Add($"{i:00}: LESS_THAN_OR_EQUAL -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.GreaterThanOrEqual:
                    var greaterThanOrEqualResult = ApplyBinary(stack, (left, right) => Convert.ToDouble(left) >= Convert.ToDouble(right));
                    stack.Push(greaterThanOrEqualResult);
                    lastValue = greaterThanOrEqualResult;
                    trace.Add($"{i:00}: GREATER_THAN_OR_EQUAL -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Not:
                    var notValue = stack.Pop();
                    var notResult = !(notValue is bool boolValue && boolValue);
                    stack.Push(notResult);
                    lastValue = notResult;
                    trace.Add($"{i:00}: NOT -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.And:
                    var andResult = ApplyBinary(stack, (left, right) => (bool)left && (bool)right);
                    stack.Push(andResult);
                    lastValue = andResult;
                    trace.Add($"{i:00}: AND -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Or:
                    var orResult = ApplyBinary(stack, (left, right) => (bool)left || (bool)right);
                    stack.Push(orResult);
                    lastValue = orResult;
                    trace.Add($"{i:00}: OR -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Negate:
                    var negateValue = stack.Pop();
                    var negateResult = -Convert.ToDouble(negateValue);
                    stack.Push(negateResult);
                    lastValue = negateResult;
                    trace.Add($"{i:00}: NEGATE -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Jump:
                    i = (int)(instruction.Operand ?? i) - 1;
                    trace.Add($"{i:00}: JUMP {instruction.Operand}");
                    break;

                case OpCode.JumpIfFalse:
                    var condition = stack.Pop();
                    var shouldJump = condition is bool flag && !flag;
                    if (shouldJump)
                    {
                        i = (int)(instruction.Operand ?? i) - 1;
                    }
                    trace.Add($"{i:00}: JUMP_IF_FALSE {instruction.Operand} -> {shouldJump} | stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Print:
                    var printValue = stack.Pop();
                    Console.WriteLine(printValue);
                    lastValue = printValue;
                    trace.Add($"{i:00}: PRINT {printValue} -> stack=[{string.Join(", ", stack.Reverse())}] env={FormatEnvironment(environment)}");
                    break;

                case OpCode.Halt:
                    return new VmTrace(trace, lastValue);
            }
        }

        return new VmTrace(trace, lastValue);
    }

    private static object? ApplyBinary(Stack<object?> stack, Func<object?, object?, object?> operation)
    {
        var right = stack.Pop();
        var left = stack.Pop();
        return operation(left, right);
    }

    private static string FormatEnvironment(Dictionary<string, object?> environment)
    {
        if (environment.Count == 0)
        {
            return "{}";
        }

        return "{" + string.Join(", ", environment.Select(pair => $"{pair.Key}={pair.Value}")) + "}";
    }
}

public sealed record VmTrace(IReadOnlyList<string> Trace, object? FinalValue);
