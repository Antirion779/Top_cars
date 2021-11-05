using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    // Start is called before the first frame update
    public void setDir(Vector2 dir)
    {
        transform.LookAt(dir);
    }
}
