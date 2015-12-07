using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[NetworkSettings(channel=1,sendInterval=0.2f)]
public class CharacterSelect : NetworkBehaviour {

    public GameObject manager;
    
    [SyncVar]
    public int ship;
    // Use this for initialization
    void Start()
    {
        ship = 0;
        manager = GameObject.FindGameObjectWithTag("lobby");
    }

    [ClientRpc]
    public void RpcSelectCharacter(int ship)
    {
        manager.GetComponent<CharacterSelectArray>().shipSelected[gameObject.GetComponent<NetworkLobbyPlayer>().slot] = ship;
    }

    [Command]
    public void CmdSelectCharacter(int shipS)
    {
        manager.GetComponent<CharacterSelectArray>().shipSelected[gameObject.GetComponent<NetworkLobbyPlayer>().slot] = shipS;
        ship = shipS;
    }
    void Update()
    {
        if (isLocalPlayer)
        {
            RpcSelectCharacter(ship);
            CmdSelectCharacter(ship);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && isLocalPlayer)
        {
            RpcSelectCharacter(0);
            CmdSelectCharacter(0);
            ship = 0;
            manager.GetComponent<CharacterSelectArray>().shipSelected[gameObject.GetComponent<NetworkLobbyPlayer>().slot] = ship;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && isLocalPlayer)
        {
            RpcSelectCharacter(1);
            CmdSelectCharacter(1);
            ship = 1;
            manager.GetComponent<CharacterSelectArray>().shipSelected[gameObject.GetComponent<NetworkLobbyPlayer>().slot] = ship;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && isLocalPlayer)
        {
            RpcSelectCharacter(2);
            CmdSelectCharacter(2);
            ship = 2;
            manager.GetComponent<CharacterSelectArray>().shipSelected[gameObject.GetComponent<NetworkLobbyPlayer>().slot] = ship;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && isLocalPlayer)
        {
            RpcSelectCharacter(3);
            CmdSelectCharacter(3);
            ship = 3;
            manager.GetComponent<CharacterSelectArray>().shipSelected[gameObject.GetComponent<NetworkLobbyPlayer>().slot] = ship;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && isLocalPlayer)
        {
            RpcSelectCharacter(4);
            CmdSelectCharacter(4);
            ship = 4;
            manager.GetComponent<CharacterSelectArray>().shipSelected[gameObject.GetComponent<NetworkLobbyPlayer>().slot] = ship;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && isLocalPlayer)
        {
            RpcSelectCharacter(5);
            CmdSelectCharacter(5);
            ship = 5;
            manager.GetComponent<CharacterSelectArray>().shipSelected[gameObject.GetComponent<NetworkLobbyPlayer>().slot] = ship;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) && isLocalPlayer)
        {
            RpcSelectCharacter(6);
            CmdSelectCharacter(6);
            ship = 6;
            manager.GetComponent<CharacterSelectArray>().shipSelected[gameObject.GetComponent<NetworkLobbyPlayer>().slot] = ship;
        }
    }

}
