﻿using Unity.NetCode;
using UnityEngine.Scripting;

[Preserve]
public class GameBoostrap : ClientServerBootstrap {
    public override bool Initialize(string defaultWorldName) {
        AutoConnectPort = 7979;
        return base.Initialize(defaultWorldName);
    }
}
