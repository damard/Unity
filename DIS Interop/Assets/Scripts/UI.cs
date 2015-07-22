using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {
	/// <summary>
	/// The width of the user interface.
	/// </summary>
	//[Range(0,1024)]
	//public float uiWidth;
	
	public GUISkin skin;
	private float rightOffset = 0.245f;

	void OnGUI()
	{	
		//UI elements
		GUI.skin = skin;
		GUI.Label(new Rect(Screen.width*(1.0f- rightOffset), 0, Screen.width*(rightOffset), 40), "LEGEND");
		GUI.Box(new Rect(Screen.width*(1.0f- rightOffset), 0, Screen.width*(rightOffset), Screen.height), "TEST TSET TSET TEST");
	}
	
	/// <summary>
	/// Creates a uniformly colored Texture2D
	/// </summary>
	/// <returns>
	/// A new Texture2D of size width x height and of color col.
	/// </returns>
	/// <param name='width'>
	/// Width of the generated texture.
	/// </param>
	/// <param name='height'>
	/// Height of the generated texture.
	/// </param>
	/// <param name='col'>
	/// Color of the generated texture.
	/// </param>
	private Texture2D Texture2DFromColor( int width, int height, Color col)
	{
		Color[] pix = new Color[width * height];
		for (int i=0; i < pix.Length; ++i)
		{
			pix[i] = col;	
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}
}
