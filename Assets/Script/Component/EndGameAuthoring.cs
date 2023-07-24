
using Unity.Entities;
using UnityEngine;
using Enum;
public class EndGameAuthoring : MonoBehaviour
{
    public Value.GameResult state = Value.GameResult.Playing;
}

public class EndGameBaker : Baker<EndGameAuthoring>
{
    public override void Bake(EndGameAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EndGame
        {
            state = authoring.state
        });
    }
}