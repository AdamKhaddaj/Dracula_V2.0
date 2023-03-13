using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveCode : MonoBehaviour
{
    Vector3 center;
    private void Start()
    {
        center = transform.position;
    }
    void Update()
    {
        Vector3 newpos = center + new Vector3(Mathf.Sin(Time.time *1.5f), 0, Mathf.Cos(Time.time*1.5f)) * 3;
        transform.position = newpos;
    }
}
