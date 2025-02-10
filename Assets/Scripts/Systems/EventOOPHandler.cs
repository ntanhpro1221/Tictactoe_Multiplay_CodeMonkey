using System;
using SingletonUtil;

public class EventOOPHandler : SceneSingleton<EventOOPHandler> {
    private static PropertySet<EventType, Action> eventBoard = new();

    public static void AddListener(EventType eventType, Action callback)
        => eventBoard[eventType] += callback;

    public static void PostEvent(EventType eventType)
        => eventBoard[eventType]?.Invoke();
}