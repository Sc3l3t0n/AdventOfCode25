namespace AdventOfCode;

public sealed class Day01 : BaseDay
{
    private readonly string _input;

    public Day01()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private IEnumerable<Rotation> ParseInput() =>
        _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseRotation);

    public override ValueTask<string> Solve_1() =>
        ParseInput()
            .Aggregate(new NobState(50, 0), TurnNob)
            .OnZero.ToSolution();

    public override ValueTask<string> Solve_2() =>
        ParseInput()
            .Aggregate(new NobState(50, 0), TurnNobPrecise)
            .OnZero.ToSolution();

    private record struct NobState(int Value, int OnZero);

    private enum Direction
    {
        Right,
        Left
    }

    private record struct Rotation(Direction Direction, int Value);

    private static Rotation ParseRotation(string input)
    {
        var direction = input[0] switch
        {
            'R' => Direction.Right,
            'L' => Direction.Left,
            _ => throw new ArgumentException("Rotation input need to start with R/L"),
        };
        var value = int.Parse(input[1..]);
        return new Rotation(direction, value);
    }

    private static int ApplyRotation(int state, Rotation rotation) => state + (rotation.Direction switch
    {
        Direction.Right => +rotation.Value,
        Direction.Left => -rotation.Value,
        _ => throw new ArgumentOutOfRangeException(nameof(rotation.Direction))
    });

    private static void NormalizeNobState(ref int state) => state = state switch
    {
        < 0 => state + 100,
        >= 100 => state - 100,
        _ => state
    };

    private static NobState TurnNob(NobState state, Rotation rotation)
    {
        var newState = ApplyRotation(state.Value, rotation);

        NormalizeNobState(ref newState);
        newState %= 100;

        if (newState == 0) return new NobState(newState, state.OnZero + 1);

        return state with { Value = newState };
    }

    private static NobState TurnNobPrecise(NobState state, Rotation rotation)
    {
        var fullRotations = rotation.Value / 100;
        var rest = rotation.Value - 100 * fullRotations;

        if (rest == 0) return state with { OnZero = fullRotations };

        var onZero = fullRotations;

        var newState = ApplyRotation(state.Value, rotation with { Value = rest });
        if (newState is <= 0 or >= 100 && state.Value != 0) onZero++;

        NormalizeNobState(ref newState);

        return new NobState(newState, state.OnZero + onZero);
    }
}