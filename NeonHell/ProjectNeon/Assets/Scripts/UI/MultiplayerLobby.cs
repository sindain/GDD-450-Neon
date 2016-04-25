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
    if (SelectedMatch == null)
      gameObject.transform.FindChild ("ButtonPanel").gameObject.transform.FindChild ("Join").GetComponent<Button> ().interactable = false;
  } //End void Start()
	
  //--------------------------------------------------------------------------------------------------------------------
  //Name:         matchRequest
  //Description:  Called once the user requests a new list of matches.  Disables multiplayer screen buttons and shows
  //              wait screen.
  //Parameters:   NA
  //Returns:      NA
  //--------------------------------------------------------------------------------------------------------------------
  public void matchRequest(){
    toggleWaitScreen (true, "Loading matches.");
  }

  //--------------------------------------------------------------------------------------------------------------------
  //Name:     matchResponse
  //Description:  Called once server has returned a list of available matches then displays them in the lobbies viewport
  //Parameters: UnityEngine.Networking.Match.ListmatchResponse matches
  //Returns:    NA
  //--------------------------------------------------------------------------------------------------------------------
  public void matchResponse (UnityEngine.Networking.Match.ListMatchResponse matches){
    toggleWaitScreen (false, "");
    if (matches.matches == null)
      return;
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
				print("23");
				transform.FindChild("ButtonPanel").FindChild("Join").GetComponent<JoinButton>().interactable=true;
      });

    } //End for(int i = 0; i < descriptions.Length; i ++)
  } //End public void matchResponse(UnityEngine.Networking.Match.ListMatchResponse matches)

  public void toggleWaitScreen(bool pbTurnOn, string pMessage){
    GameObject btnPanel = gameObject.transform.FindChild ("ButtonPanel").gameObject;
    btnPanel.transform.FindChild ("Host").GetComponent<Button> ().interactable = !pbTurnOn;
    btnPanel.transform.FindChild ("Join").GetComponent<Button> ().interactable = SelectedMatch == null ? false : !pbTurnOn;
    btnPanel.transform.FindChild ("Back").GetComponent<Button> ().interactable = !pbTurnOn;
    gameObject.transform.FindChild("TopPanel").FindChild("RefreshButton").GetComponent<Button>().interactable = !pbTurnOn;
    if(viewPort != null)
      foreach(Transform t in viewPort.transform)
        t.GetComponent<Button> ().interactable = !pbTurnOn;
    

    transform.parent.FindChild ("Wait").gameObject.SetActive (pbTurnOn);
    transform.parent.FindChild ("Wait").gameObject.transform.FindChild ("Message").gameObject.GetComponent<Text>().text = pMessage;
  }

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

  public void onRefreshClicked(){
    toggleWaitScreen (true, "Loading Matches");
    SelectedMatch = null;
  }
}
//End public class MultiplayerLobby : MonoBehaviour
