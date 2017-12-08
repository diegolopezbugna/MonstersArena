using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;

public class PlayersCountChangedEventArgs : EventArgs {
    public int PlayersCount;
}

public class MyMsgType {
    public static short PlayersCount = MsgType.Highest + 1;
};

public class PlayersCountMessage : MessageBase
{
    public int playersCount;
}

public class AutoNetworkManager : NetworkManager {

    private const string MATCH_NAME = "default"; // TODO: should be retrieved by a GET request to our backend match making depending on user level & other data

    public event EventHandler<PlayersCountChangedEventArgs> OnPlayersCountChangedEvent;

    public bool IsLan = false;
    public uint MatchSize = 4;

    public int PlayersCount;
    public float LastTimePlayerJoined;

    private MatchInfo currentMatch;

    public class MonsterSelectedMessage : MessageBase {
        public string monsterSelected;
    }

    public void Initialize()
    {
        if (!IsLan)
            StartMatchMaker();
    }

    public void JoinOrCreateGame()
    {
        if (IsLan)
        {
            var c = NetworkManager.singleton.StartHost();
            if (c == null)
                NetworkManager.singleton.StartClient();
            RegisterMessageHandlers();
        }
        else
        {
            matchMaker.ListMatches(0, 10, MATCH_NAME, true, 0, 0, OnInternetMatchList);
        }
    }

    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
            {
                //join the last server (just in case there are two...)
                matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
            }
            else
            {
                Debug.Log("No matches in requested room, create one");
                matchMaker.CreateMatch(MATCH_NAME, MatchSize, true, "", "", "", 0, 0, OnInternetMatchCreate);
            }
        }
        else
        {
            Debug.LogError("Couldn't connect to match maker");
        }
    }

    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Join match succeeded");
            StartClient(matchInfo);
            currentMatch = matchInfo;
            RegisterMessageHandlers();
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }

    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Create match succeeded");
            //NetworkServer.Listen(matchInfo, 9000);  // TODO: is this needed??
            StartHost(matchInfo);
            currentMatch = matchInfo;
            RegisterMessageHandlers();
        }
        else
        {
            Debug.LogError("Create match failed");
        }
    }

    public void DropCurrentConnection()
    {
        Debug.LogFormat("DropCurrentConnection {0}", currentMatch);
        if (currentMatch != null)
            matchMaker.DropConnection(currentMatch.networkId, currentMatch.nodeId, 0, (bool success, string extendedInfo) => {
                Debug.LogFormat("OnInternetMatchDropConnection {0},{1}", success, extendedInfo);
            });
    }

    private void RegisterMessageHandlers()
    {
        client.RegisterHandler(MyMsgType.PlayersCount, OnPlayersCountMessageReceived);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.LogFormat("OnClientSceneChanged (GameManager.Instance.SelectedMonster:{0})", GameManager.Instance.SelectedMonster);
        var monsterSelectedMessage = new MonsterSelectedMessage() { monsterSelected = GameManager.Instance.SelectedMonster.Code };
        ClientScene.AddPlayer(conn, 0, monsterSelectedMessage);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader) 
    {
        var message = extraMessageReader.ReadMessage<MonsterSelectedMessage>();
        Debug.LogFormat("OnServerAddPlayer (connectionId:{0}, monster:{1}", conn.connectionId, message.monsterSelected);

        var startPosition = GameManager.Instance.GetNextStartPosition();
        var player = Instantiate(GameManager.Instance.GetMonsterPrefab(message.monsterSelected), startPosition.position, startPosition.rotation);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect");
        base.OnServerConnect(conn);

        PlayersCount += 1;
        LastTimePlayerJoined = Time.time;
        SendPlayersCount(PlayersCount);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect");
        base.OnServerDisconnect(conn);

        PlayersCount -= 1;
        SendPlayersCount(PlayersCount);
    }

    public void OnPlayersCountMessageReceived(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<PlayersCountMessage>();
        Debug.LogFormat("OnPlayersCountMessageReceived: {0}", msg.playersCount);

        if (OnPlayersCountChangedEvent != null)
            OnPlayersCountChangedEvent(this, new PlayersCountChangedEventArgs() { PlayersCount = msg.playersCount });
    }

    public void SendPlayersCount(int playersCount)
    {
        PlayersCountMessage msg = new PlayersCountMessage();
        msg.playersCount = playersCount;

        NetworkServer.SendToAll(MyMsgType.PlayersCount, msg);
    }


}
