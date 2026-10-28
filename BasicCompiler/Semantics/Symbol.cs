namespace BasicCompiler.Semantics;

public class Symbol
{
    public Symbol(string name, string typeName, bool isMutable = true)
    {
        Name = name;
        TypeName = typeName;
        IsMutable = isMutable;
    }

    public string Name { get; }
    public string TypeName { get; }
    public bool IsMutable { get; }
}
