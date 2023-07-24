using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;
using Enum;
using UnityEngine;


namespace System
{
    [UpdateAfter(typeof(InitiatePlayerSystem))]
    public partial struct PlaySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Square>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var square_map = SystemAPI.GetSingleton<GameController>().game_controller;
            foreach (var (player_position, playerInfo) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerInfo>>())
            {

                foreach(var (square_position, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                    if(square_position.ValueRW.Position.x == player_position.ValueRO.Position.x && square_position.ValueRW.Position.z == player_position.ValueRO.Position.z){
                        square.ValueRW.State = playerInfo.ValueRO.type;
                        square_map[square.ValueRW.index] = square.ValueRW.State;
                    }
                }
            }
        }
    }

}
