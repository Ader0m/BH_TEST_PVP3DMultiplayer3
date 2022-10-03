using Mirror;


public class CustomNetworkManager : NetworkManager
{
    #region Singleton
    public static CustomNetworkManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private static CustomNetworkManager _instance;
    #endregion

    public override void Start()
    {
        _instance = this;
        base.Start();
    }

    public override void OnClientConnect()
    {
        CanvasManager.Instance.SpawnGroupToogle();   
    }

    public override void OnClientDisconnect()
    {
        PlayerCamera.Instance.SwitchCamera();
        CanvasManager.Instance.MatchGroupToogle();
        
        base.OnClientDisconnect();
    }   
}
