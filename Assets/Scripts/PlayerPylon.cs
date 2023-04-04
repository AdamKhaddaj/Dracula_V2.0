using UnityEngine;

public class PlayerPylon : PlayerUnit {

    public PlayerPylon pylonFrom;
    public PlayerPylon pylonTo;

    public PylonModel pylonmodel_prefab;

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
        if (PlayerManager.instance.GetCrystals() >= 10)
        {
            PlayerManager.instance.RemoveCrystals(10);
            PlayerManager.instance.CreateRanger(transform.position);
        }
        else
        {
            Debug.Log("Not Enough Crystals!");
        }
    }

    public override void Action3()
    {
        if (PlayerManager.instance.GetCrystals() >= 10)
        {
            PlayerManager.instance.RemoveCrystals(10);
            PlayerManager.instance.CreateHealer(transform.position);
        }
        else
        {
            Debug.Log("Not Enough Crystals!");
        }
    }

    public override void Action4()
    { //pylon construction

        if (PlayerManager.instance.GetCrystals() >= 50 && PlayerManager.instance.GetStardust() >= 10)
        {

            PylonModel p = Instantiate(pylonmodel_prefab);
            p.SetParentPylon(this);

        }
        else
        {
            Debug.Log("Not Enough Crystals!");
        }

    }

    public override void Action5()
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

}