using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class AutoNetworkManager : NetworkManager {

    public event EventHandler OnClientConnectedEvent;
    public event EventHandler OnClientDisconnectedEvent;
    public event EventHandler OnClientSceneChangedEvent;

    public override NetworkClient StartHost()
    {
        return base.StartHost();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");

        if (OnClientConnectedEvent != null)
            OnClientConnectedEvent(this, EventArgs.Empty);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("OnServerDisconnect");

        if (OnClientDisconnectedEvent != null)
            OnClientDisconnectedEvent(this, EventArgs.Empty);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        Debug.Log("OnClientSceneChanged");

        if (OnClientSceneChangedEvent != null)
            OnClientSceneChangedEvent(this, EventArgs.Empty);
    }

}
