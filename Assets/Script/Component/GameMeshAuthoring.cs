using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Component
{
    public class GameMeshAuthoring : MonoBehaviour
    {
        public Mesh player_mesh;

        public class GameMeshBaker : Baker<GameMeshAuthoring>
        {
            public override void Bake(GameMeshAuthoring authoring)
            {
                var hybridRenderer = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameMesh
                {
                    player_mesh = hybridRenderer.RegisterMesh(authoring.player_mesh)
                });
            }
        }
    }
}