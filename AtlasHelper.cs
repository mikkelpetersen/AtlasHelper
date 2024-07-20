using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using SharpDX;
using MapComponent = ExileCore.PoEMemory.Components.Map;

namespace AtlasHelper;

public class AtlasHelper : BaseSettingsPlugin<AtlasHelperSettings>
{
    internal static AtlasHelper Instance;

    private readonly TimeCache<List<Map>> _mapsCache;

    private int _highestCompletedTier;

    public AtlasHelper()
    {
        Name = "Atlas Helper";

        _mapsCache = new TimeCache<List<Map>>(() => Inventory.GetMapsFromInventory, 100);
    }

    public override bool Initialise()
    {
        Instance ??= this;
        
        Graphics.InitImage(Path.Combine(DirectoryFullName, "Images", "Transmutation.png"), false);
        Graphics.InitImage(Path.Combine(DirectoryFullName, "Images", "Alchemy.png"), false);
        Graphics.InitImage(Path.Combine(DirectoryFullName, "Images", "Regal.png"), false);
        Graphics.InitImage(Path.Combine(DirectoryFullName, "Images", "Vaal.png"), false);

        return true;
    }

    public override void AreaChange(AreaInstance area)
    {
        UpdateHighestCompletedTier();
        Log.Debug($"Highest Completed Tier: {_highestCompletedTier}");
    }

    public override Job Tick()
    {
        return null;
    }

    public override void Render()
    {
        if (!Inventory.IsVisible)
            return;
        
        /*
         * We follow the following logic to determine which Map to Run (Highlight):
         * 1. Highlight the Map with the lowest Tier that is at least two Tiers below the highest Completed Tier and is not Completed with Bonus.
         * 1a. If there are multiple Maps with the same Tier, highlight the one with the most uncompleted Connections.
         * 2. If there are no Maps to Highlight, then we highlight the Map with the lowest Tier that is not Completed with Bonus.
         * 2a. If there are multiple Maps with the same Tier, highlight the one with the most uncompleted Connections.
         * 3. If there are no Maps to Highlight, then we highlight the Map with the lowest Tier.
         * 3a. If there are multiple Maps with the same Tier, highlight the one with the most uncompleted Connections.
         */

        var mapsToHighlight = _mapsCache.Value
            .Where(x => x is { IsNormal: true, IsBonusCompleted: false } && x.Tier <= _highestCompletedTier - 2)
            .OrderBy(x => x.Tier)
            .ToList();

        switch (mapsToHighlight.Count)
        {
            // No Maps to Highlight. Continue.
            case 0:
                break;
            // There is only one Map to Highlight. Highlight it.
            case 1:
                HighlightMap(mapsToHighlight.First());
                return;
            // There are multiple Maps to Highlight. Highlight the one with the highest Tier and the most uncompleted Connections.
            case > 1:
                HighlightMap(
                    mapsToHighlight
                    .OrderBy(x => x.Tier)
                    .ThenByDescending(x => x.NumberOfUncompletedConnections).First());
                return;
        }
        
        mapsToHighlight = _mapsCache.Value
            .Where(x => x is { IsNormal: true, IsBonusCompleted: false })
            .OrderBy(x => x.Tier)
            .ToList();

        switch (mapsToHighlight.Count)
        {
            case 0:
                break;
            case 1:
                HighlightMap(mapsToHighlight.First());
                return;
            case > 1:
                HighlightMap(
                    mapsToHighlight
                    .OrderBy(x => x.Tier)
                    .ThenByDescending(x => x.NumberOfUncompletedConnections).First());
                return;
        }
        
        mapsToHighlight = _mapsCache.Value
            .Where(x => x is { IsNormal: true })
            .OrderBy(x => x.Tier)
            .ToList();
        
        switch (mapsToHighlight.Count)
        {
            case 0:
                break;
            case 1:
                HighlightMap(mapsToHighlight.First());
                return;
            case > 1:
                HighlightMap(
                    mapsToHighlight
                    .OrderBy(x => x.Tier)
                    .ThenByDescending(x => x.NumberOfUncompletedConnections).First());
                return;
        }
    }

