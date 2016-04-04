using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {
  public GameObject returnObject;
  public void clicked() {
    returnObject.SetActive(true);
    gameObject.SetActive(false);
  }
}
