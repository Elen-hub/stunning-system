using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using Network;

public class NetworkManager : TSingletonMono<NetworkManager>, INetworkRunnerCallbacks
{
    public string SessionName;
    string _name;
    protected NetworkRunner _runner;
    public NetworkRunner Runner => _runner;
    Fusion.Photon.Realtime.AuthenticationValues _authValues;
    protected NetworkMainProcessor _mainProcessor;
    protected NetworkSceneManagerDefault _networkSceneManager;
    public byte[] ProcessingSnapshotData => _mainProcessor.ProcessingSnapshotData;
    protected INetworkObjectPool _objectPool;

    PlayerInput _input;
    public PlayerInput SetInput { set => _input = value; }
    public StageEventSender StageEventSender = new StageEventSender() { EventCode = 0 };
    public StageEventReceiver StageEventReceiver = new StageEventReceiver() { EventCode = 0 };
    public CharacterEventSender CharacterEventSender = new CharacterEventSender() { EventCode = 1 };
    public CharacterEventReceiver CharacterEventReceiver = new CharacterEventReceiver() { EventCode = 1 };
    public PlayerEventSender PlayerEventSender = new PlayerEventSender() { EventCode = 2 };
    public PlayerEventReceiver PlayerEventReceiver = new PlayerEventReceiver() { EventCode = 2 };
    public ObjectEventSender ObjectEventSender = new ObjectEventSender() { EventCode = 3 };
    public ObjectEventReceiver ObjectEventReceiver = new ObjectEventReceiver() { EventCode = 3};
    public ItemEventSender ItemEventSender = new ItemEventSender() { EventCode = 4 };
    public ItemEventReceiver ItemEventReceiver = new ItemEventReceiver() { EventCode = 4 };
    public ActorEventSender ActorEventSender = new ActorEventSender() { EventCode = 5 };
    public ActorEventReceiver ActorEventReceiver = new ActorEventReceiver() { EventCode = 5 };
    public JoinEventSender JoinEventSender = new JoinEventSender() { EventCode = 6 };
    public JoinEventReceiver JoinEventReceiver = new JoinEventReceiver() { EventCode = 6 };
    //public StageEventSender StageEventSender = new StageEventSender() { EventCode = 7 };
    //public StageEventReceiver StageEventReceiver = new StageEventReceiver() { EventCode = 7 };
    protected override void OnInitialize()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        _networkSceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
        _name = SystemInfo.deviceUniqueIdentifier;
        ReliableHelper.Runner = _runner;
        _mainProcessor = new NetworkMainProcessor(new NetworkProcessorConfigure()
        {
#if UNITY_EDITOR
            DebugMode = true,
#else
            DebugMode = false,
#endif
            FalseDataStorageTime = 5f
        });
        _mainProcessor.AddReceiveHandler(StageEventReceiver);
        _mainProcessor.AddReceiveHandler(CharacterEventReceiver);
        _mainProcessor.AddReceiveHandler(PlayerEventReceiver);
        _mainProcessor.AddReceiveHandler(ObjectEventReceiver);
        _mainProcessor.AddReceiveHandler(ItemEventReceiver);
        _mainProcessor.AddReceiveHandler(ActorEventReceiver);
        _mainProcessor.AddReceiveHandler(JoinEventReceiver);
    }
    async public void CreateServer(string sessionName, string ip, ushort port)
    {
        StartGameArgs args = new StartGameArgs()
        {
            SessionName = sessionName,
            GameMode = GameMode.Server,
            Address = NetAddress.CreateFromIpPort(ip, port),
            SceneManager = _networkSceneManager,
            Config = NetworkProjectConfig.Global,
            DisableNATPunchthrough = true,
            ObjectPool = _objectPool,
        };
        args.AuthValues = new Fusion.Photon.Realtime.AuthenticationValues("Server");
        await _runner.StartGame(args);
        _runner.Spawn(Resources.Load<RPCGenerator>("RPCGenerator"));
    }
    public void JoinRoom(string address, ushort port)
    {
        //if (address == null || _name == null)
        //    return;

        if (_authValues != null)
            _authValues = new Fusion.Photon.Realtime.AuthenticationValues(_name);

        NetworkProjectConfig config = NetworkProjectConfig.Global;
        config.PeerMode = NetworkProjectConfig.PeerModes.Single;
        StartGameArgs args = new StartGameArgs()
        {
            SessionName = "ServerTest",
            // Address = NetAddress.CreateFromIpPort(address, port),
            GameMode = GameMode.Client,
            SceneManager = _networkSceneManager,
            Config = config,
            AuthValues = _authValues,
        };
        _runner.StartGame(args);
    }
    public void JoinRoom(string sessionName)
    {
        SessionName = sessionName;
        if (sessionName == null || _name == null)
            return;

        if (_authValues != null)
            _authValues = new Fusion.Photon.Realtime.AuthenticationValues(_name);

        NetworkProjectConfig config = NetworkProjectConfig.Global;
        config.PeerMode = NetworkProjectConfig.PeerModes.Single;
        StartGameArgs args = new StartGameArgs()
        {
            SessionName = sessionName,
            GameMode = GameMode.Client,
            SceneManager = _networkSceneManager,
            Config = config,
            AuthValues = _authValues,
        };
        _runner.StartGame(args);
    }
    #region Fusion Callback
    public virtual void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        Debug.LogError("Joined" + playerRef);
        _mainProcessor.JoinPlayer(playerRef);
#if UNITY_SERVER

#else
        if (playerRef != runner.LocalPlayer)
            return;

        UIManager.Instance.Close(eUIName.ULoginUI);
        PlayerManager.Instance.Me.PlayerID = playerRef;
        PlayerManager.Instance.JoinPlayer(PlayerManager.Instance.Me);
        JoinEventSender.RequestPlayerData();
        PlayerEventSender.RequestJoinPlayer(PlayerManager.Instance.Me);
        JoinEventSender.RequestItemData();
        JoinEventSender.RequestInventoryData();
        JoinEventSender.RequestTileChunk();
        JoinEventSender.RequestObjectChunk();
        StageEventSender.RequestEnvironment();

        CharacterEventSender.RequestSpawnCharacter(PlayerManager.Instance.Me.PlayerCharacterData);
#endif
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        _mainProcessor.LeavePlayer(playerRef);
        PlayerManager.Instance.LeavePlayer(playerRef);
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_input);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef playerRef, NetworkInput input)
    {
        // Debug.LogError("InputMissing");
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.LogError("ConnectServer");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        CameraManager.Instance.SetActor = null;
        Debug.LogError("Disconnect");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        request.Accept();
        Debug.LogError("ConnectRequest   " + runner.UserId);
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError("ConnectFailed");
    }
    public virtual void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogError(shutdownReason);
        Debug.LogError("ShutDown");
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.LogError("SimulationMessage");
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.LogError("SessionListUpdate");
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.LogError("CustomAuthenicationResonse");
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.LogError("SceneLoadStart");
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.LogError("SceneLoadDone");
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }
    // Host는 PlayerRef 매개변수값으로 로컬플레이어를 알 수 있다. 자기 자신은 -1로 표시.
    // Client는 자기 자신일경우 1, 아닐경우 0 (특정 할 수 없음)
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        _mainProcessor.OnReliableDataReceived(player, data.Array);
    }
    public void TEST()
    {
    }
    private void Update()
    {
#if UNITY_SERVER
        if(_runner.IsRunning)
            _mainProcessor.Update();
#else
        if (_runner.IsConnectedToServer)
            _mainProcessor.Update();
#endif
    }
#endregion
}
