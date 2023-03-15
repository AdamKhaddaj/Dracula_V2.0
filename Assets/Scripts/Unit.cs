using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour {
    private static int nextID;

    [SerializeField] private int id;
    private int level;
    [SerializeField] private int health;


    protected void Setup(int health) {
        id = nextID++;
        level = 1;
        this.health = health;
    }

    public int GetID() {
        return id;
    }

    public int GetLevel() {
        return level;
    }

    protected void LevelUp() {
        level++;
    }

    public int GetHealth() {
        return health;
    }

    public void AddHealth(int x) {
        health += x;
    }

    public void RemoveHealth(int x) {
        health = Mathf.Max(health - x, 0);
        if(health == 0)
        {
            if(GetComponent<PlayerUnit>() != null)
            {
                PlayerManager.instance.RemoveUnit(id);
            }
            else
            {
                EnemyManager.instance.RemoveUnit(id);
            }
        }
    }
}