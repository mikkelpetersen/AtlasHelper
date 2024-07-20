using System.Collections.Generic;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using static ExileCore.PoEMemory.MemoryObjects.ServerInventory;
using MapComponent = ExileCore.PoEMemory.Components.Map;

namespace AtlasHelper;

public class Inventory
{
    private static readonly AtlasHelper Instance = AtlasHelper.Instance;

    public static bool IsVisible => InventoryElement is { IsVisible: true };

    public static List<Map> GetMapsFromInventory => InventoryItems.Where(x => x.Item is { Address: not 0 }).Where(x => x.Item.HasComponent<MapComponent>()).Select(x => new Map
        {
            ClientRect = x.GetClientRect(),
            Tier = Atlas.AtlasNodes.FirstOrDefault(y => y.Area.Name == x.Item.GetComponent<MapComponent>()?.Area.Name)?.TierProgression.First() ?? 0,
            Rarity = x.Item.GetComponent<Mods>()?.ItemRarity ?? ItemRarity.Normal,
            IsCorrupted = x.Item.GetComponent<Base>()?.isCorrupted ?? false,
            IsNormal = Atlas.AtlasNodes.FirstOrDefault(y => y.Area.Name == x.Item.GetComponent<MapComponent>()?.Area.Name)?.Connections.Count > 1,
            IsCompleted = Atlas.CompletedAreas.Any(y => y.Name == x.Item.GetComponent<MapComponent>()?.Area.Name),
            IsBonusCompleted = Atlas.BonusCompletedAreas.Any(y => y.Name == x.Item.GetComponent<MapComponent>()?.Area.Name),
            NumberOfUncompletedConnections = Atlas.AtlasNodes.FirstOrDefault(y => y.Area.Name == x.Item.GetComponent<MapComponent>()?.Area.Name)?.Connections.Count(z => Atlas.CompletedAreas.All(a => a.Name != z.Area.Name)) ?? 0
        }).ToList();

    private static IList<InventSlotItem> InventoryItems => ServerInventory?.InventorySlotItems;

    private static GameController GameController => Instance.GameController;

    private static ServerInventory ServerInventory => GameController?.IngameState?.Data?.ServerData?
        .PlayerInventories[(int)InventorySlotE.MainInventory1]?.Inventory;

    private static InventoryElement InventoryElement =>
        GameController.Game.IngameState.IngameUi.InventoryPanel;
}