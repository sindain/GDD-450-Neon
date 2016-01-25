using UnityEngine;
using System.Collections;

public class FinishLineScript : MonoBehaviour {
	public UIHandler UIH;

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "RPlayer") {
			UIH.lap++;
			UIH.HandleLap();
		}
		if (other.gameObject.tag == "Finish") {
			Destroy (other.gameObject);
		}
	}
}
