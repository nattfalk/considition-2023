using Newtonsoft.Json.Serialization;

namespace Considition2023_Cs;

internal static class LocationsHelper
{
    private static double _3100Capacity;
    private static MapData _mapData;

    public static void InitializeHelper(GeneralData generalData, MapData mapData)
    {
        _3100Capacity = generalData.Freestyle3100Data.RefillCapacityPerWeek;
        _mapData = mapData;
    }

    public static Dictionary<string, PlacedLocations> InitializeLocations(
        Dictionary<string, StoreLocation> inputLocations,
        IReadOnlyDictionary<string, PlacedLocations> calculatedLocations)
    {
        var initializedLocations = new Dictionary<string, PlacedLocations>();
        if (inputLocations.Count > 0)
        {
            foreach (var (key, location) in inputLocations)
            {
                var calculatedLocation = calculatedLocations.TryGetValue(key, out var location1) ? location1 : null;
                initializedLocations[location.LocationName] = new PlacedLocations
                {
                    Freestyle3100Count = calculatedLocation?.Freestyle3100Count ??
                                         (location.SalesVolume <= _3100Capacity ? 1 : 0),
                    Freestyle9100Count = calculatedLocation?.Freestyle9100Count ??
                                         (location.SalesVolume > _3100Capacity ? 1 : 0)
                };
            }
        }
        else
        {
            foreach (var location in calculatedLocations)
            {
                initializedLocations[location.Key] = new PlacedLocations
                {
                    Freestyle3100Count = location.Value.Freestyle3100Count,
                    Freestyle9100Count = location.Value.Freestyle9100Count,
                    Latitude = location.Value.Latitude,
                    Longitude = location.Value.Longitude,
                    LocationType = location.Value.LocationType,
                    Footfall = location.Value.Footfall,
                    Spread = location.Value.Spread
                };
            }
        }

        return initializedLocations;
    }

    public static Dictionary<string, PlacedLocations> CopyLocations(
        Dictionary<string, PlacedLocations> inputLocations)
    {
        var copiedLocations = new Dictionary<string, PlacedLocations>();
        foreach (var kvp in inputLocations)
        {
            copiedLocations[kvp.Key] = new PlacedLocations
            {
                Freestyle3100Count = kvp.Value.Freestyle3100Count,
                Freestyle9100Count = kvp.Value.Freestyle9100Count,
                LocationType = kvp.Value.LocationType,
                Longitude = kvp.Value.Longitude,
                Latitude = kvp.Value.Latitude
            };
        }

        return copiedLocations;
    }

    public static Dictionary<string, PlacedLocations> GetUsedLocations(IReadOnlyDictionary<string, PlacedLocations> placedLocations) =>
            placedLocations
            .Where(x => x.Value.Freestyle3100Count > 0 || x.Value.Freestyle9100Count > 0)
            .Select(x =>
            {
                x.Value.Latitude = Math.Max(Math.Min(x.Value.Latitude, _mapData.Border.LatitudeMax),
                    _mapData.Border.LatitudeMin);
                x.Value.Longitude = Math.Max(Math.Min(x.Value.Longitude, _mapData.Border.LongitudeMax),
                    _mapData.Border.LongitudeMin);
                return x;
            })
            .ToDictionary(x => x.Key, y => y.Value);
}