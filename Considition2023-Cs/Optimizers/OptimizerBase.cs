namespace Considition2023_Cs.Optimizers;

internal abstract class OptimizerBase : IOptimizer
{
    protected readonly GeneralData _generalData;
    protected readonly MapData _mapData;
    protected readonly OptimizerSort _sort;

    public List<OptimizerAction> _optimizationFunctions;

    protected OptimizerBase(GeneralData generalData, MapData mapData, OptimizerSort sort)
    {
        _generalData = generalData;
        _mapData = mapData;
        _sort = sort;
    }

    public abstract double Optimize(
        Dictionary<string, PlacedLocations> locations, 
        double currentScore, 
        ref int optimizeRun);
}