using Unity.Entities;
using UnityEngine.Rendering;

namespace Component
{
    public struct GameMaterial : IComponentData
    {
        public BatchMaterialID ground;
        public BatchMaterialID red;
        public BatchMaterialID green;
        public BatchMaterialID wall;
    }
}