using Unity.Entities;
using Enum;

namespace Component
{
    public struct PlayerInfo : IComponentData
    {
        public Value.SquareMaterial type;
        public bool turn;
    }
}