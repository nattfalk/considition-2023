using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Optimizers;

internal static class OptimizerFunctions
{
    internal static double OptimizeByFunction(
        GeneralData generalData,
        MapData mapData,
        Dictionary<string, PlacedLocations> locations,
        double currentScoreValue,
        Action<PlacedLocations> processLocation)
    {
        var scoreValue = currentScoreValue;
        foreach (var location in locations)
        {
            var old3100 = location.Value.Freestyle3100Count;
            var old9100 = location.Value.Freestyle9100Count;

            processLocation(location.Value);

            var solution = new SubmitSolution
            {
                Locations = locations
                    .Where(x => x.Value.Freestyle3100Count > 0 || x.Value.Freestyle9100Count > 0)
                    .ToDictionary(x => x.Key, y => y.Value)
            };

            var score = new Scoring().CalculateScore(mapData.MapName, solution, mapData, generalData);
            if (scoreValue >= score.GameScore!.Total)
            {
                location.Value.Freestyle3100Count = old3100;
                location.Value.Freestyle9100Count = old9100;
            }
            else
            {
                scoreValue = score.GameScore.Total;
            }
        }

        return scoreValue;
    }

    internal static double OptimizeByLocation(
        GeneralData generalData,
        MapData mapData,
        Dictionary<string, PlacedLocations> locations,
        double currentScoreValue,
        List<Action<PlacedLocations>> processLocationFunctions)
    {
        var scoreValue = currentScoreValue;
        foreach (var location in locations)
        {
            foreach (var processFunc in processLocationFunctions)
            {
                var old3100 = location.Value.Freestyle3100Count;
                var old9100 = location.Value.Freestyle9100Count;

                processFunc(location.Value);

                var solution = new SubmitSolution
                {
                    Locations = locations
                        .Where(x => x.Value.Freestyle3100Count > 0 || x.Value.Freestyle9100Count > 0)
                        .ToDictionary(x => x.Key, y => y.Value)
                };

                var score = new Scoring().CalculateScore(mapData.MapName, solution, mapData, generalData);
                if (scoreValue >= score.GameScore!.Total)
                {
                    location.Value.Freestyle3100Count = old3100;
                    location.Value.Freestyle9100Count = old9100;
                }
                else
                {
                    scoreValue = score.GameScore.Total;
                }
            }
        }

        return scoreValue;
    }
}