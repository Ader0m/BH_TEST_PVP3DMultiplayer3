using Mirror;
using UnityEngine;

public class InputListener : NetworkBehaviour
{
    #region Singltone
    public static InputListener Instance
    {
        get
        {
            return _instance;
        }
    }
    private static InputListener _instance;
    #endregion

    [SerializeField] public float SetMovementSpeed;
    [SerializeField] public float SetJumpForce;
    [SerializeField] public float SetDashDistance;
    [SerializeField] public float SetLightTime;
    [SerializeField] public float SetSitivity;
    [SerializeField] public int SetWinScore;
    [SerializeField] public float WinBreakTime;

    [SyncVar]
    public int WIN_SCORE;
    [SyncVar]   
    public float MOVEMENT_SPEED;
    [SyncVar]
    public float JUMP_FORCE;

    /// <summary>
    /// Расстояние в метрах
    /// </summary>
    [SyncVar]
    public float DASH_DISTANCE;

    /// <summary>
    /// Устанавливать в целых секундах
    /// </summary>
    [SyncVar]
    public float LIGHT_TIME;

    private Vector3 _movementVector;
    private Player _player;
    private float _dashButton;
    private float _deltaTime;

    #region Get/Set
    public Vector3 MovementVector
    {
        get
        {
            return _movementVector;
        }
    }
    public float DashButton
    {
        get
        {
            return _dashButton;
        }
    }
    public float DeltaTime
    {
        get
        {
            return _deltaTime;
        }
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }
    #endregion

    void Start()
    {
        _movementVector = new Vector3();
        _instance = this;         
    }
    
    void Update()
    {
        _deltaTime = Time.deltaTime;
        if (_player)
            MoveInput();

        if (isServer)
        {
            MOVEMENT_SPEED = SetMovementSpeed;
            DASH_DISTANCE = SetDashDistance;
            LIGHT_TIME = SetLightTime;
            JUMP_FORCE = SetJumpForce;
            WIN_SCORE = SetWinScore;
        }
    }

    private void MoveInput()
    {
        _movementVector.x = Input.GetAxis("Horizontal");
        _movementVector.y = Input.GetKeyDown("space") ? 1f : 0f;
        _movementVector.z = Input.GetAxis("Vertical");
        _dashButton = Input.GetAxis("Fire1");      
    }

    public void SpawnPlayer()
    {
        PlayerLobby.Instance.SpawnPlayer(CanvasManager.Instance.GetNick());         
    }
}
