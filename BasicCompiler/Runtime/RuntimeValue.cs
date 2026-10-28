using System.Globalization;

namespace BasicCompiler.Runtime;

public abstract record RuntimeValue
{
    public abstract string Kind { get; }
    public abstract object ToClrObject();

    public static RuntimeValue Number(double value) => new NumberValue(value);
    public static RuntimeValue Number(int value) => new NumberValue(value);
    public static RuntimeValue String(string value) => new StringValue(value);
    public static RuntimeValue Boolean(bool value) => new BooleanValue(value);

    public static RuntimeValue FromObject(object? value) => value switch
    {
        bool b => Boolean(b),
        string s => String(s),
        int i => Number(i),
        long l => Number(l),
        double d => Number(d),
        float f => Number(f),
        decimal d => Number((double)d),
        _ => throw new InvalidOperationException($"Unsupported runtime value '{value ?? "null"}'.")
    };

    public override string ToString() => ToClrObject().ToString() ?? string.Empty;
}

public sealed record NumberValue(double Value) : RuntimeValue
{
    public override string Kind => "number";
    public override object ToClrObject() => Value;
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}

public sealed record StringValue(string Value) : RuntimeValue
{
    public override string Kind => "string";
    public override object ToClrObject() => Value;
    public override string ToString() => Value;
}

public sealed record BooleanValue(bool Value) : RuntimeValue
{
    public override string Kind => "boolean";
    public override object ToClrObject() => Value;
    public override string ToString() => Value.ToString();
}
