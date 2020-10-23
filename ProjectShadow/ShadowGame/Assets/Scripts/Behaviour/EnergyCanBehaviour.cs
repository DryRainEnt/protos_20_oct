using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCanBehaviour : ItemBehaviour
{
    public override void ConsumedBehaviour()
    {
        GetUsed();
    }
}
