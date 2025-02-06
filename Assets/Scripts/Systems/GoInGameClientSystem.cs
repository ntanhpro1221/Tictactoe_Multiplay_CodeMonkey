using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
partial struct GoInGameClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp)
            .WithAll<NetworkId>()
            .WithNone<NetworkStreamInGame>()
            .Build(state.EntityManager));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = state
            .World
            .GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>()
            .CreateCommandBuffer();

        foreach (var (
            networkId,
            entity) in SystemAPI.Query<
                RefRO<NetworkId>>()
                .WithEntityAccess()
                .WithNone<NetworkStreamInGame>()) {
            ecb.AddComponent<NetworkStreamInGame>(entity);

            Debug.Log("Connected: " + entity + " :: " + networkId.ValueRO.Value);

            Entity rpcEntity = ecb.CreateEntity();
            ecb.AddComponent(rpcEntity, new GoInGameRpcRequest() {
                playerName = new("Alice"),
            });
            ecb.AddComponent<SendRpcCommandRequest>(rpcEntity);
        }
    }
}
