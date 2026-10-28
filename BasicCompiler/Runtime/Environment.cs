namespace BasicCompiler.Runtime;

public sealed class Environment
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);
    public Environment? Parent { get; }

    public Environment(Environment? parent = null)
    {
        Parent = parent;
    }

    public void Define(string name, object? value) => _values[name] = value;

    public object? Get(string name)
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

    public void Set(string name, object? value)
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
