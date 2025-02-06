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
                GoInGameRpcRequest,
                ReceiveRpcCommandRequest>()
            .Build(state.EntityManager));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = state
            .World
            .GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>()
            .CreateCommandBuffer();

        foreach (var (
            goInGameRpcRequest,
            receiveRpcCommandRequest,
            entity) in SystemAPI.Query<
                RefRO<GoInGameRpcRequest>,
                RefRO<ReceiveRpcCommandRequest>>()
                .WithEntityAccess()) {
            ecb.AddComponent<NetworkStreamInGame>(receiveRpcCommandRequest.ValueRO.SourceConnection);

            Debug.Log("Hello: " 
                + goInGameRpcRequest.ValueRO.playerName 
                + " :: " + receiveRpcCommandRequest.ValueRO.SourceConnection
                + " :: " + entity);

            ecb.DestroyEntity(entity);
        }
    }
}
