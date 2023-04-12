using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuard : EnemyUnit {

    public int type, state;

    private Vector3 home;

    //Type will be determined upon creation, but can sometimes change. In general, it will determine the behaviour of the guard (who they target, etc.)

    //0 = Defend Location
    //1 = Pylon Hunter
    //2 = Kill units hitting ally pylons

	public void Move(Vector3 destination) {

		base.agent.destination = destination;

	}

    private void Awake()
    {
        base.Awake();
        type = 0;
    }

    private void Start()
    {
        base.Start();
        home = transform.position;

        //For now, 0 = idle, 1 = moving, 2 = moving towards enemy, 3 = attacking
        //Target will be the ID of the enemy unit it is targetting for either attacking or moving towards, or -1 if it is not targetting anything
        state = 0;

        //Animation stuff
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        animator.SetInteger("attackType", 0);
    }

    public void SetType(int t)
    {
        type = t;
    }

    private void Update()
    {
        base.Update();
        if (type == 0) //Defend location type guard
        {

            //If they get too far from their origin point, they head back
            if(Vector3.Distance(transform.position, home) > 20f)
            {
                state = 3;
                target = -1;
            }

            if(state == 0)
            {
                base.agent.isStopped = true;
                rigidbody.velocity = Vector3.zero;

                int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
                int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
                int sieger_layer_mask = 1 << LayerMask.NameToLayer("Sieger");
                int layermask = static_layer_mask | dynamic_layer_mask;
                Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 6f, layermask);

                if (colliders.Length > 0) //There is an enemy in aggro
                {
                    //A target needs to be picked if there are multiple enemies. Pick the nearest enemy
                    float closest = Mathf.Infinity;
                    int closest_target = -1;

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        PlayerUnit playerUnit = colliders[i].GetComponent<PlayerUnit>();
                        if (Vector3.Distance(transform.position, playerUnit.gameObject.transform.position) < closest)
                        {
                            closest_target = playerUnit.GetID();
                            closest = Vector3.Distance(transform.position, playerUnit.gameObject.transform.position);
                        }
                    }

                    target = closest_target;
                    state = 1;
                    base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                }
            }

            if (state == 1)
            {
                //first check if the unit exists anymore, if not, stop coroutine
                if (PlayerManager.instance.GetUnit(target) == null)
                {
                    state = 0;
                    target = -1;
                }
                else
                {
                    PlayerUnit p = PlayerManager.instance.GetUnit(target);

                    //Check if either a) the target is close enough that this unit can attack it, or b) this unit should move towrads it

                    if (Vector3.Distance(transform.position, base.destination) < 1f)
                    {
                        state = 2;
                        base.SetDestination(transform.position);
                    }
                    else
                    {
                        base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                    }
                }
            }

            if (state == 2) //This state means this unit is capable of attacking it's target
            {
                //first check if the unit exists anymore, if not, exit state
                if (PlayerManager.instance.GetUnit(target) == null)
                {
                    state = 0;
                    target = -1;
                }
                else if (Vector3.Distance(transform.position, PlayerManager.instance.GetUnit(target).transform.position) >= 1f) //if target moves too far away, start moving towards them
                {
                    state = 1;
                    base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                }
                else //Otherwise, attack animation will continue playing, and trigger the deal damage function

                {
                    transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation((PlayerManager.instance.GetUnit(target).transform.position - transform.position), Vector3.up).eulerAngles.y, 0f);
                }

            }

            if (state == 3)
            {
                if (Vector3.Distance(transform.position, home) > 15f) //will run home no matter what when between 20 and 15 units
                {
                    base.SetDestination(home);
                }
                else if (Vector3.Distance(transform.position, home) < 2f) //will return to idle if they get close enough to home
                {
                    state = 0;
                }
                else //will search for enemies around to attack if they're less than 15 units home
                {
                    int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
                    int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
                    int layermask = static_layer_mask | dynamic_layer_mask;
                    Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 6f, layermask);

                    if (colliders.Length > 0) //There is an enemy in aggro
                    {
                        //A target needs to be picked if there are multiple enemies. Pick the nearest enemy
                        float closest = Mathf.Infinity;
                        int closest_target = -1;

                        for (int i = 0; i < colliders.Length; i++)
                        {
                            PlayerUnit playerUnit = colliders[i].GetComponent<PlayerUnit>();
                            if (Vector3.Distance(transform.position, playerUnit.gameObject.transform.position) < closest)
                            {
                                closest_target = playerUnit.GetID();
                                closest = Vector3.Distance(transform.position, playerUnit.gameObject.transform.position);
                            }
                        }

                        target = closest_target;
                        state = 1;
                        base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                    }
                }
            }

            //Animation handling
            if (state == 0)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", false);
            }
            else if (state == 1)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isAttacking", false);
            }
            else if (state == 2)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", true);
                int attackType = UnityEngine.Random.Range(0, 2);
                animator.SetInteger("attackType", attackType);
            }
        }

        else if(type == 1) //Hunting Type Guard
        {
            if (state == 0)
            {
                base.agent.isStopped = true;
                rigidbody.velocity = Vector3.zero;

                int static_layer_mask = LayerMask.GetMask("StaticPlayerUnits");
                int layermask = static_layer_mask;
                Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 1000f, layermask);

                if (colliders.Length > 0) //There is an enemy in aggro
                {
                    //A target needs to be picked if there are multiple enemies. Pick the nearest enemy
                    float closest = Mathf.Infinity;
                    int closest_target = -1;

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        PlayerUnit playerUnit = colliders[i].GetComponent<PlayerUnit>();
                        if (Vector3.Distance(transform.position, playerUnit.gameObject.transform.position) < closest)
                        {
                            closest_target = playerUnit.GetID();
                            closest = Vector3.Distance(transform.position, playerUnit.gameObject.transform.position);
                        }
                    }

                    target = closest_target;
                    state = 1;
                    base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                }
                else
                {
                    Collider[] colliders2 = Physics.OverlapSphere(gameObject.transform.position, 1000f, LayerMask.GetMask("DynamicPlayerUnits"));

                    if (colliders2.Length > 0) //There is an enemy in aggro
                    {
                        //A target needs to be picked if there are multiple enemies. Pick the nearest enemy
                        float closest = Mathf.Infinity;
                        int closest_target = -1;

                        for (int i = 0; i < colliders2.Length; i++)
                        {
                            PlayerUnit playerUnit = colliders2[i].GetComponent<PlayerUnit>();
                            if (Vector3.Distance(transform.position, playerUnit.gameObject.transform.position) < closest)
                            {
                                closest_target = playerUnit.GetID();
                                closest = Vector3.Distance(transform.position, playerUnit.gameObject.transform.position);
                            }
                        }

                        target = closest_target;
                        state = 1;
                        base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                    }

                }
            }

            if (state == 1)
            {
                //first check if the unit exists anymore, if not, stop coroutine
                if (PlayerManager.instance.GetUnit(target) == null)
                {
                    state = 0;
                    target = -1;
                }
                else
                {
                    PlayerUnit p = PlayerManager.instance.GetUnit(target);

                    //Check if either a) the target is close enough that this unit can attack it, or b) this unit should move towrads it

                    if (Vector3.Distance(transform.position, base.destination) < 1f)
                    {
                        state = 2;
                        base.SetDestination(transform.position);
                    }
                    else
                    {
                        base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                    }
                }
            }

            if (state == 2) //This state means this unit is capable of attacking it's target
            {
                //first check if the unit exists anymore, if not, exit state
                if (PlayerManager.instance.GetUnit(target) == null)
                {
                    state = 0;
                    target = -1;
                }
                else if (Vector3.Distance(transform.position, PlayerManager.instance.GetUnit(target).transform.position) >= 1f) //if target moves too far away, start moving towards them
                {
                    state = 1;
                    base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                }
                else //Otherwise, attack animation will continue playing, and trigger the deal damage function

                {
                    transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation((PlayerManager.instance.GetUnit(target).transform.position - transform.position), Vector3.up).eulerAngles.y, 0f);
                }

            }

            //Animation handling
            if (state == 0)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", false);
            }
            else if (state == 1)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isAttacking", false);
            }
            else if (state == 2)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", true);
                int attackType = UnityEngine.Random.Range(0, 2);
                animator.SetInteger("attackType", attackType);
            }
        }

        else if (type == 2) //Defend Pylon
        {

            //If they get too far from their origin point, they head back
            if (Vector3.Distance(transform.position, home) > 10f)
            {
                state = 3;
                target = -1;
            }

            if (state == 0)
            {
                base.agent.isStopped = true;
                rigidbody.velocity = Vector3.zero;

                int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
                int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
                int layermask = static_layer_mask | dynamic_layer_mask;
                Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 6f, layermask);

                if (colliders.Length > 0) //There is an enemy in aggro
                {
                    int pick = -1;
                    int secondpick = -1;
                    //This one will target units attacking an enemy pylon
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        PlayerUnit playerUnit = colliders[i].GetComponent<PlayerUnit>();
                        if (EnemyManager.instance.GetUnit(playerUnit.target) != null) //gotta check this one for null first incase there is no target
                        {
                            if(EnemyManager.instance.GetUnit(playerUnit.target).GetComponent<EnemyPylon>() != null) //then we check to see if target is an enemypylon
                            {
                                pick = playerUnit.GetID();
                            }
                        }
                        else
                        {
                            secondpick = playerUnit.GetID();
                        }
                    }

                    if(pick != -1)
                    {
                        target = pick;
                    }
                    else
                    {
                        target = secondpick;
                    }

                    if(PlayerManager.instance.GetUnit(target) != null)
                    {
                        state = 1;
                        base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                    }
                    else
                    {
                        target = -1;
                    }
                }
            }

            if (state == 1)
            {
                //first check if the unit exists anymore, if not, stop coroutine
                if (PlayerManager.instance.GetUnit(target) == null)
                {
                    state = 0;
                    target = -1;
                }
                else
                {
                    PlayerUnit p = PlayerManager.instance.GetUnit(target);

                    //Check if either a) the target is close enough that this unit can attack it, or b) this unit should move towrads it

                    if (Vector3.Distance(transform.position, base.destination) < 1f)
                    {
                        state = 2;
                        base.SetDestination(transform.position);
                    }
                    else
                    {
                        base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                    }
                }
            }

            if (state == 2) //This state means this unit is capable of attacking it's target
            {
                //first check if the unit exists anymore, if not, exit state
                if (PlayerManager.instance.GetUnit(target) == null)
                {
                    state = 0;
                    target = -1;
                }
                else if (Vector3.Distance(transform.position, PlayerManager.instance.GetUnit(target).transform.position) >= 1f) //if target moves too far away, start moving towards them
                {
                    state = 1;
                    base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                }
                else //Otherwise, attack animation will continue playing, and trigger the deal damage function

                {
                    transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation((PlayerManager.instance.GetUnit(target).transform.position - transform.position), Vector3.up).eulerAngles.y, 0f);
                }

            }

            if (state == 3)
            {
                if (Vector3.Distance(transform.position, home) >= 6f) //will run home no matter what when between 20 and 15 units
                {
                    base.SetDestination(home);
                }

                if (Vector3.Distance(transform.position, home) < 6f && Vector3.Distance(transform.position, home) > 2f) //once they're close enough to home, they start looking for dudes to fight
                {
                    int dynamic_layer_mask = 1 << LayerMask.NameToLayer("DynamicPlayerUnits");
                    int static_layer_mask = 1 << LayerMask.NameToLayer("StaticPlayerUnits");
                    int layermask = static_layer_mask | dynamic_layer_mask;
                    Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 6f, layermask);

                    if (colliders.Length > 0) //There is an enemy in aggro
                    {
                        //A target needs to be picked if there are multiple enemies. Pick the nearest enemy
                        float closest = Mathf.Infinity;
                        int closest_target = -1;

                        for (int i = 0; i < colliders.Length; i++)
                        {
                            PlayerUnit playerUnit = colliders[i].GetComponent<PlayerUnit>();
                            if (Vector3.Distance(transform.position, playerUnit.gameObject.transform.position) < closest)
                            {
                                closest_target = playerUnit.GetID();
                                closest = Vector3.Distance(transform.position, playerUnit.gameObject.transform.position);
                            }
                        }

                        target = closest_target;
                        state = 1;
                        base.SetDestination(PlayerManager.instance.GetUnit(target).transform.position);
                    }
                }
                else if (Vector3.Distance(transform.position, home) <= 2f)
                {
                    state = 0;
                    target = -1;
                }
            }

            //Animation handling
            if (state == 0)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", false);
            }
            else if (state == 1)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isAttacking", false);
            }
            else if (state == 2)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", true);
                int attackType = UnityEngine.Random.Range(0, 2);
                animator.SetInteger("attackType", attackType);
            }
        }

    }

    private void DealDamage()
    {
        if (PlayerManager.instance.GetUnit(target) == null)
        {
            return;
        }
        //Actually deal the damage
        PlayerUnit p = PlayerManager.instance.GetUnit(target);
        p.RemoveHealth(5);
    }


}


