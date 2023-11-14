namespace Considition2023_Cs.Optimizers;

internal abstract class OptimizerBase : IOptimizer
{
    protected readonly GeneralData _generalData;
    protected readonly MapData _mapData;

    protected OptimizerBase(GeneralData generalData, MapData mapData)
    {
        _generalData = generalData;
        _mapData = mapData;
    }


    public abstract double Optimize(Dictionary<string, PlacedLocations> locations, double currentScore, ref int optimizeRun);
}