using UnityEngine;

[CreateAssetMenu(fileName = "New Player Unit Blueprint", menuName = "Player Unit Blueprint", order = 0)]
public class PlayerUnitBlueprint : ScriptableObject {
	public int cost;
	public int levelUpCost;

	public int health;

	public bool movable;
	public int moveSpeed;

	// temporary
	public Color color;
}