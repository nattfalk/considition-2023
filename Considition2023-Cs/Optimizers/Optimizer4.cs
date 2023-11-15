namespace Considition2023_Cs.Optimizers;

internal class Optimizer4 : OptimizerBase
{
    public Optimizer4(GeneralData generalData, MapData mapData, OptimizerSort sort) 
        : base(generalData, mapData, sort)
    {
        _optimizationFunctions = new List<OptimizerAction>
        {
            new()
            {
                Optimizer = loc =>
                {
                    loc.Freestyle9100Count = 0;
                    loc.Freestyle3100Count = 0;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Freestyle9100Count = 0;
                    loc.Freestyle3100Count = 1;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Freestyle9100Count = 0;
                    loc.Freestyle3100Count = 2;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Freestyle9100Count = 1;
                    loc.Freestyle3100Count = 0;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Freestyle9100Count = 2;
                    loc.Freestyle3100Count = 0;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Freestyle9100Count = 1;
                    loc.Freestyle3100Count = 2;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Freestyle9100Count = 2;
                    loc.Freestyle3100Count = 1;
                },
            },
        };
    }

    public override double Optimize(
        Dictionary<string, PlacedLocations> locations, 
        double currentScore, 
        ref int optimizeRun)
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


