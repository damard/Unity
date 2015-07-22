using UnityEngine;
using System.Collections;

public class ItemSelector : MonoBehaviour {

	private SelectableItem[] selectableItems;
	private Vector3 mouseDownPos;
	private float distanceThreshold = 10;

	void Start ()
	{
		selectableItems = FindObjectsOfType(typeof(SelectableItem)) as SelectableItem[];
	}
	
	void Update () 
	{
		if (Input.GetMouseButtonDown (0))
		{
			mouseDownPos = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp(0) && Vector3.Distance(mouseDownPos, Input.mousePosition) < distanceThreshold)
		{
			//Debug.Log("Raycasting");
			foreach (SelectableItem si in selectableItems)
			{
				si.itemState = ItemStates.deselected;
			}
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				//Debug.Log("Raycast hit");
				if (hit.transform.GetComponent<SelectableItem>() != null)
					hit.transform.GetComponent<SelectableItem>().itemState = ItemStates.selected;
			}
		}
	}
}
