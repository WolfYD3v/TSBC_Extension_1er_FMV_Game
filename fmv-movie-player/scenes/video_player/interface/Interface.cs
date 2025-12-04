using Godot;
using System;

public partial class Interface : Control
{
    public ProgressBar N_HealthBar { get; set; }
    double V_MaxHealth = 100.0f;

    Inventory N_Inventory;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Manager.Instance.N_Interface = this;
        N_Inventory = GetNode<Inventory>("Sections/RightSection/Inventory");

        N_HealthBar = GetNode<ProgressBar>("Sections/RightSection/Status/SubViewportContainer/SubViewport/NODES/HealthBar");
        N_HealthBar.MaxValue = V_MaxHealth;
        N_HealthBar.Value = V_MaxHealth;
    }

    public double F_GetHealth()
    {
        if (N_HealthBar is null)
        {
            return -10.0f;
        }
        return N_HealthBar.Value;
    }

    public void F_ReduceHealth(double V_Damages)
    {
        N_HealthBar.Value -= V_Damages;
        if (N_HealthBar.Value <= 0.0f)
        {
            Manager.Instance.F_EndFMV();
        }
    }
}
