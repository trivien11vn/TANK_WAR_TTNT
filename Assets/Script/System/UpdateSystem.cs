using Component;
using Unity.Entities;
using Unity.Rendering;
using Enum;
using UnityEngine;
namespace System
{
    [UpdateAfter(typeof(CreateMapSystem))]
    public partial struct UpdateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameController>();
            state.RequireForUpdate<GameMaterial>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var controller = SystemAPI.GetSingleton<GameController>();
            var stateArray = controller.game_controller;
            foreach (var square in SystemAPI.Query<RefRW<Square>>())
            {
                square.ValueRW.State = stateArray[square.ValueRO.index];
            }

            var material = SystemAPI.GetSingleton<GameMaterial>();
            foreach (var (square_state, square_material) in SystemAPI.Query<RefRO<Square>,RefRW<MaterialMeshInfo>>()){
                if (square_state.ValueRO.State == Value.SquareMaterial.Ground)
                {
                    square_material.ValueRW.MaterialID = material.ground;
                }
                else if (square_state.ValueRO.State == Value.SquareMaterial.Green)
                {
                    square_material.ValueRW.MaterialID = material.green;
                }
                else if (square_state.ValueRO.State == Value.SquareMaterial.Red)
                {
                    square_material.ValueRW.MaterialID = material.red;
                }
                else if (square_state.ValueRO.State == Value.SquareMaterial.Wall)
                {
                    square_material.ValueRW.MaterialID = material.wall;
                }
                else
                {
                    square_material.ValueRW.MaterialID = material.ground;
                }
            }
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}