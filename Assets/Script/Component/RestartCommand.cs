using CortexDeveloper.ECSMessages.Components;
using Unity.Entities;

public struct RestartCommand : IComponentData, IMessageComponent
{

    public bool IsStart { get; set; }
}