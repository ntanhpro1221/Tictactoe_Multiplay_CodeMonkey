using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
partial struct MakeMoveClientSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (Mouse.current.leftButton.wasReleasedThisFrame) {
            Debug.Log("Click");

            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            float3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            if (collisionWorld.CastRay(
                new RaycastInput {
                    Start = mousePos,
                    End = mousePos + new float3(0, 0, 9999), 
                    Filter = CollisionFilter.Default,
                }, 
                out Unity.Physics.RaycastHit hit)) {

                if (state.EntityManager.HasComponent<BoardPosData>(hit.Entity)) {
                    int2 movePos = state.EntityManager.GetComponentData<BoardPosData>(hit.Entity).pos;
                    float3 worldPos = state.EntityManager.GetComponentData<LocalToWorld>(hit.Entity).Position;
                    Debug.Log("Hit: " + movePos);

                    // make request to server
                    Entity entity = state.EntityManager.CreateEntity(
                        typeof(MakeMoveClientRpc), 
                        typeof(SendRpcCommandRequest));
                    state.EntityManager.SetComponentData(
                        entity,
                        new MakeMoveClientRpc {
                            movePos = movePos,
                            worldPos = worldPos,
                        });
                } else {
                    Debug.Log("Having hit something that does not contain GridPosData");
                }
            } else {
                Debug.Log("lmao");
            }
        }
    }
}
