using Unity.Entities;
using UnityEngine;

namespace Component
{
    public class MapAuthoring : MonoBehaviour
    {
        public int width;
        public int height;
        public int quantity;
        public GameObject groundPrefab;
        public GameObject wallPrefab;
        public class MapBaker : Baker<MapAuthoring>
        {
            public override void Bake(MapAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Map
                {
                    height = authoring.height,
                    width = authoring.width,
                    quantity = authoring.quantity,
                    groundPrefab = GetEntity(authoring.groundPrefab, TransformUsageFlags.Dynamic),
                    wallPrefab = GetEntity(authoring.wallPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}