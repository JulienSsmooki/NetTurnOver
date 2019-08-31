using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasLobby : MonoBehaviour
{
    public GameObject _panelStart;
    public GameObject _panelSetting;
    public GameObject _panelCommunication;

    private Lobby lobby;

    // Start is called before the first frame update
    void Start()
    {
        lobby = GetComponent<Lobby>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonStart()
    {
        _panelSetting.SetActive(true);
        _panelStart.SetActive(false);
    }

    public void Connect()
    {
        lobby.Connect();
        while(!lobby.IsConnected())
        {
            Debug.Log("Wait connection....");
        }
        Debug.Log("Connection complete !");
        _panelCommunication.SetActive(true);
        _panelSetting.SetActive(false);
    }

    public void SendGetTime()
    {
        lobby.SendGetTime();
    }
}
