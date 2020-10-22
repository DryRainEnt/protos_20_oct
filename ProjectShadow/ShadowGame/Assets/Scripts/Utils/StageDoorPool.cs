using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDoorPool : MonoBehaviour
{
    public static StageDoorPool instance;
    public AT.SerializableDictionary.SerializableDictionary<string, string> stageDoorPool;

    private void Awake()
    {
        instance = this;
        stageDoorPool = new AT.SerializableDictionary.SerializableDictionary<string, string>();
    }
}
