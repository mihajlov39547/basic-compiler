namespace BasicCompiler.Semantics;

public sealed class VariableSymbol : Symbol
{
    public VariableSymbol(string name, string typeName, bool isMutable = true)
        : base(name, typeName, isMutable)
    {
    }
}
