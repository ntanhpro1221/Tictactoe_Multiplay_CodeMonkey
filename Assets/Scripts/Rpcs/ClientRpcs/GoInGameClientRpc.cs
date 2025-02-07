using Unity.Collections;
using Unity.NetCode;

public struct GoInGameClientRpc : IRpcCommand {
    public FixedString128Bytes playerName;
}
