using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBarrier : EnemyUnit
{

    private bool makeDefenders;

    void Start()
    {
        base.Start();
        makeDefenders = false;
        Invoke("MakeGuard", 8f);
    }

    void Update()
    {
        base.Update();

        //if pylon is nearby, send units to kill it
        int static_layer = LayerMask.GetMask("StaticPlayerUnits");
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 12f, static_layer);

        if (colliders.Length > 0) //There is a pylon near enough to attack it
        {
            makeDefenders = true;
        }
        else
        {
            makeDefenders = false;
        }

    }

    private void MakeGuard()
    {
        if (makeDefenders)
        {
            EnemyManager.instance.CreateGuard(transform.position, 1);
        }

        Invoke("MakeGuard", 6f);

    }

}
