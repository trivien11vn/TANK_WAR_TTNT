using Unity.Entities;

namespace Component
{
    public struct Map : IComponentData
    {
        public int height;
        public int width;
        public int quantity;
        public Entity groundPrefab;
        public Entity wallPrefab;
    }
}