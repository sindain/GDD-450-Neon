using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MultiplayerLobby : MonoBehaviour {

  public GameObject matchContent;
  private GameObject viewPort;
  private RectTransform baseRect;
  // Use this for initialization
  void Start () {
    viewPort = transform.FindChild("Lobbies").gameObject.transform.FindChild ("Viewport").gameObject;
    baseRect = transform.FindChild("Lobbies").GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void matchResponse(UnityEngine.Networking.Match.ListMatchResponse matches) {
    UnityEngine.Networking.Match.MatchDesc[] descriptions = matches.matches.ToArray();

    for(int i = 0; i < descriptions.Length; i ++){
      GameObject content = GameObject.Instantiate(matchContent);
      content.transform.SetParent(viewPort.transform);
      RectTransform rect = content.GetComponent<RectTransform>();
      rect.offsetMin = new Vector2(0,-67 - 67 * i);
      rect.offsetMax = new Vector2(1, -67*i);
      content.transform.localScale = new Vector3(1,1,1);

      content.transform.FindChild("Text").GetComponent<Text>().text = 
        "Name: '" + descriptions[i].name + 
        "' Size: " + descriptions[i].currentSize +'/'+ descriptions[i].maxSize;
    }
    //matches.matches.ToArray()
  }
  public void onMatchClicked(GameObject data) {

  }
}
