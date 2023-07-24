
using CortexDeveloper.ECSMessages.Service;
using Component;
using Unity.Entities;
using Unity.Transforms;
using Enum;
using UnityEngine;
namespace System
{
[UpdateAfter(typeof(InitiatePlayerSystem))]
public partial struct UpdateScore : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Map>();
        state.RequireForUpdate<Square>();
        state.RequireForUpdate<AIScore>();
        state.RequireForUpdate<PlayerScore>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        ////Debug.Log("checkk");
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var Map = SystemAPI.GetSingleton<Map>();
        ////Debug.Log("checkk1");
        var square_array = SystemAPI.GetSingleton<GameController>().game_controller;
        var width = Map.width;
        var height = Map.height;
        ////Debug.Log("checkk2");
        foreach(var(localTransform, player,entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess())
        {
            if(SystemAPI.HasComponent<MiniMax>(entity)){
                ////Debug.Log("checkk3");
                Value.SquareMaterial ai_type = player.ValueRW.type;
                ////Debug.Log(ai_type);
                var ai_score = 0f;
                for(int i=0; i<width*height;i++){
                    if(square_array[i] == ai_type){
                        ai_score += 1f;
                    }
                }
                ////Debug.Log(ai_score);
                var game = SystemAPI.GetSingletonEntity<Map>();
                ecb.SetComponent(game, new AIScore
                    {
                        score = ai_score,
                    });
                ////Debug.Log("checkkk4");
                var player_score = 0f;
                if(ai_type == Value.SquareMaterial.Red){
                    ////Debug.Log("redd");
                    Value.SquareMaterial player_type = Value.SquareMaterial.Green;
                    for(int i=0; i<width*height;i++){
                        if(square_array[i] == player_type){
                            ////Debug.Log(i);
                            player_score += 1f;
                        }
                    }
                }
                else{
                    Value.SquareMaterial player_type = Value.SquareMaterial.Red;
                    for(int i=0; i<width*height;i++){
                        if(square_array[i] == player_type){
                            player_score += 1f;
                        }
                    }
                }
                ecb.SetComponent(game, new PlayerScore
                    {
                        score = player_score,
                    });
            }
        }

    }

    public void OnDestroy(ref SystemState state)
    {
    }


}
}