using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {
  public GameObject returnObject;
	public GameObject returnObject2;
	public GameObject FirstPage;
	public GameObject SecondPage;
	public GameObject ThirdPage;
	public GameObject ForthPage;
	private int pagecounter=0;


  public void clicked() {
    returnObject.SetActive(true);
	returnObject2.SetActive(true);
    gameObject.SetActive(false);
  }
	public void Nxtclicked() {
		pagecounter++;
		if (pagecounter > 3) {
			pagecounter = 0;
		}
		if (pagecounter == 0) {
			FirstPage.SetActive (true);
			SecondPage.SetActive (false);
			ThirdPage.SetActive (false);
			ForthPage.SetActive (false);
		}
		if (pagecounter == 1) {
			FirstPage.SetActive (false);
			SecondPage.SetActive (true);
			ThirdPage.SetActive (false);
			ForthPage.SetActive (false);
		}
		if (pagecounter == 2) {
			FirstPage.SetActive (false);
			SecondPage.SetActive (false);
			ThirdPage.SetActive (true);
			ForthPage.SetActive (false);
		}
		if (pagecounter == 3) {
			FirstPage.SetActive (false);
			SecondPage.SetActive (false);
			ThirdPage.SetActive (false);
			ForthPage.SetActive (true);
		}
	}public void Bckclicked() {
		pagecounter--;
		if (pagecounter < 0) {
			pagecounter = 3;
		}
		if (pagecounter == 0) {
			FirstPage.SetActive (true);
			SecondPage.SetActive (false);
			ThirdPage.SetActive (false);
			ForthPage.SetActive (false);
		}
		if (pagecounter == 1) {
			FirstPage.SetActive (false);
			SecondPage.SetActive (true);
			ThirdPage.SetActive (false);
			ForthPage.SetActive (false);
		}
		if (pagecounter == 2) {
			FirstPage.SetActive (false);
			SecondPage.SetActive (false);
			ThirdPage.SetActive (true);
			ForthPage.SetActive (false);
		}
		if (pagecounter == 3) {
			FirstPage.SetActive (false);
			SecondPage.SetActive (false);
			ThirdPage.SetActive (false);
			ForthPage.SetActive (true);
		}
	}

}
