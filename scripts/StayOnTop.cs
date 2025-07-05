using Godot;
using System;

public partial class StayOnTop : Node
{
    bool ReorderTimeout = true;

    public async override void _Ready()
    {
        base._Ready();
        GetParent().ChildOrderChanged += Reorder;
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Reorder();
    }

    private async void Reorder()
    {
        if (GetTree() != null && ReorderTimeout)
        {
            ReorderTimeout = false;
            GetParent().CallDeferred(MethodName.MoveChild, this, -1);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            ReorderTimeout = true;
        }
    }
}
