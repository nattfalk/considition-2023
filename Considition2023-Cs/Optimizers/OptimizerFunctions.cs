namespace Considition2023_Cs.Optimizers;

internal static class OptimizerFunctions
{
    internal static double OptimizeByFunction(
        GeneralData generalData,
        MapData mapData,
        Dictionary<string, PlacedLocations> locations,
        double currentScoreValue,
        OptimizerAction processLocation)
    {
        var scoreValue = currentScoreValue;
        foreach (var location in locations)
        {
            var old3100 = location.Value.Freestyle3100Count;
            var old9100 = location.Value.Freestyle9100Count;

            processLocation.Optimizer(location.Value);

            var solution = new SubmitSolution
            {
                Locations = LocationsHelper.GetUsedLocations(locations)
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
                processLocation.UsageCount++;
            }
        }

        return scoreValue;
    }

    internal static double OptimizeByLongLat(
        GeneralData generalData,
        MapData mapData,
        Dictionary<string, PlacedLocations> locations,
        double currentScoreValue,
        OptimizerAction processLocation)
    {
        var scoreValue = currentScoreValue;
        foreach (var location in locations)
        {
            var oldLong = location.Value.Longitude;
            var oldLat = location.Value.Latitude;

            processLocation.Optimizer(location.Value);

            var solution = new SubmitSolution
            {
                Locations = LocationsHelper.GetUsedLocations(locations)
            };

            var score = new Scoring().CalculateScore(mapData.MapName, solution, mapData, generalData);
            if (scoreValue >= score.GameScore!.Total)
            {
                location.Value.Longitude = oldLong;
                location.Value.Latitude = oldLat;
            }
            else
            {
                scoreValue = score.GameScore.Total;
                processLocation.UsageCount++;
            }
        }

        return scoreValue;
    }

    internal static double OptimizeByLocation(
        GeneralData generalData,
        MapData mapData,
        Dictionary<string, PlacedLocations> locations,
        double currentScoreValue,
        List<OptimizerAction> processLocationFunctions)
    {
        var scoreValue = currentScoreValue;
        foreach (var location in locations)
        {
            foreach (var processFunc in processLocationFunctions)
            {
                var old3100 = location.Value.Freestyle3100Count;
                var old9100 = location.Value.Freestyle9100Count;

                processFunc.Optimizer(location.Value);

                var solution = new SubmitSolution
                {
                    Locations = LocationsHelper.GetUsedLocations(locations)
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
                    processFunc.UsageCount++;
                }
            }
        }

        return scoreValue;
    }
}