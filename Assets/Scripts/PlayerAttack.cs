using UnityEngine;
using System.Collections;

public class PlayerAttack : PlayerUnit {

    [SerializeField] public int state; //only serialize field so it can be seen for debugging purposes
    public int target;
    private bool attacking;
    private void Start()
    {
        base.Start();

        //For now, 0 = idle, 1 = moving, 2 = moving towards enemy, 3 = attacking
        //Target will be the ID of the enemy unit it is targetting for either attacking or moving towards, or -1 if it is not targetting anything
        state = 0;
        target = -1;
        attacking = false;
    }

    private void Update()
    {
        base.Update();
        if (state==0) //The following code makes the unit automatically target an enemy unit only if it's within a certain radius and if the player unit idle
        {
            int layer_mask = LayerMask.GetMask("Enemy");
            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 3f, layer_mask);

            if(colliders.Length > 0) //There is an enemy in aggro
            {
                //A target needs to be picked if there are multiple enemies. Pick the nearest enemy
                float closest = Mathf.Infinity;
                int closest_target = -1;
                for (int i = 0; i < colliders.Length; i++)
                {
                    EnemyUnit enemyUnit = colliders[i].GetComponent<EnemyUnit>();
                    if(Vector3.Distance(transform.position, enemyUnit.gameObject.transform.position) < closest)
                    {
                        closest_target = enemyUnit.GetID();
                        closest = Vector3.Distance(transform.position, enemyUnit.gameObject.transform.position);
                    }
                }
                target = closest_target;
                state = 2;
                base.SetDestination(EnemyManager.instance.GetUnit(target).transform.position);
            }
        }
        if (state == 1) //If this unit has been commanded to move somewhere, nothing should break it from trying to get to that location other than it reaching that location
        {
            if(Vector3.Distance(transform.position, base.destination) < 1f)
            {
                state = 0;
            }
        }
        if (state == 2) 
        {
            //First, check if it's target still exists. If not, it should stop targetting

            if (true){ //need to do dictionary checking handling stuff, for now it'll be always true
                EnemyUnit e = EnemyManager.instance.GetUnit(target);

                //Check if either a) the target is close enough that this unit can attack it, or b) this unit should move towrads it

                if (Vector3.Distance(transform.position, base.destination) < 1.5f){
                    state = 3;
                }
                else
                {
                    base.SetDestination(EnemyManager.instance.GetUnit(target).transform.position);
                }

            }
            else
            {
                state = 0;
                target = -1;
            }
        }

        if (state == 3) //This state means this unit is capable of attacking it's target
        {
            //First, check if it's target still exists. If not, it should stop targetting

            if (true) //need to do dictionary checking handling stuff, for now it'll be always true
            {
                base.SetDestination(EnemyManager.instance.GetUnit(target).transform.position);

                if (!attacking)
                {
                    attacking = true;
                    StartCoroutine(DealDamage(target));
                }

                if (Vector3.Distance(transform.position, base.destination) > 1.5f)
                {
                    state = 2;
                }
            }
            else
            {
                state = 0;
                target = -1;
            }
        }
    }

    IEnumerator DealDamage(int target)
    {
        yield return new WaitForSeconds(0.5f); 
        while(state==3)
        {
            //First, enemy unit takes damage
            EnemyUnit e = EnemyManager.instance.GetUnit(target);
            e.RemoveHealth(5);

            //DEBUGGING TO SHOW THAT ATTACKING IS HAPPENING
            Color orig = gameObject.GetComponent<Renderer>().material.color;
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            yield return new WaitForSeconds(0.2f); //flash red for a quick second
            gameObject.GetComponent<Renderer>().material.color = orig;

            yield return new WaitForSeconds(2f); //attack every two seconds
        }
        attacking = false;
    }

    public override void Action1() { //should be obsolete now
        int layer_mask = LayerMask.GetMask("Enemy");
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position,2f, layer_mask);

        for (int i = 0; i < colliders.Length; i++)
        {
            EnemyUnit enemyUnit = colliders[i].GetComponent<EnemyUnit>();

            enemyUnit.RemoveHealth(10);
            Debug.Log("OUCH");
        }
    }

    public override void Action2() {
        Debug.Log("PULLING AGGRO!");
    }

    public override void Action3() {

    }

    public override void Action4() { }
    public override void Action5() { }
}