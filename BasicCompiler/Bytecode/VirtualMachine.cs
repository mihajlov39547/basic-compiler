namespace BasicCompiler.Bytecode;

public sealed class VirtualMachine
{
    public object? Run(CompiledProgram program)
    {
        var stack = new Stack<object?>();
        var environment = new Dictionary<string, object?>(StringComparer.Ordinal);

        for (var i = 0; i < program.Instructions.Count; i++)
        {
            var instruction = program.Instructions[i];

            switch (instruction.OpCode)
            {
                case OpCode.LoadConstant:
                    stack.Push(program.Constants[(int)(instruction.Operand ?? 0)]);
                    break;

                case OpCode.LoadVariable:
                    stack.Push(environment[(string)instruction.Operand!]);
                    break;

                case OpCode.StoreVariable:
                    environment[(string)instruction.Operand!] = stack.Pop();
                    break;

                case OpCode.Add:
                    stack.Push(ApplyBinary(stack, (left, right) => Convert.ToDouble(left) + Convert.ToDouble(right)));
                    break;

                case OpCode.Subtract:
                    stack.Push(ApplyBinary(stack, (left, right) => Convert.ToDouble(left) - Convert.ToDouble(right)));
                    break;

                case OpCode.Multiply:
                    stack.Push(ApplyBinary(stack, (left, right) => Convert.ToDouble(left) * Convert.ToDouble(right)));
                    break;

                case OpCode.Divide:
                    stack.Push(ApplyBinary(stack, (left, right) => Convert.ToDouble(left) / Convert.ToDouble(right)));
                    break;

                case OpCode.Equals:
                    stack.Push(ApplyBinary(stack, (left, right) => Equals(left, right)));
                    break;

                case OpCode.Print:
                    Console.WriteLine(stack.Pop());
                    break;

                case OpCode.Halt:
                    return stack.Count > 0 ? stack.Peek() : null;
            }
        }

        return stack.Count > 0 ? stack.Peek() : null;
    }

    private static object? ApplyBinary(Stack<object?> stack, Func<object?, object?, object?> operation)
    {
        var right = stack.Pop();
        var left = stack.Pop();
        return operation(left, right);
    }
}
