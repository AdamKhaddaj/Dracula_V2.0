using UnityEngine;

public class PlayerPylon : PlayerUnit {
    public override void Action1() {
        if (PlayerManager.instance.GetCrystals() >= 10)
        {
            PlayerManager.instance.RemoveCrystals(10);
            PlayerManager.instance.CreateWarrior(transform.position);
        }
        else
        {
            Debug.Log("Not Enough Crystals!");
        }
    }

    public override void Action2() {
        Debug.Log("create ranger");
    }

    public override void Action3()
    {
        Debug.Log("create healer");
    }

    public override void Action4()
    {

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

    public override void Action5() { }

}