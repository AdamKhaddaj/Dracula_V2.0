using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuard : EnemyUnit {

    private int type, state, target;

    //Type will be determined upon creation, but can sometimes change. In general, it will determine the behaviour of the guard (who they target, etc.)

    //0 = defender. They stand still until any unit gets within range, and then they fight, but retreat if the units go too far
    //1 = hunter. They seek specific units to kill

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

        //For now, 0 = idle, 1 = moving, 2 = moving towards enemy, 3 = attacking
        //Target will be the ID of the enemy unit it is targetting for either attacking or moving towards, or -1 if it is not targetting anything
        state = 0;
        target = -1;

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
        if (type == 0)
        {
            if(state == 0)
            {
                base.agent.isStopped = true;
                rigidbody.velocity = Vector3.zero;

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

        else if(type == 1) //if its a hunting type unit
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


