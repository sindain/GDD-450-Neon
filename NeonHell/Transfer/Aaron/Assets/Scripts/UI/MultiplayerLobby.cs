using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MultiplayerLobby : MonoBehaviour
{
  public GameObject matchContent;

  private GameObject viewPort;
  private UnityEngine.Networking.Match.MatchDesc[] descriptions;
  private UnityEngine.Networking.Match.MatchDesc SelectedMatch;
  private UnityEngine.Networking.Match.CreateMatchRequest request;

  // Use this for initialization
  void Start (){
    viewPort = transform.FindChild ("Lobbies").gameObject.transform.FindChild ("Viewport").gameObject;
    request = new UnityEngine.Networking.Match.CreateMatchRequest ();
  }
  //End void Start()
	
  //--------------------------------------------------------------------------------------------------------------------
  //Name:     matchResponse
  //Description:  Called once server has returned a list of available matches then displays them in the lobbies viewport
  //Parameters: UnityEngine.Networking.Match.ListmatchResponse matches
  //Returns:    NA
  //--------------------------------------------------------------------------------------------------------------------
  public void matchResponse (UnityEngine.Networking.Match.ListMatchResponse matches){
    descriptions = matches.matches.ToArray ();

    //Remove any children currently present
    for (int i = 0; i < viewPort.transform.childCount; i++)
      Destroy (viewPort.transform.GetChild (i).gameObject);
    
    
    //Populate new list with the match options.
    for (int i = 0; i < descriptions.Length; i++) {
      GameObject content = GameObject.Instantiate (matchContent);
      content.transform.SetParent (viewPort.transform);
      RectTransform rect = content.GetComponent<RectTransform> ();
      rect.offsetMin = new Vector2 (0, -67 - 67 * i);
      rect.offsetMax = new Vector2 (1, -67 * i);
      content.transform.localScale = new Vector3 (1, 1, 1);

      content.GetComponent<MatchContent> ().setText (descriptions [i]);

      content.GetComponent<Button> ().onClick.AddListener (delegate {
        SelectedMatch = content.GetComponent<MatchContent> ().getDesctription ();
      });

    } //End for(int i = 0; i < descriptions.Length; i ++)
  }
  //End public void matchResponse(UnityEngine.Networking.Match.ListMatchResponse matches)

  //--------------------------------------------------------------------------------------------------------------------
  //Name:         getSelectedMatch
  //Description:  Returns data about the match the user selected
  //Parameters:   NA
  //Returns:      UnityEngine.Networking.Match.MatchDesc
  //--------------------------------------------------------------------------------------------------------------------
  public UnityEngine.Networking.Match.MatchDesc getSelectedMatch (){
    return SelectedMatch;
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void changeScreen (bool pbActive){
    transform.FindChild ("Lobbies").gameObject.SetActive (pbActive);
    transform.FindChild ("ButtonPanel").gameObject.SetActive (pbActive);
    transform.FindChild ("TopPanel").gameObject.SetActive (pbActive);
    transform.FindChild ("CreateMatch").gameObject.SetActive (!pbActive);
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void setMatchName (Text pName){
    request.name = pName.text;
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void setMatchPassword (Text pPassword){
    request.password = pPassword.text;
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:
  //Description:
  //Parameters:
  //Returns:
  //--------------------------------------------------------------------------------------------------------------------
  public void onCreateClicked (){
    if (request.name != "")
      GameObject.Find ("GameManager").GetComponent<GameManager> ().StartMatchmakerGame (request);
  }
}
//End public class MultiplayerLobby : MonoBehaviour
