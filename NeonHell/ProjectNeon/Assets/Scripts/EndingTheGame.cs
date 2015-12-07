using UnityEngine;
using System.Collections;

public class EndingTheGame : MonoBehaviour {
	public GameObject particle;
	public UIHandler UIH;
	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "Player") {
			//Application.LoadLevel (Application.loadedLevel);
			//UIHandler.lose=true;
			//Destroy (other.gameObject);
			//Instantiate(particle,other.gameObject.transform.position,other.gameObject.transform.rotation);
			UIHandler.lose = true;
			Rotate.isRotate=false;
			UIH.TakeDamage(UIH.currentHealth);
		}
		if (other.gameObject.tag == "Finish") {
			Destroy (other.gameObject);
		}
	}
}
