using Unity.Entities;
using UnityEngine;

public struct BoardRefData : IComponentData {
    public Entity crossMark;
    public Entity circleMark;
}

public class BoardRefAuthoring : MonoBehaviour {
    public GameObject crossMark;
    public GameObject circleMark;

    public class Baker : Baker<BoardRefAuthoring> {
        public override void Bake(BoardRefAuthoring authoring) {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new BoardRefData {
                crossMark = GetEntity(authoring.crossMark, TransformUsageFlags.Dynamic),
                circleMark = GetEntity(authoring.circleMark, TransformUsageFlags.Dynamic),
            });
        }
    }
}