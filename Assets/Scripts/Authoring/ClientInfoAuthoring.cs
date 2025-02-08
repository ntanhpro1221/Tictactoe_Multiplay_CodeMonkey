using Unity.Entities;
using UnityEngine;

public struct ClientInfoData : IComponentData {
    public MarkType clientType;
}

public class ClientInfoAuthoring : MonoBehaviour {
    public MarkType clientType;

    public class Baker : Baker<ClientInfoAuthoring> {
        public override void Bake(ClientInfoAuthoring authoring) {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new ClientInfoData {
                clientType = authoring.clientType,
            });
        }
    }
}