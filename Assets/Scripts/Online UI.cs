using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OnlineUI : MonoBehaviour
{
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host");
        gameObject.SetActive(false);
    }
    public void Client()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client");
        gameObject.SetActive(false);
    }

}
