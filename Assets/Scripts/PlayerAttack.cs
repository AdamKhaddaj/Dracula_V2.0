using UnityEngine;

public class PlayerAttack : PlayerUnit {
    public override void Action1() {
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
}