namespace BasicCompiler.Semantics;

public sealed class Scope
{
    private readonly Dictionary<string, Symbol> _symbols = new(StringComparer.Ordinal);

    public Scope? Parent { get; }

    public Scope(Scope? parent = null)
    {
        Parent = parent;
    }

    public void Declare(Symbol symbol)
    {
        if (_symbols.ContainsKey(symbol.Name))
        {
            throw new InvalidOperationException($"Symbol '{symbol.Name}' already declared in this scope.");
        }

        _symbols[symbol.Name] = symbol;
    }

    public bool TryResolve(string name, out Symbol? symbol)
    {
        if (_symbols.TryGetValue(name, out symbol))
        {
            return true;
        }

        if (Parent is not null)
        {
            return Parent.TryResolve(name, out symbol);
        }

        symbol = null;
        return false;
    }
}
