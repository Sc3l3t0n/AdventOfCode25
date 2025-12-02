namespace AdventOfCode;

public sealed class Day02 : BaseDay
{
    private readonly string _input;

    public Day02()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private IEnumerable<Range> ParseInput() =>
        _input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseRange);

    public override ValueTask<string> Solve_1() => ParseInput().Aggregate<Range, long>(0, SumUpInvalidIdsUnprecise).ToSolution();

    public override ValueTask<string> Solve_2() => ParseInput().Aggregate<Range, long>(0, SumUpInvalidIdsPrecise).ToSolution();

    private record Range(long Start, long End);

    private static Range ParseRange(string input)
    {
        var splitStrings = input.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new Range(long.Parse(splitStrings[0]), long.Parse(splitStrings[1]));
    }

    private static long SumUpInvalidIdsUnprecise(long sum, Range range) =>
        SumUpInvalidIds(sum, range, IdIsInvalidUnprecise);

    private static long SumUpInvalidIdsPrecise(long sum, Range range) =>
        SumUpInvalidIds(sum, range, IdIsInvalidPrecise);

    private static long SumUpInvalidIds(long sum, Range range, Func<long, int, bool> validCheck)
        => Enumerable.Range(0, (int)(range.End - range.Start + 1))
            .Select(offset => (id: range.Start + offset, len: (int)Math.Log10(range.Start + offset) + 1))
            .Where(x => validCheck(x.id, x.len))
            .Aggregate(sum, (agg, x) => agg + x.id);
    
    public static bool IdIsInvalidUnprecise(long id, int len) =>
        len % 2 == 0 &&
        (id / (long)Math.Pow(10, len / 2)) == (id % (long)Math.Pow(10, len / 2));

    // NOTE: Slow, but works for now
    public static bool IdIsInvalidPrecise(long id, int len)
    {
        for (var i = 1; i < len; i++)
        {
            if (len % i != 0) continue;

            var str = id.ToString();
            var chunks = str.Chunk(i).ToArray();
            if (chunks.All(x => x.SequenceEqual(chunks[0]))) return true;
        }
        return false;
    }
}