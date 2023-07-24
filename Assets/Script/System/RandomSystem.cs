// using Component;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Rendering;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Enum;
// using UnityEngine;

// namespace System
// {
//     [UpdateAfter(typeof(InitiatePlayerSystem))]
//     public partial struct GetPossibleSystem : ISystem
//     {
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<Map>();
//             state.RequireForUpdate<Square>();
//             state.RequireForUpdate<GameMesh>();
//             state.RequireForUpdate<GameMaterial>();
//             state.RequireForUpdate<Randomm>();
//             state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
//         }

//         public void OnUpdate(ref SystemState state)
//         {
//             Debug.Log("check_update");
//             var material = SystemAPI.GetSingleton<GameMaterial>();
//             var mesh = SystemAPI.GetSingleton<GameMesh>();
//             var playerPrefab = SystemAPI.GetSingleton<Player>().Playerr;
//             var Map = SystemAPI.GetSingleton<Map>();
//             var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
//             var cellArray = SystemAPI.GetSingleton<GameController>().game_controller;
//             float3 PlayerPosition;
//             Value.SquareMaterial PlayerType;
//             var Position = new NativeArray<int>(4, Allocator.Temp);
//             var width = Map.width;
//             var height = Map.height;
//             var baseTime = System.DateTime.Now.TimeOfDay.TotalSeconds;
//             var seed = (int)(float)(baseTime + SystemAPI.Time.ElapsedTime);
//             var randomData = Unity.Mathematics.Random.CreateFromIndex((uint) seed);
//             var i = 0;
//             foreach(var(localTransform, player,entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess())
//             {
//                 if (player.ValueRW.turn == true){
//                     Debug.Log(player.ValueRW.type);
//                     PlayerPosition = localTransform.ValueRW.Position;
//                     PlayerType = player.ValueRW.type;
                    
//                     //ecb.DestroyEntity(entity);
//                     if(SystemAPI.HasComponent<Randomm>(entity)){
//                         foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                             if(squareTransform.ValueRW.Position.x == PlayerPosition.x && squareTransform.ValueRW.Position.z == PlayerPosition.z){
//                                 var index = square.ValueRW.index;
//                                 if((index+1)%width != 0){
//                                     if(cellArray[index+1] == Value.SquareMaterial.Ground){
//                                         Position[i] = index + 1;
//                                         i++;
//                                     }
//                                 }
//                                 if(index + width < width * height - 1){
//                                     if(cellArray[index+width] == Value.SquareMaterial.Ground){
//                                         Position[i] = index + width;
//                                         i++;
//                                     }
//                                 }
//                                 if(index - width > 0){
//                                     if(cellArray[index-width] == Value.SquareMaterial.Ground){
//                                         Position[i] = index - width;
//                                         i++;
//                                     }
//                                 }
//                                 if(index%width != 0){
//                                     if(cellArray[index-1] == Value.SquareMaterial.Ground){
//                                         Position[i] = index - 1;
//                                         i++;
//                                     }
//                                 }
//                                 if(i==0){
//                                     Debug.Log("GAME_OVER");
//                                     state.Enabled = false;
//                                 }
//                             }
//                         }
//                         var randomIndex = randomData.NextInt(0, i);
//                         foreach(var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                             if (square.ValueRW.index == Position[randomIndex]){
//                                 foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
//                                     if(playerr.ValueRW.turn == false){
//                                         ecb.SetComponent(entityy, new PlayerInfo
//                                             {
//                                                 type = playerr.ValueRW.type,
//                                                 count = 0,
//                                                 turn = true
//                                             });
//                                         Debug.Log(playerr.ValueRW.type);
//                                     }
//                                 }
//                                 var player1 = ecb.Instantiate(playerPrefab);
//                                 ecb.SetComponent(player1, new LocalTransform
//                                 {
//                                     Position = new float3(squareTransform.ValueRW.Position.x, 1, squareTransform.ValueRW.Position.z),
//                                     // Position = squareTransform.ValueRW.Position,
//                                     Rotation = quaternion.identity,
//                                     Scale = 1f
//                                 });
//                                 if (player.ValueRW.type == Value.SquareMaterial.Red){
//                                     square.ValueRW.State = Value.SquareMaterial.Red;
//                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                     {
//                                         MaterialID = material.red,
//                                         MeshID = mesh.player_mesh
//                                     });
//                                     ecb.AddComponent(player1, new PlayerInfo
//                                     {
//                                         type = Value.SquareMaterial.Red,
//                                         count = 0,
//                                         turn = false
//                                     });
//                                     ecb.AddComponent(player1, new Randomm
//                                     {
//                                         count = 0
//                                     });
//                                     Debug.Log("a");
//                                 }
//                                 else if(player.ValueRW.type == Value.SquareMaterial.Green){
//                                     square.ValueRW.State = Value.SquareMaterial.Green;
//                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                     {
//                                         MaterialID = material.green,
//                                         MeshID = mesh.player_mesh
//                                     });
//                                     ecb.AddComponent(player1, new PlayerInfo
//                                     {
//                                         type = Value.SquareMaterial.Green,
//                                         count = 0,
//                                         turn = false
//                                     });
//                                     // ecb.AddComponent(player1, new Randomm
//                                     // {
//                                     //     count = 0
//                                     // });
//                                 }
//                             }
//                         }
                        
