using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPylon : EnemyUnit
{

    [SerializeField] private int state; 
    //State 0 = make pylon hunting guards
    //State 1 = make pylon defending guards

    private void Start()
    {
        base.Start();

        Invoke("MakeGuard", 5f);
       
    }

    private void MakeGuard()
    {
        if(state == 0) //make pylon hunting guard
        {
            EnemyManager.instance.CreateGuard(transform.position, 1);
            Invoke("MakeGuard", 8f);
        }
        else if(state == 1) //make pylon defending guard
        {
            EnemyManager.instance.CreateGuard(transform.position, 2);
            Invoke("MakeGuard", 8f);
        }
    }

    private void Update()
    {
        base.Update();

        //change state to make defending units if there are player units too nearby
        int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
        int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
        int layermask = static_layer_mask | dynamic_layer_mask;
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 6f, layermask);

        if (colliders.Length > 0) //There is an enemy nearby
        {
            state = 1;
        }
        else //there is not an enemy nearby
        {
            state = 0;
        }

    }

}
