
using Unity.Entities;
using UnityEngine;

public class PlayerScoreAuthoring : MonoBehaviour
{
    public float score;
}

public class PlayerScoreBaker : Baker<PlayerScoreAuthoring>
{
    public override void Bake(PlayerScoreAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlayerScore
        {
            score = authoring.score
        });
    }
}