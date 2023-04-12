using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager instance;

	public int UIstate;

	public Canvas ui;

	//Resource Displays
	public Text crystalDisplay;
	public Text beastbloodDisplay;
	public Text stardustDisplay;

	//Resources
	int crystals, beastblood, stardust;

	private void Awake() {
		instance = this;
	}
	void Start() {
		SetUIstate(0);
		crystals = 0;
		beastblood = 0;
		stardust = 0;
	}


	public void SetUIstate(int state) {
		//set everything except resource displays to be inactive
		ui.transform.Find("WarriorIcons").gameObject.SetActive(false);
		ui.transform.Find("HealerIcons").gameObject.SetActive(false);
		ui.transform.Find("PylonIcons").gameObject.SetActive(false);
		ui.transform.Find("RangerIcons").gameObject.SetActive(false);

		UIstate = state;

	}

	public void UpdateCrystals(int c) {
		crystalDisplay.text = "Crystals: " + c;
	}

	public void UpdateBeastblood(int b) {
		beastbloodDisplay.text = "Beast Blood: " + b;
	}

	public void UpdateStardust(int s) {
		stardustDisplay.text = "Stardust: " + s;
	}

	// Update is called once per frame
	void Update() {


		if (UIstate == 0) {
			//do nothing
		}

		if (UIstate == 1) // Display Unit Spawning Buttons
		{
			ui.transform.Find("PylonIcons").gameObject.SetActive(true);
		}

		if (UIstate == 2) // Display Warrior Abilities
		{
			ui.transform.Find("WarriorIcons").gameObject.SetActive(true);
		}

		if (UIstate == 3) // Display Healer Abilities
		{
			ui.transform.Find("HealerIcons").gameObject.SetActive(true);
		}

		if (UIstate == 4) // Display Ranged Abilities
		{
			ui.transform.Find("RangerIcons").gameObject.SetActive(true);
		}

	}
}