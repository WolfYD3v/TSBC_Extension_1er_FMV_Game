using Godot;
using System;

public partial class Manager : Node
{
    public static Manager Instance { get; private set; }

    public VideoPlayer N_VideoPlayer { get; set; }
    public Inventory N_Inventory { get; set; }
    public Interface N_Interface { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;
    }

    // VIDEO PLAYER
    public void F_SetVideoPlayerSizeSmall()
    {
        N_VideoPlayer.F_ChangeVideoPlayerSize(N_VideoPlayer.V_MinVideoPlayerSize);
    }
    public void F_SetVideoPlayerSizeBig()
    {
        N_VideoPlayer.F_ChangeVideoPlayerSize(N_VideoPlayer.V_MaxVideoPlayerSize);
    }

    // INVENTORY
    public void F_InventoryAddItem(string V_ItemName, string V_IconPath = "icon.svg")
    {
        N_Inventory.F_AddItem(V_ItemName, V_IconPath);
    }
    public void F_InventoryRemoveItem(string V_ItemName)
    {
        N_Inventory.F_RemoveItem(N_Inventory.F_GetItem(V_ItemName));
    }
    public string F_InventoryGetItem(string V_ItemName)
    {
        int V_InventoryItemIndex = N_Inventory.F_GetItem(V_ItemName);
        GD.Print(V_InventoryItemIndex);
        GD.Print(N_Inventory.V_Inventory);

        if (V_InventoryItemIndex < 0)
        {
            return "NONE";
        }
        string V_Item = N_Inventory.V_Inventory[V_InventoryItemIndex];
        GD.Print(V_Item);
        return V_Item;
    }

    // HEALTH BAR
    public void F_ReduceHealth(double V_Damages)
    {
        N_Interface.F_ReduceHealth(V_Damages);
    }

    public void F_EndFMV()
    {
        GD.Print("GAME ENDED");
        GetTree().Quit();
    }
}
