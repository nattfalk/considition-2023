namespace Considition2023_Cs.Optimizers;

internal class Optimizer12 : OptimizerBase
{
    public Optimizer12(GeneralData generalData, MapData mapData, OptimizerSort sort)
        : base(generalData, mapData, sort)
    {
        _optimizationFunctions = new List<OptimizerAction>
        {
            new()
            {
                Optimizer = loc =>
                {
                    if (loc.LocationType != "Grocery-store" && loc.LocationType != "Kiosk") return;
                    loc.Freestyle3100Count = Math.Max(loc.Freestyle3100Count - 1, 0);
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    if (loc.LocationType != "Grocery-store" && loc.LocationType != "Kiosk") return;
                    loc.Freestyle3100Count = Math.Max(loc.Freestyle3100Count - 1, 0);
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    if (loc.LocationType != "Grocery-store" && loc.LocationType != "Kiosk") return;
                    loc.Freestyle3100Count = Math.Min(loc.Freestyle3100Count + 1, 1);
                },
            },
            new()
            {
                Optimizer = loc =>
                {
                    if (loc.LocationType != "Grocery-store" && loc.LocationType != "Kiosk") return;
                    loc.Freestyle3100Count = Math.Min(loc.Freestyle3100Count + 1, 1);
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
                scoreValue = OptimizerFunctions.OptimizeByFunction(_generalData, _mapData, locations, scoreValue, optimizationFunction);
                Console.SetCursorPosition(0, 8);
                Console.WriteLine($"- Optimize step: {optimizeRun++,3:0}, New score: {scoreValue,11:#.00}");
            }

            if (Math.Abs(previousScore - scoreValue) < 0.0000001d) break;
        }

        return scoreValue;
    }
}


