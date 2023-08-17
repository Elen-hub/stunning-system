using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInstallable : ITileSetable
{
    int Index { get; }
    ulong WorldID { get; }
    public void InstallMode(bool isInstallMode, Vector3 position);
}