    public override void EntityAdded(Entity entity)
    {
    }

    private void UpdateHighestCompletedTier()
    {
        _highestCompletedTier = Atlas.BonusCompletedAreas
            .Max(x => Atlas.AtlasNodes.FirstOrDefault(y => y.Area.Name == x.Name && y.Connections.Count > 1)?.TierProgression.FirstOrDefault()) ?? 0;
    }

    private void HighlightMap(Map map)
    {
        // Map is already Completed with Bonus. We do not care about its Tier nor ItemRarity.
        if (map.IsBonusCompleted)
        {
            Graphics.DrawFrame(map.ClientRect, Settings.MapReady.Value, Settings.StrokeWidth.Value);
            return;
        }
        
        // The Map is not Completed, so we care about its Tier and ItemRarity.
        switch (map.Tier)
        {
            case <= 5: // White Maps
                switch (map.Rarity)
                {
                    case ItemRarity.Normal:
                        Graphics.DrawFrame(map.ClientRect, Settings.MapNeedsCurrency.Value, Settings.StrokeWidth.Value);
                        Graphics.DrawImage("Transmutation.png", map.IconRect, new RectangleF(1, 1, 1, 1));
                        break;
                    case ItemRarity.Magic:
                        Graphics.DrawFrame(map.ClientRect, Settings.MapReady.Value, Settings.StrokeWidth.Value);
                        break;
                    case ItemRarity.Rare:
                        Graphics.DrawFrame(map.ClientRect, Settings.MapReady.Value, Settings.StrokeWidth.Value);
                        break;
                }
                break;
            case <= 10: // Yellow Maps
                switch (map.Rarity)
                {
                    case ItemRarity.Normal:
                        Graphics.DrawFrame(map.ClientRect, Settings.MapNeedsCurrency.Value, Settings.StrokeWidth.Value);
                        Graphics.DrawImage("Alchemy.png", map.IconRect, new RectangleF(1, 1, 1, 1));
                        break;
                    case ItemRarity.Magic:
                        Graphics.DrawFrame(map.ClientRect, Settings.MapNeedsCurrency.Value, Settings.StrokeWidth.Value);
                        Graphics.DrawImage("Regal.png", map.IconRect, new RectangleF(1, 1, 1, 1));
                        break;
                    case ItemRarity.Rare:
                        Graphics.DrawFrame(map.ClientRect, Settings.MapReady.Value, Settings.StrokeWidth.Value);
                        break;
                }
                break;
            case <= 16: // Red Maps
                switch (map.Rarity)
                {
                    case ItemRarity.Normal:
                        Graphics.DrawFrame(map.ClientRect, Settings.MapNeedsCurrency.Value, Settings.StrokeWidth.Value);
                        Graphics.DrawImage("Alchemy.png", map.IconRect, new RectangleF(1, 1, 1, 1));
                        break;
                    case ItemRarity.Magic:
                        Graphics.DrawFrame(map.ClientRect, Settings.MapNeedsCurrency.Value, Settings.StrokeWidth.Value);
                        Graphics.DrawImage("Regal.png", map.IconRect, new RectangleF(1, 1, 1, 1));
                        break;
                    case ItemRarity.Rare:
                        if (!map.IsCorrupted)
                        {
                            Graphics.DrawFrame(map.ClientRect, Settings.MapNeedsCurrency.Value, Settings.StrokeWidth.Value);
                            Graphics.DrawImage("Vaal.png", map.IconRect, new RectangleF(1, 1, 1, 1));
                        }
                        else
                        {
                            Graphics.DrawFrame(map.ClientRect, Settings.MapReady.Value, Settings.StrokeWidth.Value);
                        }
                        break;
                }
                break;
        }
    }
}