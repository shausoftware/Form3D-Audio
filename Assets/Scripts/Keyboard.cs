using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour {

	public delegate void ChangeBackground(int backgroundId);
	public static event ChangeBackground changeBackground;
	public delegate void Help();
	public static event Help help;
	public delegate void Reset();
	public static event Reset reset; 
	public delegate void ChangeShape(int shapeId);
	public static event ChangeShape changeShape;

	#region Singleton
	private static Keyboard _instance;
	public static Keyboard Instance;
	#endregion

	void Awake() {
		_instance = this;
	}

	void Update () {

		//Backgrounds
		if (Input.GetKeyDown("1")) {
			UpdateBackground(1);
		}
		if (Input.GetKeyDown("2")) {
			UpdateBackground(2);
		}
		if (Input.GetKeyDown("3")) {
			UpdateBackground(3);
		}
		if (Input.GetKeyDown("4")) {
			UpdateShape(1);
		} 
		if (Input.GetKeyDown("5")) {
			UpdateShape(2);
		} 
		if (Input.GetKeyDown("6")) {
		} 
		if (Input.GetKeyDown("7")) {
		} 

		//Help
		if (Input.GetKeyDown("h")) {
			if (help != null) {
				help();
			}
		}

		//Reset
		if (Input.GetKeyDown("r")) {
			if (reset != null) {
				reset();
			}
		}
	}

	private void UpdateBackground(int id) {
		if (changeBackground != null) {
			changeBackground(id);
		}
	}

	private void UpdateShape(int id) {
		if (changeShape != null) {
			changeShape(id);
		}
	}
}
