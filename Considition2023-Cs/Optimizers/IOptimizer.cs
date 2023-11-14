using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Optimizers;

internal interface IOptimizer
{
    double Optimize(Dictionary<string, PlacedLocations> locations, double currentScore, ref int optimizeRun);
}