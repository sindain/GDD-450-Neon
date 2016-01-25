using UnityEngine;
using System.Collections;

public class TitleMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

  // Update is called once per frame
  void Update() {

  }

  public void onSinglePlayerClicked() {
    transform.parent.GetComponent<MenuScript>().setCameraTarget(GameObject.Find("Bays").transform.GetChild(0).transform.FindChild("CameraPosition").transform);
    transform.parent.FindChild("VehicleSelection").gameObject.SetActive(true);
    //Update camera target transform
    gameObject.SetActive(false);

  }
  public void onMultiPlayerClicked() {
  }
  public void onControlsClicked() {
    transform.parent.FindChild("Controls").gameObject.SetActive(true);
    gameObject.SetActive(false);
  }
  public void onCreditsClicked() {
    transform.parent.FindChild("Controls").gameObject.SetActive(true);
    gameObject.SetActive(false);
  }
  public void onExitClicked() {
    Application.Quit();
  }
}
