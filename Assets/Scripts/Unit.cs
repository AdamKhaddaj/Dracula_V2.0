using UnityEngine;

public abstract class Unit : MonoBehaviour {
    private static int nextID;

    private int id;
    private int level;
    private int health;

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
    }
}