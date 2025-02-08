using Unity.Collections;
using Unity.Entities;

public struct EventPostData : IComponentData {
    public NativeHashMap<EquatableEnum<EventType>, int> eventBoard;
}

