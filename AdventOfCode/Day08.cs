namespace AdventOfCode;

public sealed class Day08 : BaseDay
{
    private readonly string _input;

    public Day08()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private IEnumerable<JunctionBox> ParseInput() =>
        _input.Lines().Select(x =>
        {
            var coords = x.Split(',').Select(int.Parse).ToArray();
            return new JunctionBox(coords[0], coords[1], coords[2]);
        });

    private sealed record JunctionBox(int X, int Y, int Z)
    {
        public double DistanceTo(JunctionBox other) => Math.Sqrt(
            Math.Pow(X - other.X, 2) +
            Math.Pow(Y - other.Y, 2) +
            Math.Pow(Z - other.Z, 2)
        );
    }

    public override ValueTask<string> Solve_1() => CalculateNetworks(ParseInput().ToList()).ToSolution();

    public override ValueTask<string> Solve_2() => CalculateSingleNetwork(ParseInput().ToList()).ToSolution();

    private static int CalculateNetworks(List<JunctionBox> junctionBoxes)
    {
        Dictionary<JunctionBox, HashSet<JunctionBox>> networkMap = [];

        foreach (var junctionBox in junctionBoxes) networkMap.Add(junctionBox, [junctionBox]);
        var nearestPairs = junctionBoxes
            .SelectMany((a, i) => junctionBoxes.Skip(i + 1).Select(b => (distance: a.DistanceTo(b), a, b)))
            .OrderBy(x => x.distance)
            .Take(1000);

        foreach (var pair in nearestPairs)
        {
            var networkA = networkMap[pair.a];
            var networkB = networkMap[pair.b];

            if (networkA == networkB) continue;

            var newNetwork = networkA.Union(networkB).ToHashSet();
            foreach (var junctionBox in newNetwork) networkMap[junctionBox] = newNetwork;
        }

        return networkMap.Values
            .Distinct()
            .Select(x => x.Count)
            .OrderDescending()
            .Take(3)
            .Aggregate(1, (acc, x) => acc * x);
    }
    
    private static long CalculateSingleNetwork(List<JunctionBox> junctionBoxes)
    {
        Dictionary<JunctionBox, HashSet<JunctionBox>> networkMap = [];
        var junctionsCount = junctionBoxes.Count;

        foreach (var junctionBox in junctionBoxes) networkMap.Add(junctionBox, [junctionBox]);
        var nearestPairs = junctionBoxes
            .SelectMany((a, i) => junctionBoxes.Skip(i + 1).Select(b => (distance: a.DistanceTo(b), a, b)))
            .OrderBy(x => x.distance);

        foreach (var pair in nearestPairs)
        {
            var networkA = networkMap[pair.a];
            var networkB = networkMap[pair.b];

            if (networkA == networkB) continue;

            var newNetwork = networkA.Union(networkB).ToHashSet();
            if (newNetwork.Count == junctionsCount) return (long)pair.a.X * (long)pair.b.X;
            foreach (var junctionBox in newNetwork) networkMap[junctionBox] = newNetwork;
        }

        throw new Exception();
    }
}