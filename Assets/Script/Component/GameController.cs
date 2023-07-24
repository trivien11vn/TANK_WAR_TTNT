using Unity.Collections;
using Unity.Entities;
using Enum;

namespace Component
{
    public struct GameController : IComponentData
    {
        public NativeArray<Value.SquareMaterial> game_controller;
    }
}