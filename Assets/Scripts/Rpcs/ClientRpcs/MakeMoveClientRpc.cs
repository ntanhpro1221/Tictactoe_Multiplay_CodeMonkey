using Unity.Mathematics;
using Unity.NetCode;

public struct MakeMoveClientRpc : IRpcCommand {
    public int2 movePos;
    public float3 worldPos;
}
