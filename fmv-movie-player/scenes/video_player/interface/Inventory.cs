using Godot;
using System;

public partial class Inventory : Control
{
    [Export]
    int V_MaxInventorySize = 12;
    int V_TotalItemCount = 0;
    [Export]
    public string[] V_Inventory { get; set; }
    GridContainer N_InventorySlots;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Manager.Instance.N_Inventory = this;

        V_Inventory = new string[V_MaxInventorySize];
        N_InventorySlots = GetNode<GridContainer>("SubViewportContainer/SubViewport/NODES/InventorySlots");
    }

    public void F_AddItem(string V_ItemName, string V_IconPath = "icon.svg")
    {
        if (V_TotalItemCount + 1 <= V_MaxInventorySize)
        {
            int V_SpaceIndex = F_FindFreeAreaIndex();

            Button N_SlotButton;
            N_SlotButton = N_InventorySlots.GetChild(V_SpaceIndex).GetChild(0) as Button;
            N_SlotButton.Icon = GD.Load<Texture2D>(V_IconPath);

            V_Inventory[V_SpaceIndex] = V_ItemName;

            V_TotalItemCount += 1;
        }
    }

    public void F_RemoveItem(int V_ItemIndex)
    {
        V_Inventory[V_ItemIndex] = null;

        Button N_SlotButton;
        N_SlotButton = N_InventorySlots.GetChild(V_ItemIndex).GetChild(0) as Button;
        N_SlotButton.Icon = null;

        V_TotalItemCount -= 1;
    }

    public int F_GetItem(string V_ItemName)
    {
        int V_ItemIndex = -1;
        int V_CurrentIndex = -1;

        foreach (var V_Item in V_Inventory)
        {
            V_CurrentIndex += 1;

            if (V_Item == V_ItemName)
            {
                V_ItemIndex = V_CurrentIndex;
            }
        }

        return V_ItemIndex;
    }

    private int F_FindFreeAreaIndex()
    {
        int V_Index = -1;
        int V_CurrentIndex = -1;

        foreach (var V_ItemSpace in V_Inventory)
        {
            V_CurrentIndex += 1;

            if (V_ItemSpace == null)
            {
                GD.Print("FIND!");
                V_Index = V_CurrentIndex;
                break;
            }
        }
        GD.Print(V_Index);
        return V_Index;
    }
}