//                             ecb.DestroyEntity(entity);
//                             // var delayJob = new DelayJob { DelayTime = 1f };
//                             // delayJob.Schedule().Complete();
//                             // Debug.Log("check sleep");
//                     }
//                     else{
//                         Debug.Log(player.ValueRW.type);
//                         if (Input.GetKeyDown(KeyCode.S))
//                         {
//                             Debug.Log("W");
//                             foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                                 if(squareTransform.ValueRW.Position.x == PlayerPosition.x && squareTransform.ValueRW.Position.z == PlayerPosition.z){
//                                     var index = square.ValueRW.index;
//                                     if(index - width > 0 && cellArray[index-width] == Value.SquareMaterial.Ground){
//                                         foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
//                                             if(playerr.ValueRW.turn == false){
//                                                 ecb.SetComponent(entityy, new PlayerInfo
//                                                     {
//                                                         type = playerr.ValueRW.type,
//                                                         count = 0,
//                                                         turn = true
//                                                     });
//                                                 Debug.Log(playerr.ValueRW.type);
//                                             }
//                                         }
//                                         foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                                             if(new_square.ValueRW.index == index - width){
//                                                 var player1 = ecb.Instantiate(playerPrefab);
//                                                 ecb.SetComponent(player1, new LocalTransform
//                                                 {
//                                                     Position = new float3(new_squareTransform.ValueRW.Position.x, 1, new_squareTransform.ValueRW.Position.z),
//                                                     Rotation = quaternion.identity,
//                                                     Scale = 1f
//                                                 });
//                                                 if (player.ValueRW.type == Value.SquareMaterial.Red){
//                                                     square.ValueRW.State = Value.SquareMaterial.Red;
//                                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                                     {
//                                                         MaterialID = material.red,
//                                                         MeshID = mesh.player_mesh
//                                                     });
//                                                     ecb.AddComponent(player1, new PlayerInfo
//                                                     {
//                                                         type = Value.SquareMaterial.Red,
//                                                         count = 0,
//                                                         turn = false
//                                                     });
//                                                 }
//                                                 else if(player.ValueRW.type == Value.SquareMaterial.Green){
//                                                     square.ValueRW.State = Value.SquareMaterial.Green;
//                                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                                     {
//                                                         MaterialID = material.green,
//                                                         MeshID = mesh.player_mesh
//                                                     });
//                                                     ecb.AddComponent(player1, new PlayerInfo
//                                                     {
//                                                         type = Value.SquareMaterial.Green,
//                                                         count = 0,
//                                                         turn = false
//                                                     });
//                                                 }     
//                                                 ecb.DestroyEntity(entity);
//                                             }
//                                         }
                                        
