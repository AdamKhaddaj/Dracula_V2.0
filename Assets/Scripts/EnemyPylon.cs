using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPylon : EnemyUnit
{

    private void Start()
    {
        base.Start();

        Invoke("MakeGuard", 5f);
       
    }

    private void MakeGuard()
    {
        EnemyManager.instance.CreateHuntingGuard(transform.position);
        Invoke("MakeGuard", 8f); 
    }

}
