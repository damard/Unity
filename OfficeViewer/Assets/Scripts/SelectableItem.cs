using UnityEngine;
using System.Collections;

public enum ItemStates
{
	deselected = 0,
	selected = 1
}
public class SelectableItem : MonoBehaviour {

	public ItemStates itemState = ItemStates.deselected;

	private Color deselectedColor;
	private Color selectedColor;

	public Material[] availableColors;

	private int btnOffsetX = 20;
	private int btnOffsetY = 100;
	private int btnSize = 40;
	private int btnPadding = 10;

	// Use this for initialization
	void Start ()
	{
		deselectedColor = new Color(0.0f,0.0f,0.0f,0.0f);
		selectedColor = Color.black;

		//availableColors = new Color[]{Color.blue, Color.red, Color.green, Color.cyan};
	}
	
	// Update is called once per frame
	void OnGUI ()
	{
		switch (itemState)
		{
		case ItemStates.selected:
			renderer.material.SetColor("_OutlineColor", selectedColor);

			for (int i = 0; i<availableColors.Length; i++)
			{
				if (GUI.Button(new Rect (btnOffsetX + i*(btnSize+btnPadding), btnOffsetY, btnSize, btnSize), "Mat"+i))
				{
					renderer.material = availableColors[i];
				}
			}

			break;
		case ItemStates.deselected:
			renderer.material.SetColor ("_OutlineColor", deselectedColor);
			break;
		}
	}

	private Texture GetColorImage (int width, int height, Color color)
	{
		Texture2D result = new Texture2D(width,height);
		Color[] pixels = result.GetPixels();

		for (int i=0; i < pixels.Length; i++)
		{
			pixels[i] = color;
		}
		result.SetPixels(pixels);
		result.Apply();
		return result;
	}
}