//                                         // var delayJob = new DelayJob { DelayTime = 1f };
//                                         // delayJob.Schedule().Complete();
//                                         // Debug.Log("check sleep");                                 
//                                     }
//                                 }
//                             }
//                         }
//                         else if (Input.GetKeyDown(KeyCode.W))
//                         {
//                             Debug.Log("S");
//                             foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                                 if(squareTransform.ValueRW.Position.x == PlayerPosition.x && squareTransform.ValueRW.Position.z == PlayerPosition.z){
//                                     var index = square.ValueRW.index;
//                                     if(index + width < width*height-1 && cellArray[index+width] == Value.SquareMaterial.Ground){
//                                         foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
//                                             if(playerr.ValueRW.turn == false){
//                                                 ecb.SetComponent(entityy, new PlayerInfo
//                                                     {
//                                                         type = playerr.ValueRW.type,
//                                                         count = 0,
//                                                         turn = true
//                                                     });
//                                                 Debug.Log(playerr.ValueRW.type);
//                                             }
//                                         }
//                                         foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                                             if(new_square.ValueRW.index == index + width){
//                                                 var player1 = ecb.Instantiate(playerPrefab);
//                                                 ecb.SetComponent(player1, new LocalTransform
//                                                 {
//                                                     Position = new float3(new_squareTransform.ValueRW.Position.x, 1, new_squareTransform.ValueRW.Position.z),
//                                                     Rotation = quaternion.identity,
//                                                     Scale = 1f
//                                                 });
//                                                 if (player.ValueRW.type == Value.SquareMaterial.Red){
//                                                     square.ValueRW.State = Value.SquareMaterial.Red;
//                                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                                     {
//                                                         MaterialID = material.red,
//                                                         MeshID = mesh.player_mesh
//                                                     });
//                                                     ecb.AddComponent(player1, new PlayerInfo
//                                                     {
//                                                         type = Value.SquareMaterial.Red,
//                                                         count = 0,
//                                                         turn = false
//                                                     });
//                                                 }
//                                                 else if(player.ValueRW.type == Value.SquareMaterial.Green){
//                                                     square.ValueRW.State = Value.SquareMaterial.Green;
//                                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                                     {
//                                                         MaterialID = material.green,
//                                                         MeshID = mesh.player_mesh
//                                                     });
//                                                     ecb.AddComponent(player1, new PlayerInfo
//                                                     {
//                                                         type = Value.SquareMaterial.Green,
//                                                         count = 0,
//                                                         turn = false
//                                                     });
//                                                 }     
//                                                 ecb.DestroyEntity(entity);
//                                             }
//                                         }                                
//                                     }
//                                 }
//                             }
//                         }
//                         else if (Input.GetKeyDown(KeyCode.A))
//                         {
//                             Debug.Log("A");
//                             foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                                 if(squareTransform.ValueRW.Position.x == PlayerPosition.x && squareTransform.ValueRW.Position.z == PlayerPosition.z){
//                                     var index = square.ValueRW.index;
//                                     if(index % width != 0 && cellArray[index-1] == Value.SquareMaterial.Ground){
//                                         foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
//                                             if(playerr.ValueRW.turn == false){
//                                                 ecb.SetComponent(entityy, new PlayerInfo
//                                                     {
//                                                         type = playerr.ValueRW.type,
//                                                         count = 0,
//                                                         turn = true
//                                                     });
//                                                 Debug.Log(playerr.ValueRW.type);
//                                             }
//                                         }
//                                         foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                                             if(new_square.ValueRW.index == index - 1){
//                                                 var player1 = ecb.Instantiate(playerPrefab);
//                                                 ecb.SetComponent(player1, new LocalTransform
//                                                 {
//                                                     Position = new float3(new_squareTransform.ValueRW.Position.x, 1, new_squareTransform.ValueRW.Position.z),
//                                                     Rotation = quaternion.identity,
//                                                     Scale = 1f
//                                                 });
//                                                 if (player.ValueRW.type == Value.SquareMaterial.Red){
//                                                     square.ValueRW.State = Value.SquareMaterial.Red;
//                                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                                     {
//                                                         MaterialID = material.red,
//                                                         MeshID = mesh.player_mesh
//                                                     });
//                                                     ecb.AddComponent(player1, new PlayerInfo
//                                                     {
//                                                         type = Value.SquareMaterial.Red,
//                                                         count = 0,
//                                                         turn = false
//                                                     });
//                                                 }
//                                                 else if(player.ValueRW.type == Value.SquareMaterial.Green){
//                                                     square.ValueRW.State = Value.SquareMaterial.Green;
//                                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                                     {
//                                                         MaterialID = material.green,
//                                                         MeshID = mesh.player_mesh
//                                                     });
//                                                     ecb.AddComponent(player1, new PlayerInfo
//                                                     {
//                                                         type = Value.SquareMaterial.Green,
//                                                         count = 0,
//                                                         turn = false
//                                                     });
//                                                 }     
//                                                 ecb.DestroyEntity(entity);
//                                             }
//                                         }
                                        
