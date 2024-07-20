using ExileCore.Shared.Enums;
using SharpDX;

namespace AtlasHelper;

public class Map
{
    public RectangleF ClientRect { get; init; }
    
    public RectangleF IconRect => new(ClientRect.X + 3, ClientRect.Y + 3, 36, 36);

    public int Tier { get; init; }
    
    public ItemRarity Rarity { get; init; }
    
    public bool IsCorrupted { get; init; }

    public bool IsNormal { get; init; }

    public bool IsCompleted { get; set; }

    public bool IsBonusCompleted { get; init; }

    public int NumberOfUncompletedConnections { get; init; }
}