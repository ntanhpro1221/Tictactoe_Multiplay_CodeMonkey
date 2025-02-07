using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct MakeMoveServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate(new EntityQueryBuilder(Allocator.Temp)
            .WithAll<
                MakeMoveClientRpc,
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
            makeMoveClientRpc,
            receiveRpcRequest,
            entity) in SystemAPI.Query<
                RefRO<MakeMoveClientRpc>,
                RefRO<ReceiveRpcCommandRequest>>()
                .WithEntityAccess()) {
            Debug.Log("Server make move at: " + makeMoveClientRpc.ValueRO.movePos + " | by: " + receiveRpcRequest.ValueRO.SourceConnection);

            Entity markEntity = ecb.Instantiate(SystemAPI.GetSingleton<BoardRefData>().circleMark);
            ecb.SetComponent(markEntity, LocalTransform.FromPosition(makeMoveClientRpc.ValueRO.worldPos));

            ecb.DestroyEntity(entity);
        }
    }
}
