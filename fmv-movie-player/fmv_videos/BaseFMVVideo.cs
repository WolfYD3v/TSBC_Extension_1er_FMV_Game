using Godot;
using System;

public partial class BaseFMVVideo : Control
{
    [Signal]
    public delegate void S_NextVideoSceneEventHandler(PackedScene V_NextVideoScene);

    [ExportCategory("Next Video Scene Transition")]
    [Export]
    bool V_NextSceneAuto = true;
    [Export]
    PackedScene V_NextScene = null;
    [ExportCategory("Waiting Time on Skip Button")]
    [Export]
    bool V_WaitForEnableButton = false;
    [Export]
    double V_WaitTimeForEnableButton = 10.0f;
    [ExportCategory("Interface Control")]
    [Export]
    bool V_RevealFullInterface = false;

    VideoStreamPlayer N_VideoStreamPlayer;
    Button N_Button;

    AudioStreamPlayer N_ButonHoverdAudioStreamPlayer;
    AudioStreamPlayer N_ButonClickedAudioStreamPlayer;

    // Called when the node enters the scene tree for the first time.
    public async override void _Ready()
    {
        N_ButonHoverdAudioStreamPlayer = GetNode<AudioStreamPlayer>("ButtonHoveredAudioStreamPlayer");
        N_ButonClickedAudioStreamPlayer = GetNode<AudioStreamPlayer>("ButtonClickedAudioStreamPlayer");

        N_VideoStreamPlayer = GetNode<VideoStreamPlayer>("VideoStreamPlayer");
        N_Button = GetNode<Button>("Buttons/Button");
        N_Button.Show();
        N_Button.Disabled = true;

        if (V_RevealFullInterface == true)
        {
            Manager.Instance.F_SetVideoPlayerSizeSmall();
        }

        if (V_NextSceneAuto == true) { N_Button.Hide(); }
        else
        {
            if (V_WaitForEnableButton == true)
            {
                await ToSignal(GetTree().CreateTimer(V_WaitTimeForEnableButton), "timeout");
                N_Button.Disabled = false;
            }
        }
    }

    public void F_NextVideoScene(PackedScene V_OverrideVideoScene = null)
    {
        if (V_OverrideVideoScene != null)
        {
            V_NextScene = V_OverrideVideoScene;
        }

        N_ButonClickedAudioStreamPlayer.Play();
        if (V_NextScene != null)
        {
            EmitSignal(SignalName.S_NextVideoScene, V_NextScene);
        }
    }

    public void F_VideoSceneFinished()
    {
        N_VideoStreamPlayer.Paused = true;
        if (V_NextSceneAuto == true) { F_NextVideoScene(); }
        else { N_Button.Disabled = false; }
    }

    public void F_OnButtonMouseEntered()
    {
        if (N_Button.Disabled == false)
        {
            N_ButonHoverdAudioStreamPlayer.Play();
        }
    }
}
