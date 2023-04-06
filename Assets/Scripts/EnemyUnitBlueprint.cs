using UnityEngine;

[CreateAssetMenu(fileName = "New enemy Unit Blueprint", menuName = "Enemy Unit Blueprint", order = 0)]
public class EnemyUnitBlueprint : ScriptableObject {

	public int health;

	public bool movable;
	public int moveSpeed;

	// temporary
	public Color color;
}