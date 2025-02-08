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
            MarkType playerType = SystemAPI.GetComponent<ClientInfoData>(receiveRpcRequest.ValueRO.SourceConnection).clientType;
            var gameInfo = SystemAPI.GetSingletonRW<GameServerInfoData>();
            if (gameInfo.ValueRO.markedBoard
                [makeMoveClientRpc.ValueRO.movePos.x]
                [makeMoveClientRpc.ValueRO.movePos.y] != MarkType.Nope) {
                Debug.LogWarning("This place has already been marked");
            } else if (playerType != gameInfo.ValueRO.curTurn) {
                Debug.LogWarning("Client make wrong turn");
            } else if (playerType == MarkType.Nope) {
                Debug.LogError("Client Info has not been started yet");
            } else {
                var tmpMaredLine = gameInfo.ValueRO.markedBoard[makeMoveClientRpc.ValueRO.movePos.x];
                tmpMaredLine[makeMoveClientRpc.ValueRO.movePos.y] = gameInfo.ValueRO.curTurn;
                gameInfo.ValueRW.markedBoard[makeMoveClientRpc.ValueRO.movePos.x] = tmpMaredLine;
                gameInfo.ValueRW.curTurn = gameInfo.ValueRO.curTurn.Inverse();

                Debug.Log("Server make move at: " + makeMoveClientRpc.ValueRO.movePos + " | by: " + receiveRpcRequest.ValueRO.SourceConnection);
                Entity markEntity = ecb.Instantiate(playerType switch {
                    MarkType.Cross => SystemAPI.GetSingleton<BoardRefData>().crossMark,
                    MarkType.Circle => SystemAPI.GetSingleton<BoardRefData>().circleMark,
                });
                ecb.SetComponent(markEntity, LocalTransform.FromPosition(makeMoveClientRpc.ValueRO.worldPos));
            }

            ecb.DestroyEntity(entity);
        }
    }
}
