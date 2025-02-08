using Unity.Collections;
using Unity.Entities;

public struct EventGetData : IComponentData {
    public NativeHashMap<EquatableEnum<EventType>, int> eventBoard;
}

