using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetLobby : NetworkBehaviour
{
    #region Singltone
    public static NetLobby Instance
    {
        get
        {
            return _instance;
        }
    }

    private static NetLobby _instance;
    #endregion

    [SerializeField] private readonly SyncList<string> _playerNickList = new SyncList<string>();
    public static bool _restart;

    #region Get/Set

    public SyncList<string> PlayerList { get => _playerNickList; }

    #endregion

    public void Start()
    {
        _instance = this;
        _playerNickList.Callback += RefreshPlayersStatsRedirect;

    }

    void Update()
    {
        if (_restart && isServer)
        {
            CustomNetworkManager.Instance.ServerChangeScene(SceneManager.GetActiveScene().name);
            _restart = false;
        }
    }

    public void RefreshPlayersStatsRedirect(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        CanvasManager.Instance.RefreshPlayersStats();
    }    
}
