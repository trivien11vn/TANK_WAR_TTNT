using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Component
{
    public class GameMaterialAuthoring : MonoBehaviour
    {
        public Material ground;
        public Material red;
        public Material green;
        public Material wall;

        public class GameMaterialBaker : Baker<GameMaterialAuthoring>
        {
            public override void Bake(GameMaterialAuthoring authoring)
            {
                var hybridRenderer = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameMaterial
                {
                    red= hybridRenderer.RegisterMaterial(authoring.red),
                    green = hybridRenderer.RegisterMaterial(authoring.green),
                    ground = hybridRenderer.RegisterMaterial(authoring.ground),
                    wall = hybridRenderer.RegisterMaterial(authoring.wall)
                });
            }
        }
    }
}