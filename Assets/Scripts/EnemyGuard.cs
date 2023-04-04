using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuard : EnemyUnit
{
    public void Move(Vector3 destination)
    {
        // set destination of all selected units, and set their states to 1 (which will be the state representing "commanded movement") for all dynamic units

        base.agent.destination = destination;

    }
}
