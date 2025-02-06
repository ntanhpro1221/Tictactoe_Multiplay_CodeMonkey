using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
partial struct MakeMoveSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        if (Mouse.current.leftButton.wasReleasedThisFrame) {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            float3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            if (collisionWorld.CastRay(
                new RaycastInput {
                    Start = mousePos,
                    End = mousePos + new float3(0, 0, 9999), 
                    Filter = CollisionFilter.Default,
                }, 
                out Unity.Physics.RaycastHit hit)) {

                Debug.Log("Hit: " + hit.Entity);
            } else {
                Debug.Log("Nope");
            }
        }
    }
}
