using Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Enum;

namespace System
{
    [UpdateAfter(typeof(CreateMapSystem))]
    public partial struct InitiatePlayerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameMesh>();
            state.RequireForUpdate<GameMaterial>();
            state.RequireForUpdate<Player>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Debug.Log("InitiatePlayerSystem");
            var player = SystemAPI.GetSingleton<Player>().Playerr;
            var material = SystemAPI.GetSingleton<GameMaterial>();
            var mesh = SystemAPI.GetSingleton<GameMesh>();
            var dem = 0;
            foreach(var(controller,entity) in SystemAPI.Query<RefRW<GameController>>().WithEntityAccess()){
                dem++;
            }
            Debug.Log(dem);

            var player1 = state.EntityManager.Instantiate(player);
            state.EntityManager.SetComponentData(player1, new LocalTransform
            {
                Position = new float3(0, 1, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            state.EntityManager.SetComponentData(player1, new MaterialMeshInfo
            {
                MaterialID = material.red,
                MeshID = mesh.player_mesh
            });
            state.EntityManager.AddComponentData(player1, new PlayerInfo
            {
                type = Value.SquareMaterial.Red,
                turn = true
            });
            state.EntityManager.AddComponentData(player1, new MiniMax
            {
                count = 0
            });

            var player2 = state.EntityManager.Instantiate(player);
            state.EntityManager.SetComponentData(player2, new LocalTransform
            {
                Position = new float3(9, 1, 9),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            state.EntityManager.SetComponentData(player2, new MaterialMeshInfo
            {
                MaterialID = material.green,
                MeshID = mesh.player_mesh
            });
            state.EntityManager.AddComponentData(player2, new PlayerInfo
            {
                type = Value.SquareMaterial.Green,
                turn = false
            });

            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state)
        {
        }
    }
}
