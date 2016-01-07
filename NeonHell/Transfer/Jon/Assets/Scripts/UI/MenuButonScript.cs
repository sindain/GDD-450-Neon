using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class MenuButonScript : NetworkBehaviour {
	public GameObject LobbyPref;
    public NetworkManager manager;
	// Use this for initialization
	void Start () {
        LobbyPref = GameObject.FindWithTag("lobby");
		if(LobbyPref != null)
        	manager = LobbyPref.GetComponent<NetworkManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Transition(){
		Application.LoadLevel ("MainMenu");
		Time.timeScale =1;
        NetworkServer.Shutdown();
        NetworkClient.ShutdownAll();
		Destroy (GameObject.FindGameObjectsWithTag("lobby")[0]);
        Destroy(GameObject.FindGameObjectsWithTag("lobby")[1]);
        Destroy(GameObject.FindGameObjectsWithTag("lobby")[2]);
        Destroy(GameObject.FindGameObjectsWithTag("lobby")[3]);
        Destroy(GameObject.FindGameObjectsWithTag("lobbyPlayer")[0]);
        Destroy(GameObject.FindGameObjectsWithTag("lobbyPlayer")[1]);
        Destroy(GameObject.FindGameObjectsWithTag("lobbyPlayer")[2]);
		Destroy(GameObject.FindGameObjectsWithTag("lobbyPlayer")[3]);
		if(LobbyPref != null) {
			manager.StopClient ();
			manager.StopAllCoroutines ();
			manager.StopHost ();
			manager.StopMatchMaker ();
			manager.StopServer ();
		}
	}
}
