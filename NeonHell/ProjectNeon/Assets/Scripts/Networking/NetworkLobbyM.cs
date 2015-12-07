using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkLobbyM : NetworkLobbyManager
{
    public int connections;
    void Start()
    {
        connections = 0;
        PlayerPrefs.SetFloat("multi", 1);
    }
    void Update()
    {
    }
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        return true;
    }
    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    { 
        playerPrefab = spawnPrefabs[gameObject.GetComponent<CharacterSelectArray>().shipSelected[connections]];
        gamePlayerPrefab = spawnPrefabs[gameObject.GetComponent<CharacterSelectArray>().shipSelected[connections]];
        connections++;

        return null;
    }
}
/*
 * 100
 * 6
 * 0
 * 0
 * 3.5
 * .3
*/