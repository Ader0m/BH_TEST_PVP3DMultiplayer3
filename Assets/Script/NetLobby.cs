using Mirror;
using System.Collections.Generic;
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
    public static List<string> NickMass = new List<string>();
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

    /// <summary>
    /// ѕри изменении массива _playerNickList выполн€ет перерисовку статов игроков.
    /// ≈сли NickMass не пуст, значит это перезапуск. Ќазначаем подключающимс€ игрокам их имена из статичного массива.
    /// </summary>
    /// <param name="op"></param>
    /// <param name="index"></param>
    /// <param name="oldItem"></param>
    /// <param name="newItem"></param>
    public void RefreshPlayersStatsRedirect(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        if (NickMass.Count > index)
        {
            _playerNickList[index] = NickMass[index];
            Player[] players = GameObject.FindObjectsOfType<Player>();
            foreach (Player p in players) 
            {
                if (string.IsNullOrEmpty(p.Nick))
                {
                    p.SetNameOnReload(NickMass[index]);
                    break;
                }
            }
        }
        CanvasManager.Instance.RefreshPlayersStats();
    }    
}
