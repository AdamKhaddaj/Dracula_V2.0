using UnityEngine;

public class PlayerPylon : PlayerUnit {
    public override void Action1() {
        Debug.Log("ATTACK MODE");

        int layer_mask = LayerMask.GetMask("Enemy");
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 4f, layer_mask);

        for (int i = 0; i < colliders.Length; i++)
        {
            EnemyUnit enemyUnit = colliders[i].GetComponent<EnemyUnit>();

            enemyUnit.RemoveHealth(10);
            Debug.Log("OUCH");
        }

    }

    public override void Action2() {

        Debug.Log("HEAL MODE");

        int layer_mask = LayerMask.GetMask("Player");
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 4f, layer_mask);

        for (int i = 0; i < colliders.Length; i++)
        {
            PlayerUnit playerUnit = colliders[i].GetComponent<PlayerUnit>();

            playerUnit.AddHealth(5);
            Debug.Log("THANKS!");
        }

    }

    public override void Action3() {
        if(PlayerManager.instance.GetCrystals() >= 10)
        {
            PlayerManager.instance.RemoveCrystals(10);
            PlayerManager.instance.CreateWarrior(transform.position);
        }
        else
        {
            Debug.Log("YOU ARE BROKE");
        }
    }
}