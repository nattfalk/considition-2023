namespace Considition2023_Cs.Optimizers;

internal class Optimizer13 : OptimizerBase
{
    public Optimizer13(GeneralData generalData, MapData mapData, OptimizerSort sort)
        : base(generalData, mapData, sort)
    {
        _optimizationFunctions = new List<OptimizerAction>
        {
            new()
            {
                Optimizer = loc =>
                {
                    loc.Longitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0010d;
                    loc.Latitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0010d;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Longitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0010d;
                    loc.Latitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0010d;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Longitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0015d;
                    loc.Latitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0015d;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Longitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0015d;
                    loc.Latitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0015d;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Longitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0020d;
                    loc.Latitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0020d;
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    loc.Longitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0020d;
                    loc.Latitude += (Random.Shared.NextDouble() * 2d - 1d) * 0.0020d;
                },
            },
        };
    }

    public override double Optimize(
        Dictionary<string, PlacedLocations> locations,
        double currentScore,
        ref int optimizeRun)
    {

        if (_mapData.locations.Count > 0)
        {
            locations = _sort switch
            {
                OptimizerSort.Ascending => locations
                    .OrderBy(x => _mapData.locations[x.Key].SalesVolume * _mapData.locations[x.Key].Footfall)
                    .ToDictionary(x => x.Key, y => y.Value),
                OptimizerSort.Descending => locations
                    .OrderByDescending(x => _mapData.locations[x.Key].SalesVolume * _mapData.locations[x.Key].Footfall)
                    .ToDictionary(x => x.Key, y => y.Value),
                _ => locations
            };
        }
        else
        {
            locations = _sort switch
            {
                OptimizerSort.Ascending => locations
                    .OrderBy(x => x.Value.Spread / x.Value.Footfall)
                    .ToDictionary(x => x.Key, y => y.Value),
                OptimizerSort.Descending => locations
                    .OrderByDescending(x => x.Value.Spread * x.Value.Footfall)
                    .ToDictionary(x => x.Key, y => y.Value),
                _ => locations
            };
        }

        var scoreValue = currentScore;
        while (true)
        {
            var previousScore = scoreValue;

            foreach (var optimizationFunction in _optimizationFunctions)
            {
                scoreValue = OptimizerFunctions.OptimizeByLongLat(_generalData, _mapData, locations, scoreValue, optimizationFunction);
                Console.SetCursorPosition(0, 8);
                Console.WriteLine($"- Optimize step: {optimizeRun++,3:0}, New score: {scoreValue,11:#.00}");

                var solution = new SubmitSolution
                {
                    Locations = LocationsHelper.GetUsedLocations(locations)
                };
                var score = new Scoring().CalculateScore(_mapData.MapName, solution, _mapData, _generalData);
                Console.WriteLine($"- Validation step: {(optimizeRun - 1),3:0}, New score: {scoreValue,11:#.00}");
            }

            if (Math.Abs(previousScore - scoreValue) < 0.0000001d) break;
        }

        return scoreValue;
    }
}


