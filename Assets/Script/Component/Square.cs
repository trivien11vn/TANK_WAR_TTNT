using Unity.Entities;
using Enum;

namespace Component
{
    public struct Square: IComponentData
    {
        public int index;
        public Value.SquareMaterial State;
    }
}