namespace BasicCompiler.Semantics;

public sealed class Scope
{
    private readonly Dictionary<string, Symbol> _symbols = new(StringComparer.Ordinal);

    public Scope? Parent { get; }

    public Scope(Scope? parent = null)
    {
        Parent = parent;
    }

    public IReadOnlyList<Symbol> Symbols => _symbols.Values.ToList();

    public void Declare(Symbol symbol)
    {
        if (_symbols.ContainsKey(symbol.Name))
        {
            throw new InvalidOperationException($"Symbol '{symbol.Name}' already declared in this scope.");
        }

        _symbols[symbol.Name] = symbol;
    }

    public IEnumerable<Scope> WalkAncestors()
    {
        var current = this;
        while (current is not null)
        {
            yield return current;
            current = current.Parent;
        }
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

    public IEnumerable<Symbol> Flatten()
    {
        if (Parent is not null)
        {
            foreach (var symbol in Parent.Flatten())
            {
                yield return symbol;
            }
        }

        foreach (var symbol in _symbols.Values)
        {
            yield return symbol;
        }
    }
}
