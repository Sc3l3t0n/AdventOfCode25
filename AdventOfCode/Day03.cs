namespace AdventOfCode;

public sealed class Day03 : BaseDay
{
    private readonly string _input;

    public Day03()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private IEnumerable<Bank> ParseInput()
        => _input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Bank.Parse);


    public override ValueTask<string> Solve_1() =>
        ParseInput().Aggregate<Bank, long>(0, FindHighestPossiblePowerWith2Digits).ToSolution();

    public override ValueTask<string> Solve_2() =>
        ParseInput().Aggregate<Bank, long>(0, FindHighestPossiblePowerWith12Digits).ToSolution();

    private sealed record Bank(List<int> Batteries)
    {
        public static Bank Parse(string input) => new(input.Select(x => x - '0').ToList());
    }

    private static long FindHighestPossiblePowerWith2Digits(long previousSum, Bank bank)
        => FindHighestPossiblePower(previousSum, bank, 2);

    private static long FindHighestPossiblePowerWith12Digits(long previousSum, Bank bank)
        => FindHighestPossiblePower(previousSum, bank, 12);

    private static long FindHighestPossiblePower(long previousSum, Bank bank, int digits)
    {
        long max = 0;
        var nextStartIndex = 0;

        for (var i = 0; i < digits; i++)
        {
            var newMax = bank.Batteries[nextStartIndex..^(digits - (i + 1))].Index().MaxBy(x => x.Item);
            max += newMax.Item * (long)Math.Pow(10, digits - 1 - i);
            nextStartIndex += newMax.Index + 1;
        }

        return previousSum + max;
    }
}