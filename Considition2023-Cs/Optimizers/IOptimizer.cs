namespace Considition2023_Cs.Optimizers;

internal interface IOptimizer
{
    double Optimize(
        Dictionary<string, PlacedLocations> locations, 
        double currentScore, 
        ref int optimizeRun);
}