using Godot;
using System;

public partial class InteractionArea : Control
{
    public BaseFMVVideo N_ParentFMVVideo;

    Button N_Button;

    InteractionAction N_InteractionAction;
    Control N_Action;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        N_ParentFMVVideo = (BaseFMVVideo)GetParent();

        N_Button = GetNode<Button>("Button");

        N_Action = GetNode<Control>("Action");
        if (N_Action.GetChildCount() > 0)
        {
            N_InteractionAction = GetNode<InteractionAction>("Action/" + N_Action.GetChild(0).Name);

            if (N_InteractionAction.V_Mode == InteractionAction.E_MODES.NEXT_VIDEO && N_InteractionAction.V_Conditional == false)
            {
                N_Button.Pressed += () => N_ParentFMVVideo.F_NextVideoScene();
                return;
            }
        }
        N_Button.Pressed += F_ButtonPressed;
    }

    public void F_ButtonPressed()
    {
        if (N_InteractionAction != null)
        {
            N_InteractionAction.F_PlayInteraction();
        }
    }
}
