using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using Enum;
using UnityEngine;

namespace System
{
    [UpdateAfter(typeof(InitiatePlayerSystem))]
    [UpdateAfter(typeof(CreateMapSystem))]
    public partial struct MiniMaxSystem : ISystem
    {
        float3 player_positionn;
        float3 enemy_positionn;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Map>();
            state.RequireForUpdate<Square>();
            state.RequireForUpdate<GameMesh>();
            state.RequireForUpdate<GameMaterial>();
            state.RequireForUpdate<MiniMax>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            ////Debug.Log("check_update");
            var material = SystemAPI.GetSingleton<GameMaterial>();
            var mesh = SystemAPI.GetSingleton<GameMesh>();
            var playerPrefab = SystemAPI.GetSingleton<Player>().Playerr;
            var Map = SystemAPI.GetSingleton<Map>();
            
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var square_map = SystemAPI.GetSingleton<GameController>().game_controller;
            float3 PlayerPosition;
            Value.SquareMaterial PlayerType;
            var Position = new NativeArray<int>(4, Allocator.Temp);
            var width = Map.width;
            var height = Map.height;
            foreach(var(localTransform, player,entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess())
            {
                if (player.ValueRW.turn == true){
                    foreach(var(localTransformmm, playerrr,entityyy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess())
                    {
                        if(playerrr.ValueRW.turn == false){
                            enemy_positionn = localTransformmm.ValueRW.Position;
                        }
                    }
                    ////Debug.Log(player.ValueRW.type);
                    PlayerPosition = localTransform.ValueRW.Position;
                    PlayerType = player.ValueRW.type;
                    player_positionn = localTransform.ValueRW.Position;
                    //ecb.DestroyEntity(entity);
                    if(SystemAPI.HasComponent<MiniMax>(entity)){
                        ////Debug.Log("MiniMax");
                        NativeArray<Value.SquareMaterial> map = new NativeArray<Value.SquareMaterial>(width*height, Allocator.Temp);
                        for (int j = 0; j < width*height; j++)
                        {
                            map[j] = square_map[j];
                        }
                        float3 nextMove = SelectMove(ref state,map, player_positionn);
                        if(nextMove.x == -1 && nextMove.z == -1){
                            var game = SystemAPI.GetSingletonEntity<Map>();
                            ecb.SetComponent(game, new EndGame
                                {        
                                    state = Value.GameResult.Win,
                                });
                            state.Enabled = false;
                        }
                        else{
                            ////Debug.Log(nextMove);
                            // ////Debug.Log("check_map");
                            foreach(var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                // ////Debug.Log(squareTransform.ValueRW.Position);
                                // ////Debug.Log(square.ValueRW.State);
                                if(squareTransform.ValueRW.Position.x == nextMove.x && squareTransform.ValueRW.Position.z == nextMove.z){
                                    var index = square.ValueRW.index;
                                    foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
                                        if(playerr.ValueRW.turn == false){
                                            ecb.SetComponent(entityy, new PlayerInfo
                                                {
                                                    type = playerr.ValueRW.type,
                                                    turn = true
                                                });
                                        }
                                    }
                                    var player1 = ecb.Instantiate(playerPrefab);
                                    ecb.SetComponent(player1, new LocalTransform
                                    {
                                        Position = new float3(squareTransform.ValueRW.Position.x, 1, squareTransform.ValueRW.Position.z),
                                        // Position = squareTransform.ValueRW.Position,
                                        Rotation = quaternion.identity,
                                        Scale = 1f
                                    });
                                    if (player.ValueRW.type == Value.SquareMaterial.Red){
                                        square.ValueRW.State = Value.SquareMaterial.Red;
                                        ecb.SetComponent(player1, new MaterialMeshInfo
                                        {
                                            MaterialID = material.red,
                                            MeshID = mesh.player_mesh
                                        });
                                        ecb.AddComponent(player1, new PlayerInfo
                                        {
                                            type = Value.SquareMaterial.Red,
                                            turn = false
                                        });
                                        ecb.AddComponent(player1, new MiniMax
                                        {
                                            count = 0
                                        });

                                    }
                                    else if(player.ValueRW.type == Value.SquareMaterial.Green){
                                        square.ValueRW.State = Value.SquareMaterial.Green;
                                        ecb.SetComponent(player1, new MaterialMeshInfo
                                        {
                                            MaterialID = material.green,
                                            MeshID = mesh.player_mesh
                                        });
                                        ecb.AddComponent(player1, new PlayerInfo
                                        {
                                            type = Value.SquareMaterial.Green,
                                            turn = false
                                        });
                                        ecb.AddComponent(player1, new MiniMax
                                        {
                                            count = 0
                                        });
                                    }

                                }
                            }
                            
                            // ////Debug.Log("check_map2");
                            // foreach(var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                            //     ////Debug.Log(squareTransform.ValueRW.Position);
                            //     ////Debug.Log(square.ValueRW.State);
                            // }
                            ecb.DestroyEntity(entity);
                        }
                    }
                    
                    else{
                        var check_next_move = findValidMove(ref state, square_map, player_positionn);
                        if(check_next_move.Length == 0){
                            var game = SystemAPI.GetSingletonEntity<Map>();
                            ecb.SetComponent(game, new EndGame
                                {        
                                    state = Value.GameResult.Lose,
                                });
                            state.Enabled = false;
                        }
                        //////Debug.Log("Player_choice");
                        if (Input.GetKeyDown(KeyCode.S))
                        {
                            foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                if(squareTransform.ValueRW.Position.x == PlayerPosition.x && squareTransform.ValueRW.Position.z == PlayerPosition.z){
                                    var index = square.ValueRW.index;
                                    if(index - width > 0 && square_map[index-width] == Value.SquareMaterial.Ground){
                                        foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
                                            if(playerr.ValueRW.turn == false){
                                                ecb.SetComponent(entityy, new PlayerInfo
                                                    {
                                                        type = playerr.ValueRW.type,
                                                        turn = true
                                                    });
                                            }
                                        }
                                        foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                            if(new_square.ValueRW.index == index - width){
                                                var player1 = ecb.Instantiate(playerPrefab);
                                                ecb.SetComponent(player1, new LocalTransform
                                                {
                                                    Position = new float3(new_squareTransform.ValueRW.Position.x, 1, new_squareTransform.ValueRW.Position.z),
                                                    Rotation = quaternion.identity,
                                                    Scale = 1f
                                                });
                                                if (player.ValueRW.type == Value.SquareMaterial.Red){
                                                    square.ValueRW.State = Value.SquareMaterial.Red;
                                                    ecb.SetComponent(player1, new MaterialMeshInfo
                                                    {
                                                        MaterialID = material.red,
                                                        MeshID = mesh.player_mesh
                                                    });
                                                    ecb.AddComponent(player1, new PlayerInfo
                                                    {
                                                        type = Value.SquareMaterial.Red,
                                                        turn = false
                                                    });
                                                }
                                                else if(player.ValueRW.type == Value.SquareMaterial.Green){
                                                    square.ValueRW.State = Value.SquareMaterial.Green;
                                                    ecb.SetComponent(player1, new MaterialMeshInfo
                                                    {
                                                        MaterialID = material.green,
                                                        MeshID = mesh.player_mesh
                                                    });
                                                    ecb.AddComponent(player1, new PlayerInfo
                                                    {
                                                        type = Value.SquareMaterial.Green,
                                                        turn = false
                                                    });
                                                }     
                                                ecb.DestroyEntity(entity);
                                            }
                                        }
                                        
                                        // var delayJob = new DelayJob { DelayTime = 1f };
                                        // delayJob.Schedule().Complete();                             
                                    }
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.W))
                        {
                            foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                if(squareTransform.ValueRW.Position.x == PlayerPosition.x && squareTransform.ValueRW.Position.z == PlayerPosition.z){
                                    var index = square.ValueRW.index;
                                    if(index + width < width*height-1 && square_map[index+width] == Value.SquareMaterial.Ground){
                                        foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
                                            if(playerr.ValueRW.turn == false){
                                                ecb.SetComponent(entityy, new PlayerInfo
                                                    {
                                                        type = playerr.ValueRW.type,
                                                        turn = true
                                                    });
                                            }
                                        }
                                        foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                            if(new_square.ValueRW.index == index + width){
                                                var player1 = ecb.Instantiate(playerPrefab);
                                                ecb.SetComponent(player1, new LocalTransform
                                                {
                                                    Position = new float3(new_squareTransform.ValueRW.Position.x, 1, new_squareTransform.ValueRW.Position.z),
                                                    Rotation = quaternion.identity,
                                                    Scale = 1f
                                                });
                                                if (player.ValueRW.type == Value.SquareMaterial.Red){
                                                    square.ValueRW.State = Value.SquareMaterial.Red;
                                                    ecb.SetComponent(player1, new MaterialMeshInfo
                                                    {
                                                        MaterialID = material.red,
                                                        MeshID = mesh.player_mesh
                                                    });
                                                    ecb.AddComponent(player1, new PlayerInfo
                                                    {
                                                        type = Value.SquareMaterial.Red,
                                                        turn = false
                                                    });
                                                }
                                                else if(player.ValueRW.type == Value.SquareMaterial.Green){
                                                    square.ValueRW.State = Value.SquareMaterial.Green;
                                                    ecb.SetComponent(player1, new MaterialMeshInfo
                                                    {
                                                        MaterialID = material.green,
                                                        MeshID = mesh.player_mesh
                                                    });
                                                    ecb.AddComponent(player1, new PlayerInfo
                                                    {
                                                        type = Value.SquareMaterial.Green,
                                                        turn = false
                                                    });
                                                }     
                                                ecb.DestroyEntity(entity);
                                            }
                                        }                                
                                    }
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.A))
                        {
                            foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                if(squareTransform.ValueRW.Position.x == PlayerPosition.x && squareTransform.ValueRW.Position.z == PlayerPosition.z){
                                    var index = square.ValueRW.index;
                                    if(index % width != 0 && square_map[index-1] == Value.SquareMaterial.Ground){
                                        foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
                                            if(playerr.ValueRW.turn == false){
                                                ecb.SetComponent(entityy, new PlayerInfo
                                                    {
                                                        type = playerr.ValueRW.type,
                                                        turn = true
                                                    });
                                            }
                                        }
                                        foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                            if(new_square.ValueRW.index == index - 1){
                                                var player1 = ecb.Instantiate(playerPrefab);
                                                ecb.SetComponent(player1, new LocalTransform
                                                {
                                                    Position = new float3(new_squareTransform.ValueRW.Position.x, 1, new_squareTransform.ValueRW.Position.z),
                                                    Rotation = quaternion.identity,
                                                    Scale = 1f
                                                });
                                                if (player.ValueRW.type == Value.SquareMaterial.Red){
                                                    square.ValueRW.State = Value.SquareMaterial.Red;
                                                    ecb.SetComponent(player1, new MaterialMeshInfo
                                                    {
                                                        MaterialID = material.red,
                                                        MeshID = mesh.player_mesh
                                                    });
                                                    ecb.AddComponent(player1, new PlayerInfo
                                                    {
                                                        type = Value.SquareMaterial.Red,
                                                        turn = false
                                                    });
                                                }
                                                else if(player.ValueRW.type == Value.SquareMaterial.Green){
                                                    square.ValueRW.State = Value.SquareMaterial.Green;
                                                    ecb.SetComponent(player1, new MaterialMeshInfo
                                                    {
                                                        MaterialID = material.green,
                                                        MeshID = mesh.player_mesh
                                                    });
                                                    ecb.AddComponent(player1, new PlayerInfo
                                                    {
                                                        type = Value.SquareMaterial.Green,
                                                        turn = false
                                                    });
                                                }     
                                                ecb.DestroyEntity(entity);
                                            }
                                        }
                                        
                                        // var delayJob = new DelayJob { DelayTime = 1f };
                                        // delayJob.Schedule().Complete();                               
                                    }
                                }
                            }
                        }
                        else if (Input.GetKeyDown(KeyCode.D))
                        {
                            foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                if(squareTransform.ValueRW.Position.x == PlayerPosition.x && squareTransform.ValueRW.Position.z == PlayerPosition.z){
                                    var index = square.ValueRW.index;
                                    if((index+1) % width != 0 && square_map[index+1] == Value.SquareMaterial.Ground){
                                        foreach(var(localTransformm, playerr,entityy) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInfo>>().WithEntityAccess()){
                                            if(playerr.ValueRW.turn == false){
                                                ecb.SetComponent(entityy, new PlayerInfo
                                                    {
                                                        type = playerr.ValueRW.type,
                                                        turn = true
                                                    });
                                            }
                                        }
                                        foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                            if(new_square.ValueRW.index == index + 1){
                                                var player1 = ecb.Instantiate(playerPrefab);
                                                ecb.SetComponent(player1, new LocalTransform
                                                {
                                                    Position = new float3(new_squareTransform.ValueRW.Position.x, 1, new_squareTransform.ValueRW.Position.z),
                                                    Rotation = quaternion.identity,
                                                    Scale = 1f
                                                });
                                                if (player.ValueRW.type == Value.SquareMaterial.Red){
                                                    square.ValueRW.State = Value.SquareMaterial.Red;
                                                    ecb.SetComponent(player1, new MaterialMeshInfo
                                                    {
                                                        MaterialID = material.red,
                                                        MeshID = mesh.player_mesh
                                                    });
                                                    ecb.AddComponent(player1, new PlayerInfo
                                                    {
                                                        type = Value.SquareMaterial.Red,
                                                        turn = false
                                                    });
                                                }
                                                else if(player.ValueRW.type == Value.SquareMaterial.Green){
                                                    square.ValueRW.State = Value.SquareMaterial.Green;
                                                    ecb.SetComponent(player1, new MaterialMeshInfo
                                                    {
                                                        MaterialID = material.green,
                                                        MeshID = mesh.player_mesh
                                                    });
                                                    ecb.AddComponent(player1, new PlayerInfo
                                                    {
                                                        type = Value.SquareMaterial.Green,
                                                        turn = false
                                                    });
                                                }     
                                                ecb.DestroyEntity(entity);
                                            }
                                        }
                                        
                                        // var delayJob = new DelayJob { DelayTime = 1f };
                                        // delayJob.Schedule().Complete();                              
                                    }
                                }
                            }
                        }


                        }
                    
                }
            }
        }            
        
        public void OnDestroy(ref SystemState state)
        {

        }
        private NativeArray<float3> findValidMove(ref SystemState state,NativeArray<Value.SquareMaterial> map, float3 player_position){
            ////Debug.Log(player_positionn);
            ////Debug.Log("----");
            ////Debug.Log(enemy_positionn);
            NativeArray<float3> result = new NativeArray<float3>(4, Allocator.Temp);
            var i = 0;
            foreach (var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                if(squareTransform.ValueRW.Position.x == player_position.x  && squareTransform.ValueRW.Position.z == player_position.z){
                    var index = square.ValueRW.index;
                    var Map = SystemAPI.GetSingleton<Map>();
                    var width = Map.width;
                    var height = Map.height;
                    
                    if((index+1)%width != 0){
                        if(map[index+1] == Value.SquareMaterial.Ground){
                            foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                if(new_square.ValueRW.index == index +1){
                                    result[i] = new_squareTransform.ValueRW.Position;
                                    ////Debug.Log("check1");
                                    ////Debug.Log(result[i]);
                                    i++;
                                }
                            }
                        }
                    }
                    if(index + width < width * height - 1){
                        if(map[index+width] == Value.SquareMaterial.Ground){
                            foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                if(new_square.ValueRW.index == index + width){
                                    result[i] = new_squareTransform.ValueRW.Position;
                                    ////Debug.Log("check2");
                                    ////Debug.Log(result[i]);
                                    i++;
                                }
                            }
                        }
                    }
                    if(index - width > 0){
                        if(map[index-width] == Value.SquareMaterial.Ground){
                            foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                if(new_square.ValueRW.index == index - width){
                                    result[i] = new_squareTransform.ValueRW.Position;
                                    ////Debug.Log("check3");
                                    ////Debug.Log(result[i]);
                                    i++;
                                }
                            }
                        }
                    }
                    if(index%width != 0){
                        if(map[index-1] == Value.SquareMaterial.Ground){
                            foreach(var(new_squareTransform, new_square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                if(new_square.ValueRW.index == index -1){
                                    result[i] = new_squareTransform.ValueRW.Position;
                                    ////Debug.Log("check4");
                                    ////Debug.Log(result[i]);
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            NativeArray<float3> result1 = new NativeArray<float3>(i, Allocator.Temp);
            for (int j = 0; j < i; j++)
            {
                result1[j] = result[j];
            }
            return result1;
        }
    
        private NativeArray<Value.SquareMaterial> MakeMove (ref SystemState state,NativeArray<Value.SquareMaterial> map, float3 next_position, float3 player_position){
            Value.SquareMaterial next_state = Value.SquareMaterial.Ground;
            NativeArray<Value.SquareMaterial> mapp = map;
            foreach(var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                if (squareTransform.ValueRW.Position.x == player_position.x && squareTransform.ValueRW.Position.z == player_position.z){
                    var index = square.ValueRW.index;
                    next_state = mapp[index];
                }
            }
            foreach(var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                if (squareTransform.ValueRW.Position.x == next_position.x && squareTransform.ValueRW.Position.z == next_position.z){
                    var index = square.ValueRW.index;
                    
                    mapp[index] = next_state;
                    // ////Debug.Log(index + ":" + next_state);
                    // ////Debug.Log(squareTransform.ValueRW.Position);
                    // ////Debug.Log(square.ValueRW.State);
                    if(player_position.x == player_positionn.x && player_position.z == player_positionn.z){
                        player_positionn = next_position;
                    }
                    if(player_position.x == enemy_positionn.x && player_position.z == enemy_positionn.z){
                        enemy_positionn = next_position;
                    }
                }
            }
            return mapp;
        }

        private float3 SelectMove(ref SystemState state, NativeArray<Value.SquareMaterial> map,float3 player_position){
            NativeArray<float3> validMove = findValidMove(ref state, map, player_position);
            
            var randomIndex = -1;
            var Position = new NativeArray<int>(4, Allocator.Temp);
            if (validMove.Length == 0){
                return new float3(-1,-1,-1);
            }

            ////Debug.Log("run minimax");
            var move = MiniMax(ref state,map,player_position,validMove,0,-64,64);
            ////Debug.Log(move);
            if (move.Item2.x != -1){
                ////Debug.Log("aaaaa");
                foreach(var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                    if(squareTransform.ValueRW.Position.x == move.Item2.x && squareTransform.ValueRW.Position.z == move.Item2.z){
                        ////Debug.Log(square.ValueRW.State);
                    }
                }
                return move.Item2;
            }
            else{
                ////Debug.Log(player_position);
                foreach(var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                    if(squareTransform.ValueRW.Position.x == player_position.x && squareTransform.ValueRW.Position.z == player_position.z){
                        var index = square.ValueRW.index;
                        var i = 0;
                        var Map = SystemAPI.GetSingleton<Map>();
                        var mappp = SystemAPI.GetSingleton<GameController>().game_controller;
                        var width = Map.width;
                        var height = Map.height;
                        ////Debug.Log(index);
                        if((index+1)%width != 0){
                            ////Debug.Log("case1");
                            ////Debug.Log(map[index+1]);
                            if(mappp[index+1] == Value.SquareMaterial.Ground){
                                Position[i] = index + 1;
                                i++;
                            }
                        }
                        if(index + width < width * height - 1){
                            ////Debug.Log("case2");
                            ////Debug.Log(map[index+width]);
                            if(mappp[index+width] == Value.SquareMaterial.Ground){
                                Position[i] = index + width;
                                i++;
                            }
                        }
                        if(index - width > 0){
                            ////Debug.Log("case3");
                            ////Debug.Log(map[index-width]);
                            if(mappp[index-width] == Value.SquareMaterial.Ground){
                                Position[i] = index - width;
                                i++;
                            }
                        }
                        if(index%width != 0){
                            ////Debug.Log("case4");
                            ////Debug.Log(map[index-1]);
                            if(mappp[index-1] == Value.SquareMaterial.Ground){
                                Position[i] = index - 1;
                                i++;
                            }
                        }
                        if(i==0){
                            ////Debug.Log("i=0");
                            return new float3(-1,-1,-1);
                        }
                        else{
                            var baseTime = System.DateTime.Now.TimeOfDay.TotalSeconds;
                            var seed = (int)(float)(baseTime + SystemAPI.Time.ElapsedTime);
                            var randomData = Unity.Mathematics.Random.CreateFromIndex((uint) seed);
                            randomIndex = randomData.NextInt(0, i);
                            ////Debug.Log(i);
                            ////Debug.Log(randomIndex);
                            foreach(var(squareTransformm, squaree) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                                if(squaree.ValueRW.index == Position[randomIndex]){
                                    return squareTransformm.ValueRW.Position;
                                }
                            }
                        }
                    }
                }
                return new float3(-1, -1, -1);
            }
        }
        private (int,float3) MiniMax(ref SystemState state,NativeArray<Value.SquareMaterial> map, float3 player_position,NativeArray<float3> validMove,int depth,int anpha,int beta){
            float3 bestMove = new float3(-1,-1,-1);
            float3 none = new float3(-1,-1,-1);
            if (depth == 10){
                ////Debug.Log("full");
                return (evaluateMove(ref state,map,player_position),none);      
            }
            if (validMove.Length == 0){
                ////Debug.Log("fullllllllllll");
                return (evaluateMove(ref state,map,player_position),none); 
            }
            foreach (var a_move in validMove){
                NativeArray<Value.SquareMaterial> new_map = map;
                NativeArray<float3> new_validmove = validMove;
                if(player_position.x == player_positionn.x && player_position.z == player_positionn.z){
                    ////Debug.Log("player");
                    ////Debug.Log(player_positionn);
                    new_map = MakeMove(ref state,map,a_move,player_position);
                    new_validmove = findValidMove(ref state,new_map, enemy_positionn);
                    if (new_validmove.Length==3){
                        return (100,none);
                    }
                    if (new_validmove.Length==2){
                        return (100,none);
                    }
                    var res = MiniMax(ref state,new_map,enemy_positionn,new_validmove,depth+1,-beta, -anpha);
                    var new_val = -res.Item1;
                    if (new_val > anpha){
                        anpha = new_val;
                        bestMove = a_move;
                    }
                    if (anpha >= beta){
                        return (anpha, bestMove);
                    }
                }
                else if (player_position.x == enemy_positionn.x && player_position.z == enemy_positionn.z){
                    ////Debug.Log("enemy");
                    ////Debug.Log(enemy_positionn);
                    new_map = MakeMove(ref state,map,a_move,player_position);
                    new_validmove = findValidMove(ref state,new_map, player_positionn);
                    if (new_validmove.Length==3){
                        return (100,none);
                    }
                    if (new_validmove.Length==2){
                        return (100,none);
                    }
                    var res = MiniMax(ref state,new_map,player_positionn,new_validmove,depth+1,-beta, -anpha);
                    var new_val = -res.Item1;
                    if (new_val > anpha){
                        anpha = new_val;
                        bestMove = a_move;
                    }
                    if (anpha >= beta){
                        return (anpha, bestMove);
                    }
                }
            }
            return (-100, bestMove);
        }
        private int evaluateMove(ref SystemState state,NativeArray<Value.SquareMaterial> map,float3 player_position){
            int score_player = 0;
            var Map = SystemAPI.GetSingleton<Map>();
            var width = Map.width;
            var height = Map.height;

            Value.SquareMaterial statee = Value.SquareMaterial.Red;
            foreach(var(squareTransform, square) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Square>>()){
                if (squareTransform.ValueRW.Position.x == player_position.x && squareTransform.ValueRW.Position.z == player_position.z){
                    var index = square.ValueRW.index;
                    statee = map[index];
                    ////Debug.Log("index: "+index);
                    ////Debug.Log(statee);
                    ////Debug.Log(square.ValueRW.State);
                }
            }

            for (int i = 0; i < width*height; i++)
            {
                if(map[i] == statee){
                    score_player += 1;
                }
            }            
            ////Debug.Log("score_player: " + score_player);
            if(player_position.x == player_positionn.x && player_position.z == player_positionn.z){
                var enemy_move = findValidMove(ref state,map,enemy_positionn);
                ////Debug.Log(enemy_move.Length);
                return score_player - enemy_move.Length;    
            }
            else{
                var enemy_move = findValidMove(ref state,map,player_positionn);
                return score_player - enemy_move.Length; 
            }
        }
        
    }
}
    

