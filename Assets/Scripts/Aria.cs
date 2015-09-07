using UnityEngine;
using System.Collections;

namespace Aria
{
    public enum QLvl
    {
        junk,
        flawed,
        normal,
        fine,
        excellent,
        epic,
        legendary,
        mythic
    }

    [System.Flags]
    public enum ItemType
    {
        Null = 0,
        StdEquip   = 1,
        Accessory  = 2,
        BackEquip  = 4,
        WaistEquip = 8
    }

    [System.Flags]
    public enum Equipment
    {
        Null   = 0,
        Crown  = 1,
        Face   = 2,
        Head   = 3,
        Jacket = 4,
        Pants  = 8,
        Outer  = 12,
        Top    = 16,
        Bottom = 32,
        Inner  = 48,
        Body   = 64,
        Legs   = 128,
        Under  = 192,
        Hands  = 256,
        Feet   = 512
    }
}
