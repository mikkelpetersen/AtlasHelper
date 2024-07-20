using System.Collections.Generic;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;

namespace AtlasHelper;

public class Atlas
{
    private static readonly AtlasHelper Instance = AtlasHelper.Instance;
    
    public static List<AtlasNode> AtlasNodes => GameController?.Game?.Files?.AtlasNodes?.EntriesList ?? [];
    
    public static IList<WorldArea> CompletedAreas => ServerData?.CompletedAreas ?? [];

    public static IList<WorldArea> BonusCompletedAreas => ServerData?.BonusCompletedAreas ?? [];
    
    private static GameController GameController => Instance.GameController;
    
    private static ServerData ServerData => GameController?.IngameState?.Data?.ServerData;
}