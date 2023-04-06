using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoxDrawer {

	//NOTE, this code is mostly borrowed from https://github.com/pickles976/RTS_selection/blob/master/Utils.cs , we should redo it and make it our own eventually

	static Texture2D boxTexture = new Texture2D(1, 1);

	public static Texture2D getRectTexture() {
		boxTexture.SetPixel(0, 0, Color.white);
		boxTexture.Apply();
		return boxTexture;
	}

	public static void makeRect(Rect rect, Color color) {
		GUI.color = color;
		GUI.DrawTexture(rect, getRectTexture());
		GUI.color = Color.white;
	}

	public static void makeRectBorders(Rect rect, float thickness, Color color) {
		//top part
		BoxDrawer.makeRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
		//left part
		BoxDrawer.makeRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
		//right part
		BoxDrawer.makeRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
		//bottom part
		BoxDrawer.makeRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
	}

	public static Rect getRect(Vector3 screenPosition1, Vector3 screenPosition2) {
		// Move origin from bottom left to top left
		screenPosition1.y = Screen.height - screenPosition1.y;
		screenPosition2.y = Screen.height - screenPosition2.y;
		// Calculate corners
		var topLeft = Vector3.Min(screenPosition1, screenPosition2);
		var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
		// Create Rect
		return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
	}
}