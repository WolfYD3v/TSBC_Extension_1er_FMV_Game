using Godot;
using System;

public partial class VideoPlayer : Control
{
    Interface N_Interface;
    Control N_VideoPlayerControl;
    Control N_FMVVideoLocation;

    public Vector2 V_MinVideoPlayerSize { get; set; }
    public Vector2 V_MaxVideoPlayerSize { get; set; }

    SubViewport N_VideoPlayerSubViewport;

    AnimationPlayer N_AnimationPlayer;

    Tween V_VideoPlayerSizeTween;

    public async override void _Ready()
    {
        Manager.Instance.N_VideoPlayer = this;

        N_VideoPlayerSubViewport = GetNode<SubViewport>("VideoPlayer/SubViewportContainer/SubViewport");

        N_Interface = GetNode<Interface>("Interface");

        V_MinVideoPlayerSize = F_BuildVector2(780.0f, 533.0f);
        V_MaxVideoPlayerSize = F_BuildVector2(1152.0f, 648.0f);

        N_VideoPlayerControl = GetNode<Control>("VideoPlayer");
        N_VideoPlayerControl.Size = V_MaxVideoPlayerSize;

        N_FMVVideoLocation = GetNode<Control>("VideoPlayer/SubViewportContainer/SubViewport/FMVVideoLocation");

        N_AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        N_AnimationPlayer.Play("BlackFading");
        await ToSignal(N_AnimationPlayer, "animation_finished");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (N_VideoPlayerSubViewport is not null)
        {
            N_VideoPlayerSubViewport.PushInput(@event);
        }
    }

    public void F_ChangeVideoPlayerSize(Vector2 V_NewVideoPlayerSize)
    {
        if (V_VideoPlayerSizeTween is not null)
        {
            V_VideoPlayerSizeTween.Kill();
        }
        string V_MoveTweenProperty = "size";
        float V_MoveTweenTime = 5.0f;
        V_VideoPlayerSizeTween = GetTree().CreateTween();
        V_VideoPlayerSizeTween.TweenProperty(N_VideoPlayerControl, V_MoveTweenProperty, V_NewVideoPlayerSize, V_MoveTweenTime);
    }

    public Vector2 F_BuildVector2(float V_XValue, float V_YValue)
    {
        Vector2 V_NewVector = new Vector2();
        V_NewVector.X = V_XValue;
        V_NewVector.Y = V_YValue;

        return V_NewVector;
    }

    public void F_SetFMVVideo(PackedScene V_NextVideoScene)
    {
        if (N_FMVVideoLocation != null)
        {
            if (N_FMVVideoLocation.GetChildCount() > 0)
            {
                N_FMVVideoLocation.GetChildren()[0].QueueFree();
            }

            BaseFMVVideo V_NewFMVVideoScene = V_NextVideoScene.Instantiate() as BaseFMVVideo;
            N_FMVVideoLocation.AddChild(V_NewFMVVideoScene);
            V_NewFMVVideoScene.S_NextVideoScene += F_SetFMVVideo;
            GD.Print($"NEXT SCENE: {V_NewFMVVideoScene.Get("V_NextScenePath")}");
        }
    }
}
