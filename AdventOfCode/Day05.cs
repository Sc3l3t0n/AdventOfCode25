namespace AdventOfCode;

public sealed class Day05 : BaseDay
{
    private readonly string _input;

    public Day05()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private record Range(long Start, long End);

    private Range GetFreshIdRange(string input)
    {
        var splitted = input.Split('-');
        var start = long.Parse(splitted[0]);
        var end = long.Parse(splitted[1]);
        return new Range(start, end);
    }

    private (List<Range>, List<long>) ParseInput()
    {
        var inputSplit = _input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var (freshIds, ingredients) = (inputSplit[0], inputSplit[1]);

        var freshIdsSet = freshIds.Lines().Select(GetFreshIdRange).ToList();
        var ingredientIds = ingredients.Lines().Select(long.Parse).ToList();

        return (freshIdsSet, ingredientIds);
    }

    public override ValueTask<string> Solve_1() => GetFreshIngredients(ParseInput()).ToSolution();

    public override ValueTask<string> Solve_2() => GetFreshIdCount(ParseInput().Item1).ToSolution();

    private static long GetFreshIngredients((List<Range> freshIdRanges, List<long> ingredients) input)
        => input.ingredients.Count(id => input.freshIdRanges.Any(r => r.Start <= id && id <= r.End));

    private enum ConstrainType
    {
        Start,
        End
    }

    private record RangeConstrain(ConstrainType Type, long Value);

    private static long GetFreshIdCount(List<Range> freshIdRanges)
    {
        var constrains = freshIdRanges
            .SelectMany<Range, RangeConstrain>(range =>
            [
                new(ConstrainType.Start, range.Start),
                new(ConstrainType.End, range.End)
            ])
            .GroupBy(x => x.Value)
            .SelectMany(x =>
            {
                var types = x.Select(c => c.Type).ToArray();
                switch (types.Length)
                {
                    case 2 when types.SequenceEqual([ConstrainType.End, ConstrainType.Start]):
                        return [];
                    case 3:
                        var significant = types
                            .GroupBy(i => i)
                            .OrderByDescending(grp => grp.Count())
                            .Select(grp => grp.Key)
                            .First();
                        return [new RangeConstrain(significant, x.Key)];
                    default:
                        return x;
                }
            })
            .OrderBy(x => x.Value);

        long? currentStart = null;
        var depth = 0;
        List<Range> newRanges = [];

        foreach (var constrain in constrains)
        {
            switch (constrain.Type)
            {
                case ConstrainType.Start:
                    depth++;
                    currentStart ??= constrain.Value;
                    break;
                case ConstrainType.End:
                {
                    depth--;
                    if (depth == 0)
                    {
                        newRanges.Add(new Range(currentStart!.Value, constrain.Value));
                        currentStart = null;
                    }

                    break;
                }
            }
        }

        return newRanges.Sum(x => x.End - x.Start + 1);
    }
}