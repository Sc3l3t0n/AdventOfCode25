namespace AdventOfCode;

public static class InputExtension
{
    extension(string input)
    {
        public IEnumerable<string> Lines() =>
            input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}