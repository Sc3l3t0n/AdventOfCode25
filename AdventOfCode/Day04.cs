namespace AdventOfCode;

public sealed class Day04 : BaseDay
{
    private readonly string _input;

    public Day04()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private List<List<Tile>> ParseInput() =>
        _input.Lines().Select(x => x.Select(GetTile).ToList()).ToList();

    public override ValueTask<string> Solve_1() => ToAdjutantCountList(ParseInput()).ToSolution();

    public override ValueTask<string> Solve_2() => ToAdjutantCountListMultipleRuns(ParseInput()).ToSolution();

    private enum Tile
    {
        Empty,
        PaperRoll,
    }

    private static Tile GetTile(char c) => c switch
    {
        '.' => Tile.Empty,
        '@' => Tile.PaperRoll,
        _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
    };

    private readonly List<(int xAdd, int yAdd)> _addCords =
    [
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1),
    ];

    private bool IsMoveable(List<List<Tile>> tiles, int x, int y)
        => _addCords
            .Where(c =>
                x + c.xAdd < tiles.Count &&
                x + c.xAdd >= 0 &&
                y + c.yAdd < tiles.Count &&
                y + c.yAdd >= 0
            ).Count(c => tiles[y + c.yAdd][x + c.xAdd] == Tile.PaperRoll) < 4;

    private int ToAdjutantCountList(List<List<Tile>> tiles)
    {
        var moveable = 0;
        for (var y = 0; y < tiles.Count; y++)
        for (var x = 0; x < tiles[y].Count; x++)
        {
            if (tiles[y][x] == Tile.Empty) continue;
            if (IsMoveable(tiles, x, y)) moveable++;
        }
        
        return moveable;
    }

    private int ToAdjutantCountListMultipleRuns(List<List<Tile>> tiles)
    {
        var someThingChanged = false;
        var moveable = 0;
        var newGrid = tiles.ToList();

        for (var y = 0; y < tiles.Count; y++)
        for (var x = 0; x < tiles[y].Count; x++)
        {
            if (tiles[y][x] == Tile.Empty) continue;
            
            if (!IsMoveable(tiles, x, y)) continue;
            
            moveable++;
            newGrid[y][x] = Tile.Empty;
            someThingChanged = true;
        }

        if (someThingChanged) moveable += ToAdjutantCountListMultipleRuns(newGrid);

        return moveable;
    }
}