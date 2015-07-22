using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextController : MonoBehaviour {

	private Text text;

	private enum States {cell, mirror, sheets_0, lock_0, cell_mirror, sheets_1, lock_1, freedom};
	private States myState;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		text.text = "";

		myState = States.cell;
	}
	
	// Update is called once per frame
	void Update () {
		switch (myState) {
		case States.cell : 
			state_cell();
			break;
		case States.mirror :
			state_mirror();
			break;
		case States.sheets_0 :
			state_sheets_0();
			break;
		case States.lock_0 :
			state_lock_0();
			break;
		case States.cell_mirror :
			state_cell_mirror();
			break;
		case States.sheets_1 :
			state_sheets_1();
			break;
		case States.lock_1 :
			state_lock_1();
			break;
		case States.freedom :
			state_freedom();
			break;

		}
	}

	void state_cell () {
		text.text = "You are in a prison cell, and you want to escape. There are some dirty sheets on the bed, " +
					"a mirror on the wall, and the door is locked from the outside.\n\n" +
					"View [S]heets, look at [M]irror or inspect [L]ock?";

		if (Input.GetKeyDown(KeyCode.S)) {
			myState = States.sheets_0;
		}
		if (Input.GetKeyDown(KeyCode.M)) {
			myState = States.mirror;
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			myState = States.lock_0;
		}
	}
	void state_mirror () {
		text.text = "You can't believe you sleep in these things. Surely it's time somebody changed them. " +
			"The pleasures of prison life I guess...\n\n" +
				"[R]eturn to roaming your cell.";
		
		if (Input.GetKeyDown(KeyCode.R)) {
			myState = States.cell;
		}
	}
	void state_sheets_0 () {
		text.text = "You can't believe you sleep in these things. Surely it's time somebody changed them. " +
					"The pleasures of prison life I guess...\n\n" +
					"[R]eturn to roaming your cell.";
		
		if (Input.GetKeyDown(KeyCode.R)) {
			myState = States.cell;
		}
	}
	void state_lock_0 () {
		text.text = "You are in a prison cell, and you want to escape. There are some dirty sheets on the bed, " +
			"a mirror on the wall, and the door is locked from the outside.\n\n" +
				"View [S]heets, look at [M]irror or inspect [L]ock?";
		
		if (Input.GetKeyDown(KeyCode.S)) {
			myState = States.sheets_0;
		}
		if (Input.GetKeyDown(KeyCode.M)) {
			myState = States.mirror;
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			myState = States.lock_0;
		}
	}
	void state_cell_mirror () {
		text.text = "You are in a prison cell, and you want to escape. There are some dirty sheets on the bed, " +
			"a mirror on the wall, and the door is locked from the outside.\n\n" +
				"View [S]heets, look at [M]irror or inspect [L]ock?";
		
		if (Input.GetKeyDown(KeyCode.S)) {
			myState = States.sheets_0;
		}
		if (Input.GetKeyDown(KeyCode.M)) {
			myState = States.mirror;
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			myState = States.lock_0;
		}
	}
	void state_sheets_1 () {
		text.text = "You are in a prison cell, and you want to escape. There are some dirty sheets on the bed, " +
			"a mirror on the wall, and the door is locked from the outside.\n\n" +
				"View [S]heets, look at [M]irror or inspect [L]ock?";
		
		if (Input.GetKeyDown(KeyCode.S)) {
			myState = States.sheets_0;
		}
		if (Input.GetKeyDown(KeyCode.M)) {
			myState = States.mirror;
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			myState = States.lock_0;
		}
	}
	void state_lock_1 () {
		text.text = "You are in a prison cell, and you want to escape. There are some dirty sheets on the bed, " +
			"a mirror on the wall, and the door is locked from the outside.\n\n" +
				"View [S]heets, look at [M]irror or inspect [L]ock?";
		
		if (Input.GetKeyDown(KeyCode.S)) {
			myState = States.sheets_0;
		}
		if (Input.GetKeyDown(KeyCode.M)) {
			myState = States.mirror;
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			myState = States.lock_0;
		}
	}
	void state_freedom () {
		text.text = "You are in a prison cell, and you want to escape. There are some dirty sheets on the bed, " +
			"a mirror on the wall, and the door is locked from the outside.\n\n" +
				"View [S]heets, look at [M]irror or inspect [L]ock?";
		
		if (Input.GetKeyDown(KeyCode.S)) {
			myState = States.sheets_0;
		}
		if (Input.GetKeyDown(KeyCode.M)) {
			myState = States.mirror;
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			myState = States.lock_0;
		}
	}
}
