using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MatchContent : MonoBehaviour {
  private UnityEngine.Networking.Match.MatchDesc desc;

  public void setText(UnityEngine.Networking.Match.MatchDesc pDesc){
    transform.GetChild (0).GetComponent<Text> ().text = 
      "Name: " + pDesc.name +
    " Size: " + pDesc.currentSize + '/' + pDesc.maxSize;
    desc = pDesc;
  }
  public UnityEngine.Networking.Match.MatchDesc getDesctription(){
    return desc;
  }

}
