using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BoardPosData : IComponentData {
    public int2 pos;
}

public class BoardPosAuthoring : MonoBehaviour {
    public int2 pos;

    public class Baker : Baker<BoardPosAuthoring> {
        public override void Bake(BoardPosAuthoring authoring) {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new BoardPosData {
                pos = authoring.pos,
            });
        }
    }
}