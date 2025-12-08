namespace AdventOfCode;

public sealed class Day07 : BaseDay
{
    private readonly string _input;

    public Day07()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private sealed record Grid(int Start, HashSet<int>[] Splits);

    private static HashSet<int> FindSplits(string input) => input
        .Select(((c, i) => (c, i)))
        .Where(x => x.c == '^')
        .Select(x => x.i)
        .ToHashSet();

    private Grid ParseInput()
    {
        var lines = _input.Lines().ToList();

        var start = lines.First().IndexOf('S');

        var splits = lines
            .Where((_, i) => i % 2 == 0)
            .Skip(1)
            .Select(FindSplits)
            .ToArray();

        return new Grid(start, splits);
    }

    public override ValueTask<string> Solve_1() => CalculateBeam(ParseInput()).ToSolution();

    public override ValueTask<string> Solve_2()
    {
        var (start, splitters) = ParseInput();
        return CalculateBeamTimeLines(start, new ReadOnlySpan<HashSet<int>>(splitters)).ToSolution();
    }

    private static int CalculateBeam(Grid grid)
    {
        HashSet<int> beams = [grid.Start];
        HashSet<int> newBeams = [];
        var splits = 0;

        foreach (var line in grid.Splits)
        {
            newBeams.Clear();

            foreach (var beam in beams)
            {
                if (line.Contains(beam))
                {
                    newBeams.Add(beam - 1);
                    newBeams.Add(beam + 1);
                    splits++;
                }
                else
                {
                    newBeams.Add(beam);
                }
            }

            (beams, newBeams) = (newBeams, beams);
        }

        return splits;
    }

    private readonly Dictionary<(int Start, int Left), long> _timelineCache = [];

    private long CalculateBeamTimeLines(int start, ReadOnlySpan<HashSet<int>> splitters)
    {
        var left = splitters.Length;
        if (left == 0) return 1;

        var line = splitters[0];
        var newSplitters = splitters[1..];

        if (_timelineCache.TryGetValue((start, left), out var cachedResult)) return cachedResult;

        long result;
        if (line.Contains(start))
        {
            result = CalculateBeamTimeLines(start - 1, newSplitters) +
                   CalculateBeamTimeLines(start + 1, newSplitters);
        }
        else
        {
            result = CalculateBeamTimeLines(start, newSplitters);
        }

        _timelineCache[(start, left)] = result;
        return result;
    }
}