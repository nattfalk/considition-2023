namespace Considition2023_Cs.Optimizers;

internal class Optimizer6 : OptimizerBase
{
    private readonly List<Action<PlacedLocations>> _optimizationFunctions;
    
    public Optimizer6(GeneralData generalData, MapData mapData) 
        : base(generalData, mapData)
    {
        _optimizationFunctions = new List<Action<PlacedLocations>>
        {
            loc =>
            {
                loc.Freestyle9100Count = 0;
                loc.Freestyle3100Count = 0;
            },
            loc =>
            {
                loc.Freestyle9100Count = 0;
                loc.Freestyle3100Count = 1;
            },
            loc =>
            {
                loc.Freestyle9100Count = 0;
                loc.Freestyle3100Count = 2;
            },
            loc =>
            {
                loc.Freestyle9100Count = 1;
                loc.Freestyle3100Count = 0;
            },
            loc =>
            {
                loc.Freestyle9100Count = 2;
                loc.Freestyle3100Count = 0;
            },
            loc =>
            {
                loc.Freestyle9100Count = 1;
                loc.Freestyle3100Count = 2;
            },
            loc =>
            {
                loc.Freestyle9100Count = 2;
                loc.Freestyle3100Count = 1;
            },
            loc =>
            {
                loc.Freestyle9100Count = 2;
                loc.Freestyle3100Count = 2;
            },

            loc =>
            {
                loc.Freestyle9100Count = Math.Max(loc.Freestyle9100Count - 1, 0);
                loc.Freestyle3100Count = Math.Min(loc.Freestyle3100Count + 1, 2);
            },
            loc =>
            {
                loc.Freestyle3100Count = Math.Max(loc.Freestyle3100Count - 1, 0);
            },
            loc =>
            {
                loc.Freestyle3100Count = Math.Min(loc.Freestyle3100Count + 1, 2);
            },
            loc =>
            {
                loc.Freestyle3100Count = Math.Max(loc.Freestyle3100Count - 1, 0);
                loc.Freestyle9100Count = Math.Min(loc.Freestyle9100Count + 1, 2);
            },
            loc =>
            {
                loc.Freestyle9100Count = Math.Max(loc.Freestyle9100Count - 1, 0);
            },
            loc =>
            {
                loc.Freestyle9100Count = Math.Min(loc.Freestyle9100Count + 1, 2);
            }
        };
    }

    public override double Optimize(Dictionary<string, PlacedLocations> locations, double currentScore, ref int optimizeRun)
    {
        var scoreValue = currentScore;
        while (true)
        {
            var previousScore = scoreValue;

            scoreValue = OptimizerFunctions.OptimizeByLocation(_generalData, _mapData, locations, scoreValue, _optimizationFunctions);
            Console.SetCursorPosition(0, 8);
            Console.WriteLine($"- Optimize step: {optimizeRun++,3:0}, New score: {scoreValue,11:#.00}");

            if (Math.Abs(previousScore - scoreValue) < 0.0000001d) break;
        }

        return scoreValue;
    }
}


