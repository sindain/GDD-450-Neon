using UnityEngine;
using System.Collections;

public class DificultyChange : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnEasyClicked(){
		PlayerController.DificultyMod = -10f;
	}
	public void OnNormalClicked(){
		PlayerController.DificultyMod = 7f;
	}
	public void OnHardClicked(){
		PlayerController.DificultyMod = 17f;
	}
}
