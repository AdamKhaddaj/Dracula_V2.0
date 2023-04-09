using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {
	public Slider slider;
	public Gradient gradient;
	public Image fill;

	public void SetMaxHealth(int health) {
		slider.maxValue = health;
		fill.color = gradient.Evaluate(1f);
	}

	public void SetHealth(int health) {
		slider.value = health;
		fill.color = gradient.Evaluate(slider.normalizedValue);
	}

	private void Start() {
		SetMaxHealth(GetComponentInParent<Unit>().GetMaxHealth());
		SetHealth(GetComponentInParent<Unit>().GetHealth());

	}
	private void Update() {
		transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
		SetHealth(GetComponentInParent<Unit>().GetHealth());
	}

}
