using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct GoInGameServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp)
            .WithAll<
                GoInGameClientRpc,
                ReceiveRpcCommandRequest>()
            .Build(state.EntityManager));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (SystemAPI
            .QueryBuilder()
            .WithAll<
                GoInGameClientRpc,
                ReceiveRpcCommandRequest>()
            .Build()
            .CalculateEntityCount() != 2) 
            return;


        var ecb = state
            .World
            .GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>()
            .CreateCommandBuffer();
        
        state.EntityManager.CreateSingleton(new GameServerInfoData {
            curTurn = MarkType.Cross,
            markedBoard = new(new NativeArray<MarkType>[] {
                new(new MarkType[3], Allocator.Persistent),
                new(new MarkType[3], Allocator.Persistent),
                new(new MarkType[3], Allocator.Persistent),
            }, Allocator.Persistent),
        });

        MarkType dirtyTypeAssign = MarkType.Cross;
        foreach (var (
            goInGameRpcRequest,
            receiveRpcCommandRequest,
            entity) in SystemAPI.Query<
                RefRO<GoInGameClientRpc>,
                RefRO<ReceiveRpcCommandRequest>>()
                .WithEntityAccess()) {
            ecb.AddComponent<NetworkStreamInGame>(receiveRpcCommandRequest.ValueRO.SourceConnection);
            ecb.AddComponent(receiveRpcCommandRequest.ValueRO.SourceConnection, new ClientInfoData {
                clientType = dirtyTypeAssign,
            });
            dirtyTypeAssign = dirtyTypeAssign.Inverse();

            Debug.Log("Hello: " 
                + goInGameRpcRequest.ValueRO.playerName 
                + " :: " + receiveRpcCommandRequest.ValueRO.SourceConnection
                + " :: " + entity);

            ecb.DestroyEntity(entity);
        }
    }
}
