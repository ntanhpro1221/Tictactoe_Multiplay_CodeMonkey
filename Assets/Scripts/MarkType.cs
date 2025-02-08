public enum MarkType {
    Nope,
    Cross,
    Circle
}

public static class PlayerTypeExtensions {
    public static MarkType Inverse(this MarkType playerType) => playerType switch {
        MarkType.Cross => MarkType.Circle,
        MarkType.Circle => MarkType.Cross,
        _ => MarkType.Nope
    };
}