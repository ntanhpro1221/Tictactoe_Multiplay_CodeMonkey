using Unity.Collections;
using Unity.NetCode;

public struct GoInGameRpcRequest : IRpcCommand {
    public FixedString128Bytes playerName;
}
