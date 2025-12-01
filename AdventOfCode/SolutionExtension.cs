namespace AdventOfCode;

public static class SolutionExtension
{
    extension<T>(T solution)
    {
        public ValueTask<string> ToSolution() => new(solution.ToString());
    }
}