using Component;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Enum;
using Random=UnityEngine.Random;
namespace System
{

    // [UpdateAfter(typeof(RestartSystem))]
    public partial struct CreateMapSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Map>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            //state.RequireForUpdate<RestartCommand>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Debug.Log("CreateMapSystem");
            var Map = SystemAPI.GetSingleton<Map>();
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var controller = state.EntityManager.CreateEntity();

            var square_map = new NativeArray<Value.SquareMaterial>(Map.width * Map.height, Allocator.Persistent);  
            for (var i = 0; i < Map.width*Map.height; i++){
                square_map[i] = Value.SquareMaterial.Ground;
                Debug.Log(square_map[i]);
            }

            state.EntityManager.AddComponent<GameController>(controller);
            state.EntityManager.SetComponentData(controller, new GameController
            {
                game_controller = square_map,
            });

            var squarejob = new CreateGroundJob
            {
                ecb = ecb.AsParallelWriter(),
                h = Map.height,
                w = Map.width,
                groundPrefab = Map.groundPrefab,
                wallPrefab = Map.wallPrefab
            };

            var walljob = new CreateWallJob
            {
                square_map = square_map,
                ecb = ecb,
                h = Map.height,
                w = Map.width,
                wallPrefab = Map.wallPrefab,
                quantity = Map.quantity,
            };

            state.Dependency = squarejob.ScheduleParallel(square_map.Length,64,state.Dependency);
            state.Dependency = walljob.Schedule(state.Dependency);
            Debug.Log("end createmap");
            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state)
        {
        }
    }
    
    public struct CreateGroundJob : IJobFor
    {
        public int h;
        public int w;
        public Entity groundPrefab;
        public Entity wallPrefab;
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute(int index)
        {
            int i = index / w;
            int j = index % w;
                var newCell = ecb.Instantiate(index,groundPrefab);
                ecb.SetComponent(index,newCell, new LocalTransform
                {
                    Position = new float3(j, 0 , i),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                ecb.AddComponent(index,newCell, new Square
                {
                    index = i * w + j,
                    State = Value.SquareMaterial.Ground
                });
        }
    }

    public struct IntTuple
    {
        public int x;
        public int y;
    }
    public struct CreateWallJob : IJob
    {
        public int w;
        public int h;
        public int quantity;
        public Entity wallPrefab;
        public NativeArray<Value.SquareMaterial> square_map;
        public EntityCommandBuffer ecb;

        public void Execute()
        {
            var randomData = Unity.Mathematics.Random.CreateFromIndex((uint)System.DateTime.Now.TimeOfDay.TotalSeconds);
            NativeArray<IntTuple> check = new NativeArray<IntTuple>(quantity/2, Allocator.Temp);
            for (var i = 0; i < quantity / 2; i++)
            {
                var x = 0;
                var y = 0;
                var find = false;
                while ((x == 0 && y == 0) || find == false)
                {
                    x = randomData.NextInt(0, w / 2);
                    y = randomData.NextInt(0, h);
                    find = true;
                    if(i!=0){
                        for(var j=0;j<=i;j++){
                            if(check[j].x == x && check[j].y == y){
                                find = false;
                                break;
                            }
                        }
                    }
                }
                check[i] = new IntTuple { x = x, y = y };
                var wall1 = ecb.Instantiate(wallPrefab);
                var wall2 = ecb.Instantiate(wallPrefab);
                ecb.SetComponent(wall1, new LocalTransform
                {
                    Position = new float3(x, 1, y),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });

                ecb.SetComponent(wall2, new LocalTransform
                {
                    Position = new float3(w - 1 - x, 1, h - 1 - y),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                var index1 = y*w + x;
                var index2 = w*h - index1 - 1;
                square_map[index1] = Value.SquareMaterial.Wall;
                square_map[index2] = Value.SquareMaterial.Wall;
            }
        }
    }
}
