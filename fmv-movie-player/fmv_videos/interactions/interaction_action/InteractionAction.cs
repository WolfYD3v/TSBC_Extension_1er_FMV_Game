using Godot;
using System;

public partial class InteractionAction : Control
{
    public enum E_MODES
    {
        PLAY_VIDEO,
        ADD_ITEM,
        NEXT_VIDEO
    }
    public enum E_CONDITIONS
    {
        LOOK_FOR_ITEM
    }

    VideoStreamPlayer N_VideoStreamPlayer;

    [ExportCategory("Mode settings")]
    [Export]
    public E_MODES V_Mode = E_MODES.PLAY_VIDEO;
    [ExportGroup("ADD_ITEM")]
    [Export]
    String V_ItemAddedName = "";
    [Export]
    String V_ItemAddedTexturePath = "icon.svg";
    [ExportGroup("PLAY_VIDEO")]
    [Export]
    VideoStream V_VideoStream = null;
    [ExportGroup("NEXT_VIDEO")]
    [Export]
    PackedScene V_OverrideNextScene = null;

    [ExportCategory("Condition settings")]
    [Export]
    public bool V_Conditional = false;
    [Export]
    InteractionAction V_FalseAction = null;
    [Export]
    E_CONDITIONS V_Condition = E_CONDITIONS.LOOK_FOR_ITEM;
    [ExportGroup("LOOK_FOR_ITEM")]
    [Export]
    String V_ItemNameWanted = "";

    public override void _Ready()
    {
        N_VideoStreamPlayer = GetNode<VideoStreamPlayer>("VideoStreamPlayer");
        N_VideoStreamPlayer.Hide();
        N_VideoStreamPlayer.Stream = V_VideoStream;
    }

    public void F_PlayInteraction()
    {
        if (V_Conditional == true)
        {
            GD.Print("Conditional!");
            if (V_Condition == E_CONDITIONS.LOOK_FOR_ITEM)
            {
                GD.Print(Manager.Instance.F_InventoryGetItem(V_ItemNameWanted));
                if (Manager.Instance.F_InventoryGetItem(V_ItemNameWanted) != V_ItemNameWanted)
                {
                    if (V_FalseAction != null)
                    {
                        V_FalseAction.F_PlayInteraction();
                        return;
                    }
                }
                else
                {
                    Manager.Instance.F_InventoryRemoveItem(V_ItemNameWanted);
                }
            }
        }

        GD.Print(V_Mode);

        switch (V_Mode)
        {
            case E_MODES.PLAY_VIDEO:
                N_VideoStreamPlayer.Show();
                N_VideoStreamPlayer.Play();
                break;

            case E_MODES.ADD_ITEM:
                Manager.Instance.F_InventoryAddItem(V_ItemAddedName, V_ItemAddedTexturePath);
                break;

            case E_MODES.NEXT_VIDEO:
                InteractionArea N_InteractionArea = (InteractionArea)GetParent().GetParent();
                BaseFMVVideo N_ParentFMVVideo = (BaseFMVVideo)N_InteractionArea.N_ParentFMVVideo;
                GD.Print("pp: " + N_ParentFMVVideo.ToString());
                N_ParentFMVVideo.F_NextVideoScene(V_OverrideNextScene);
                break;
        }
    }

    public void F_VideoFinished()
    {
        N_VideoStreamPlayer.Hide();
        N_VideoStreamPlayer.Stop();
    }
}
