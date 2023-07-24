using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using Enum;
using UnityEngine;
using System;
public partial struct RestartSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RestartCommand>();
        state.RequireForUpdate<Map>();
    }

    public void OnUpdate(ref SystemState state)
    {

        var dem = 0;
        Debug.Log("check restart");
        dem = 0;
        foreach(var(restart,entity) in SystemAPI.Query<RefRW<RestartCommand>>().WithEntityAccess()){
            dem++;
        }
        Debug.Log(dem);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var game = SystemAPI.GetSingletonEntity<Map>();
        ecb.SetComponent(game, new AIScore
            {
                score = 0
            });  
        ecb.SetComponent(game, new PlayerScore
            {
                score = 0,
            }); 
        ecb.SetComponent(game, new EndGame
            {
                state = Value.GameResult.Playing,
            });
        dem = 0;
        Debug.Log("check game controller");
        foreach(var(controller,entity) in SystemAPI.Query<RefRW<GameController>>().WithEntityAccess()){
            dem++;
        }
        Debug.Log(dem);
        foreach(var(controller,entity) in SystemAPI.Query<RefRW<GameController>>().WithEntityAccess()){
            ecb.DestroyEntity(entity);
        }
        dem = 0;
        foreach(var(controller,entity) in SystemAPI.Query<RefRW<GameController>>().WithEntityAccess()){
            dem++;
        }
        Debug.Log(dem);
        Debug.Log("check game controller");

        foreach(var(local,entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithEntityAccess()){
            ecb.DestroyEntity(entity);
        }
        // foreach(var(square,entity) in SystemAPI.Query<RefRW<Square>>().WithEntityAccess()){
        //     ecb.DestroyEntity(entity);
        // }

        foreach(var(restart,entity) in SystemAPI.Query<RefRW<RestartCommand>>().WithEntityAccess()){
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        state.WorldUnmanaged.GetExistingSystemState<CreateMapSystem>().Enabled = true;
        state.WorldUnmanaged.GetExistingSystemState<InitiatePlayerSystem>().Enabled = true;
        state.WorldUnmanaged.GetExistingSystemState<MiniMaxSystem>().Enabled = true;

    }

    public void OnDestroy(ref SystemState state)
    {
    }
}