//                                         // var delayJob = new DelayJob { DelayTime = 1f };
//                                         // delayJob.Schedule().Complete();
//                                         // Debug.Log("check sleep");                                 
//                                     }
//                                 }
//                             }
//                         }
//                         else if (Input.GetKeyDown(KeyCode.D))
//                         {
//                             Debug.Log("D");
//                             foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                                 if(squareTransform.ValueRW.Position.x == PlayerPosition.x && squareTransform.ValueRW.Position.z == PlayerPosition.z){
//                                     var index = square.ValueRW.index;
//                                     if((index+1) % width != 0 && cellArray[index+1] == Value.SquareMaterial.Ground){
//                                         foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
//                                             if(playerr.ValueRW.turn == false){
//                                                 ecb.SetComponent(entityy, new PlayerInfo
//                                                     {
//                                                         type = playerr.ValueRW.type,
//                                                         count = 0,
//                                                         turn = true
//                                                     });
//                                                 Debug.Log(playerr.ValueRW.type);
//                                             }
//                                         }
//                                         foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
//                                             if(new_square.ValueRW.index == index + 1){
//                                                 var player1 = ecb.Instantiate(playerPrefab);
//                                                 ecb.SetComponent(player1, new LocalTransform
//                                                 {
//                                                     Position = new float3(new_squareTransform.ValueRW.Position.x, 1, new_squareTransform.ValueRW.Position.z),
//                                                     Rotation = quaternion.identity,
//                                                     Scale = 1f
//                                                 });
//                                                 if (player.ValueRW.type == Value.SquareMaterial.Red){
//                                                     square.ValueRW.State = Value.SquareMaterial.Red;
//                                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                                     {
//                                                         MaterialID = material.red,
//                                                         MeshID = mesh.player_mesh
//                                                     });
//                                                     ecb.AddComponent(player1, new PlayerInfo
//                                                     {
//                                                         type = Value.SquareMaterial.Red,
//                                                         count = 0,
//                                                         turn = false
//                                                     });
//                                                 }
//                                                 else if(player.ValueRW.type == Value.SquareMaterial.Green){
//                                                     square.ValueRW.State = Value.SquareMaterial.Green;
//                                                     ecb.SetComponent(player1, new MaterialMeshInfo
//                                                     {
//                                                         MaterialID = material.green,
//                                                         MeshID = mesh.player_mesh
//                                                     });
//                                                     ecb.AddComponent(player1, new PlayerInfo
//                                                     {
//                                                         type = Value.SquareMaterial.Green,
//                                                         count = 0,
//                                                         turn = false
//                                                     });
//                                                 }     
//                                                 ecb.DestroyEntity(entity);
//                                             }
//                                         }
                                        
//                                         // var delayJob = new DelayJob { DelayTime = 1f };
//                                         // delayJob.Schedule().Complete();
//                                         // Debug.Log("check sleep");                                 
//                                     }
//                                 }
//                             }
//                         }


//                         }
//                 }
//             }
//         }            
        
//         public void OnDestroy(ref SystemState state)
//         {

//         }
//     }
    
// }
