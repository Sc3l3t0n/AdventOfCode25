namespace AdventOfCode;

public sealed class Day06 : BaseDay
{
    private readonly string _input;

    private const string testInput =
        """
        123 328  51 64 
         45 64  387 23 
          6 98  215 314
        *   +   *   +  
        """;

    public Day06()
    {
        _input = false ? testInput : File.ReadAllText(InputFilePath);
    }

    private enum Operators
    {
        Add,
        Multiply,
    }

    private static Operators ParseOperator(string input) => ParseOperator(input[0]);

    private static Operators ParseOperator(char input) => input switch
    {
        '+' => Operators.Add,
        '*' => Operators.Multiply,
        _ => throw new ArgumentOutOfRangeException()
    };

    private sealed record Equations(List<List<long>> Operants, List<Operators> Operators);

    private Equations ParseInput()
    {
        var lines = _input.Lines().ToList();

        var operators = lines
            .Last()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ParseOperator).ToList();

        var operants = lines[..^1]
            .Select(x =>
                x.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(long.Parse).ToList()
            ).ToList();

        return new Equations(operants, operators);
    }

    private Equations ParseInputVertically()
    {
        var lines = _input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        Span<char> vertical = stackalloc char[100];

        List<Operators> allOperators = [];
        List<List<long>> allOperants = [];

        var startIndex = 0;
        var lineLength = lines.First().Length;
        for (var c = 0; c <= lineLength; c++)
        {
            var endOfEquation = c == lineLength || lines.All(x => x[c] == ' ');

            if (!endOfEquation) continue;
            
            var @operator = ParseOperator(lines.Last()[startIndex]);
            var operantStrs = lines[..^1].Select(x => x[startIndex..c]).ToList();

            List<long> operants = [];

            var operantsLen = operantStrs.Count;
            for (var i = operantStrs.First().Length - 1; i >= 0; i--)
            {
                for (var k = 0; k < operantsLen; k++)
                {
                    var operant = operantStrs[k][i];
                    vertical[k] = operant;
                }

                operants.Add(long.TryParse(vertical[..operantsLen].Trim(), out var result)? result : 0);
            }
            
            allOperators.Add(@operator);
            allOperants.Add(operants);
            
            startIndex = c + 1;
        }

        return new Equations(allOperants, allOperators);
    }

    public override ValueTask<string> Solve_1() => SumUpEquations(ParseInput()).ToSolution();

    public override ValueTask<string> Solve_2() => SumUpEquationsVertically(ParseInputVertically()).ToSolution();

    private static long SumUpEquations(Equations equations)
    {
        return equations.Operators.Select((t, i) => t switch
            {
                Operators.Add => equations.Operants.Sum(x => x[i]),
                Operators.Multiply => equations.Operants.Aggregate(1L, (agg, ops) => agg * ops[i]),
                _ => throw new ArgumentOutOfRangeException(),
            })
            .Sum();
    }
    
    private static long SumUpEquationsVertically(Equations equations)
    {
        return equations.Operators.Select((t, i) => t switch
            {
                Operators.Add => equations.Operants[i].Sum(),
                Operators.Multiply => equations.Operants[i].Aggregate(1L, (agg, ops) => agg * ops),
                _ => throw new ArgumentOutOfRangeException(),
            })
            .Sum();
    }
}