using Unity.Entities;
using UnityEngine;

public struct AliceTag : IComponentData, IEnableableComponent { }

public class AliceAuthoring : MonoBehaviour {
    public class Baker : Baker<AliceAuthoring> {
        public override void Bake(AliceAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AliceTag { });
            SetComponentEnabled<AliceTag>(entity, false);
        }
    }
}