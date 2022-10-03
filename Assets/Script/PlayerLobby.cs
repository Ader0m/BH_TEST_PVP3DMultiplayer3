using Mirror;
using UnityEngine;

public class PlayerLobby : MonoBehaviour
{
    #region Singltone
    public static PlayerLobby Instance
    {
        get
        {
            return _instance;
        }
    }
    private static PlayerLobby _instance;
    #endregion

    [SerializeField] private NetworkManager _netManager;
    [SerializeField] private string _nick;

    #region Get/Set
    
    public string Nick
    {
        get
        {
            return _nick;
        }
    }

    #endregion

    void Start()
    {
        _instance = this;
    }

    public void SpawnPlayer(string nick)
    {
        if (!string.IsNullOrWhiteSpace(nick))
        {
            if (!_netManager.clientLoadedScene)
            {
                if (!NetworkClient.ready)
                    NetworkClient.Ready();

                _nick = nick;
                CanvasManager.Instance.SpawnGroupToogle();
                CanvasManager.Instance.MatchGroupToogle();
                NetworkClient.AddPlayer();
            }
        }             
    }
}
