using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct EventECSSystem : ISystem {
    private NativeArray<EventType> enumValues;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        enumValues = new((EventType[])Enum.GetValues(typeof(EventType)), Allocator.Persistent);
        NativeHashMap<EquatableEnum<EventType>, int> enumEventPatternBoard = new(enumValues.Length, Allocator.Persistent);
        foreach (EventType enumValue in enumValues) enumEventPatternBoard.Add(enumValue, default);

        state.EntityManager.CreateSingleton(new EventGetData {
            eventBoard = enumEventPatternBoard,
        });
        state.EntityManager.CreateSingleton(new EventPostData {
            eventBoard = enumEventPatternBoard,
        });
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ref var eventGetBoard = ref SystemAPI.GetSingletonRW<EventGetData>().ValueRW.eventBoard;
        ref var eventPostBoard = ref SystemAPI.GetSingletonRW<EventPostData>().ValueRW.eventBoard;

        foreach (EventType key in enumValues) {
            eventGetBoard[key] = eventPostBoard[key];
            while (eventPostBoard[key]-- > 0) EventOOPHandler.PostEvent(key);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        try {
            enumValues.Dispose();
        } catch { }
        try {
            SystemAPI.GetSingleton<EventGetData>().eventBoard.Dispose();
        } catch { }
        try {
            SystemAPI.GetSingleton<EventPostData>().eventBoard.Dispose();
        } catch { }
    }
}