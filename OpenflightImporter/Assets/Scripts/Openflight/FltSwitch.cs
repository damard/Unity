using UnityEditor;
using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class FltSwitch : MonoBehaviour {

	public FltSwitchMask[] masks;

	[Range(0,31)]
	public int currentMask;

	void Update()
	{
		UpdateMask ();
	}

	private void UpdateMask()
	{
		if (currentMask < masks.Length && currentMask >= 0)
			for (int i = 0; i < masks[currentMask].states.Length; i++)
				transform.GetChild(i).gameObject.SetActive(masks[currentMask].states[i]);

	}
}
[Serializable]
public class FltSwitchMask
{
	public bool[] states;
	public FltSwitchMask(int count)
	{
		states = new bool[count];
	}
}