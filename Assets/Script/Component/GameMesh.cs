using Unity.Entities;
using UnityEngine.Rendering;

namespace Component
{
    public struct GameMesh : IComponentData
    {
        public BatchMeshID player_mesh;
    }
}