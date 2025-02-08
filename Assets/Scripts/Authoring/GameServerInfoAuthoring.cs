using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct GameServerInfoData : IComponentData {
    public MarkType curTurn;
    public NativeArray<NativeArray<MarkType>> markedBoard;
}

public class GameServerInfoAuthoring : MonoBehaviour {
    public MarkType curTurn;

    public class Baker : Baker<GameServerInfoAuthoring> {
        public override void Bake(GameServerInfoAuthoring authoring) {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new GameServerInfoData {
                curTurn = authoring.curTurn,
            });
        }
    }
}
