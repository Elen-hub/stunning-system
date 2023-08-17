using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct DropTableStructure
{
    public readonly int Index;
    public readonly int Count;
    public readonly float Probability;
    public DropTableStructure(int index, int count, float probability)
    {
        Index = index;
        Count = count;
        Probability = probability;
    }
}
