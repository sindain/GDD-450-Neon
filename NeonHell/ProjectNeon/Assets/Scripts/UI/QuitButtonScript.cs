using UnityEngine;
using System.Collections;

public class QuitButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Transition()
    {
        Application.Quit();
    }
    
}
