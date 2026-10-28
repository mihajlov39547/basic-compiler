using BasicCompiler.Diagnostics;

namespace BasicCompiler.Text;

public sealed class SourceText
{
    private readonly List<int> _lineStarts;

    public SourceText(string text, string fileName = "")
    {
        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        FileName = fileName;
        Text = text;
        _lineStarts = ComputeLineStarts(text);
    }

    public string FileName { get; }
    public string Text { get; }
    public int Length => Text.Length;
    public int LineCount => _lineStarts.Count;

    public static SourceText FromText(string text, string fileName = "") => new(text, fileName);

    public int GetLineStart(int line)
    {
        if (line < 0 || line >= _lineStarts.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(line));
        }

        return _lineStarts[line];
    }

    public SourcePosition GetPosition(int index)
    {
        if (index < 0 || index > Text.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var line = 0;
        var lineStart = 0;

        while (line + 1 < _lineStarts.Count && _lineStarts[line + 1] <= index)
        {
            line++;
            lineStart = _lineStarts[line];
        }

        return new SourcePosition(index, line, index - lineStart);
    }

    public SourceSpan GetSpan(int start, int length)
    {
        if (start < 0 || length < 0 || start + length > Text.Length)
        {
            throw new ArgumentOutOfRangeException();
        }

        var position = GetPosition(start);
        return new SourceSpan(start, length, position.Line, position.Column);
    }

    public override string ToString() => Text;

    private static List<int> ComputeLineStarts(string text)
    {
        var starts = new List<int> { 0 };

        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                starts.Add(i + 1);
            }
        }

        return starts;
    }
}
