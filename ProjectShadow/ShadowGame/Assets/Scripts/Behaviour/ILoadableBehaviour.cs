using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ILoadableBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => !DataController.instance.OnLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
