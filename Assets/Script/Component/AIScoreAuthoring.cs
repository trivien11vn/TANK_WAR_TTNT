
using Unity.Entities;
using UnityEngine;

public class AIScoreAuthoring : MonoBehaviour
{
    public float score;
}

public class AIScoreBaker : Baker<AIScoreAuthoring>
{
    public override void Bake(AIScoreAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new AIScore
        {
            score = authoring.score
        });
    }
}