namespace BasicCompiler.Runtime;

public sealed class Environment
{
    private readonly Dictionary<string, RuntimeValue> _values = new(StringComparer.Ordinal);
    public Environment? Parent { get; }

    public Environment(Environment? parent = null)
    {
        Parent = parent;
    }

    public void Define(string name, RuntimeValue value) => _values[name] = value;

    public RuntimeValue Get(string name)
    {
        if (_values.TryGetValue(name, out var value))
        {
            return value;
        }

        if (Parent is not null)
        {
            return Parent.Get(name);
        }

        throw new InvalidOperationException($"Undefined variable '{name}'.");
    }

    public void Set(string name, RuntimeValue value)
    {
        if (_values.ContainsKey(name))
        {
            _values[name] = value;
            return;
        }

        if (Parent is not null)
        {
            Parent.Set(name, value);
            return;
        }

        throw new InvalidOperationException($"Undefined variable '{name}'.");
    }
}
