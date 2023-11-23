using Considition2023_Cs.Optimizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs;

internal class NormalMapSolver
{
    private readonly GeneralData _generalData;

    public NormalMapSolver(GeneralData generalData)
    {
        _generalData = generalData;
    }

    public Dictionary<string, PlacedLocations> Solve(MapData mapData)
    {
        Dictionary<string, PlacedLocations> locations = new();
        SubmitSolution solution = new()
        {
            Locations = null
        };

        solution.Locations = LocationsHelper.GetUsedLocations(LocationsHelper.InitializeLocations(mapData.locations, locations));

        GameData score = null;
        if (solution.Locations.Count > 0)
            score = new Scoring().CalculateScore(mapData.MapName, solution, mapData, _generalData);
        Console.SetCursorPosition(0, 7);
        Console.WriteLine($"Map: {mapData.MapName}, Initial GameScore: {score?.GameScore?.Total ?? 0d}");
        var scoreValue = score?.GameScore?.Total ?? 0d;

        var optimizeRunCount = 1;
        var optimizers = new HashSet<IOptimizer>()
        {
            new Optimizer10(_generalData, mapData, OptimizerSort.None),
            new Optimizer10(_generalData, mapData, OptimizerSort.Ascending),
            new Optimizer10(_generalData, mapData, OptimizerSort.Descending),
            new Optimizer11(_generalData, mapData, OptimizerSort.None),
            new Optimizer11(_generalData, mapData, OptimizerSort.Ascending),
            new Optimizer11(_generalData, mapData, OptimizerSort.Descending),
            //new Optimizer12(_generalData, mapData, OptimizerSort.None),
            //new Optimizer12(_generalData, mapData, OptimizerSort.Ascending),
            //new Optimizer12(_generalData, mapData, OptimizerSort.Descending),
            //new Optimizer13(generalData, mapData, OptimizerSort.None),
            //new Optimizer13(generalData, mapData, OptimizerSort.Ascending),
            //new Optimizer13(generalData, mapData, OptimizerSort.Descending),
            
            //new Optimizer2(generalData, mapData),       // Linköping 699.57
            //new Optimizer3_sorted_dec(generalData, mapData),    // Göteborg, 6147.40, G-Sandbox, 2342.08
            //new Optimizer3_sorted(generalData, mapData),    // Uppsala, 2412.25, Västerås, 1498.38
            //new Optimizer1(generalData, mapData),
            //new Optimizer3(generalData, mapData),
            
            //new Optimizer3_2(generalData, mapData),
            //new Optimizer4(generalData, mapData),
            //new Optimizer5(generalData, mapData),
            //new Optimizer6(generalData, mapData),
            //new Optimizer6_2(generalData, mapData)
            //new Optimizer6_sorted(generalData, mapData),
        };

        Dictionary<string, PlacedLocations> currentBestLocations = new();
        while (true)
        {
            var previousScore = scoreValue;
            foreach (var optimizer in optimizers)
            {
                var tempLocations = LocationsHelper.InitializeLocations(mapData.locations, locations);
                var newScore = optimizer.Optimize(tempLocations, scoreValue, ref optimizeRunCount);
                if (newScore > previousScore)
                {
                    previousScore = newScore;
                    currentBestLocations = LocationsHelper.CopyLocations(tempLocations);
                }
                //var tempLocations = LocationsHelper.InitializeLocations(mapData.locations, locations);
                //var newScore = optimizer.Optimize(tempLocations, scoreValue, ref optimizeRunCount);
                //if (newScore >= previousScore)
                //{
                //    previousScore = newScore;
                //    scoreValue = newScore;
                //    currentBestLocations = LocationsHelper.CopyLocations(tempLocations);
                //    locations = LocationsHelper.CopyLocations(tempLocations);
                //}

                Console.SetCursorPosition(0, 8);
                Console.WriteLine($"- Optimize step: {optimizeRunCount,3:0}, New score: {scoreValue,11:#.00}");
            }

            if (Math.Abs(previousScore - scoreValue) < 0.0000001d) break;
            locations = currentBestLocations;
            scoreValue = previousScore;
        }

        return locations;
    }
}