using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustmentScript : MonoBehaviour {
    bool done = false; 
	// Use this for initialization
	public void Resize()
    {
        if (!done)
        {
            transform.localScale= new Vector3 (0.8f, 1f, 1f);
            done = true;
        }
    }
}
