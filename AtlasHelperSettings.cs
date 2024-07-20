using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace AtlasHelper;

public class AtlasHelperSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new(false);
    
    public RangeNode<int> StrokeWidth { get; set; } = new(3, 1, 10);

    public ColorNode MapReady { get; set; } = new(Color.LightGreen);
    public ColorNode MapNeedsCurrency { get; set; } = new(Color.Yellow);
    
    public RangeNode<int> TierDifference { get; set; } = new(2, 1, 10);
    
    public ListNode LogLevel { get; set; } = new()
    {
        Values = ["None", "Debug", "Error"],
        Value = "Error"
    };
}