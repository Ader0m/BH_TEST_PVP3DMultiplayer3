using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _dash;
    [SyncVar]
    private bool _stopScoreUp;
    [SyncVar]
    [SerializeField] private string _nick;
    [SyncVar]
    [SerializeField] private int _score;
    [SerializeField] private float _maxVelosity;
    [SerializeField] private Material _standart;
    [SerializeField] private Material _light;
    private Rigidbody _rb;
    private DateTime _lastDash;
    private Vector3 _startDashPoint;
    private float _distance;
    private float _mouseY;   
    private float impuls;

    #region Get/Set    

    public bool Dash
    {
        get
        {
            return _dash;
        }
    }

    public Rigidbody rb
    {
        get
        {
            return _rb;
        }

    }

    public string Nick
    {
        get
        {
            return _nick;
        }
    }

    public int Score
    {
        get
        {
            return _score;
        }
    }

    #endregion

    void Start()
    {   
        _rb = GetComponent<Rigidbody>();
        if (isServer)
        {
            _score = 0;
            _stopScoreUp = true;
        }
        if (isLocalPlayer)
        {
            CmdSetDash(false);
            InputListener.Instance.SetPlayer(this);
            CmdSetName(PlayerLobby.Instance.Nick);
            CmdRegisterPlayer();        
            _lastDash = DateTime.Now;             
        }       
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            MoveLogick();
            DashLogick();
            RotateLogick();
            SpeedController();
        } 
    }

    private void RotateLogick()
    {
        _mouseY = Input.GetAxis("Mouse X") * InputListener.Instance.SetSitivity * Time.deltaTime;        

        transform.Rotate(Vector3.up * _mouseY);
    }

    private void MoveLogick()
    {
        _rb.AddForce(               
                transform.forward * 
                InputListener.Instance.MovementVector.z *
                InputListener.Instance.MOVEMENT_SPEED *
                InputListener.Instance.DeltaTime
                );
        _rb.AddForce(
                transform.right *
                InputListener.Instance.MovementVector.x *
                InputListener.Instance.MOVEMENT_SPEED *
                InputListener.Instance.DeltaTime
            );
        _rb.AddForce(
                transform.up *
                InputListener.Instance.MovementVector.y *
                InputListener.Instance.JUMP_FORCE,
                ForceMode.Impulse
            );
    }

    /// <summary>
    /// ¬ычисл€ем рассто€ние при включенной гравитации = 9.81 и силе трени€ 0,6
    /// S = V^2 / (трение * G * н.сила)
    /// I = sqrt(S * 23,8)
    /// 23,8 = (неизвестна€ сила либо умножаетс€ либо суммируетс€ с силой трени€ * G)
    /// </summary>
    private void DashLogick()
    {      
        if ((DateTime.Now - _lastDash).Seconds >= 1 && InputListener.Instance.DashButton != 0)
        {
            impuls = Mathf.Sqrt(InputListener.Instance.DASH_DISTANCE * 23.8f);
            _lastDash = DateTime.Now;
            _startDashPoint = transform.position;
            _distance = 0;

            _rb.AddForce(transform.forward * impuls, ForceMode.Impulse);          
            CmdSetDash(true);
        }
    }   

    /// <summary>
    /// ќграничение скорости игрока. ќтключаетс€ в момент рывка.
    /// ¬ключаетс€ когда проходит необходимое рассто€ние или его скорость падает ниже номинальной
    /// </summary>
    private void SpeedController()
    {
        if (Dash)
        {
            _distance += Vector3.Distance(_startDashPoint, transform.position);
            _startDashPoint = transform.position;
            if (_distance > InputListener.Instance.DASH_DISTANCE || (_rb.velocity.magnitude < _maxVelosity && (DateTime.Now - _lastDash).Seconds >= 1))
            {
                CmdSetDash(false);
            }          
        }
        else
        {
            if (_rb.velocity.magnitude > _maxVelosity && (DateTime.Now - _lastDash).Seconds >= 1)
            {
                _rb.velocity = _rb.velocity.normalized * _maxVelosity;
            }
        }
    }

    /// <summary>
    /// ѕри столкновении свер€ет тег, скорость тела дл€ определени€ рывка.
    /// ѕри успешной проверке запускает корутину дл€ мигани€ и мен€ет тег дл€ игнора физики.
    /// 
    /// »грок который совершил рывок назначает себе счет
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {     
        if (other.gameObject.CompareTag("Player") &&
            other.gameObject.GetComponent<Player>().Dash)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Luminous");
            StartCoroutine(LuminousCor());
        }

        if (other.gameObject.CompareTag("Player") &&
            _dash)
        {
            this.CmdUpScore();
        }
    }

    IEnumerator LuminousCor()
    {
        for (int i = 0; i < InputListener.Instance.LIGHT_TIME; i++)
        {
            this.gameObject.GetComponent<MeshRenderer>().material = _light;
            yield return new WaitForSeconds(0.25f);
            this.gameObject.GetComponent<MeshRenderer>().material = _standart;
            yield return new WaitForSeconds(0.25f);
            this.gameObject.GetComponent<MeshRenderer>().material = _light;
            yield return new WaitForSeconds(0.25f);
            this.gameObject.GetComponent<MeshRenderer>().material = _standart;
            yield return new WaitForSeconds(0.25f);
        }

        this.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    IEnumerator WaitWinBreakeTime()
    {       
        yield return new WaitForSeconds(InputListener.Instance.WinBreakTime);
        NetLobby._restart = true;
    }

    [Command]
    public void CmdSetDash(bool flag)
    {
        _dash = flag;
    }

    [Command]
    public void CmdUpScore()
    {     
        if (_stopScoreUp)
        {
            _score++;

            if (_score == InputListener.Instance.WIN_SCORE)
            {
                _stopScoreUp = false;                
                StartCoroutine(WaitWinBreakeTime());
            }
        }                  
    }

    [Command]
    public void CmdRegisterPlayer()
    {
        NetLobby.Instance.PlayerList.Add(_nick);
    }

    [Command]
    public void CmdDropPlayer()
    {
        NetLobby.Instance.PlayerList.Remove(_nick);
    }

    [Command]
    public void CmdSetName(String nick)
    {
        _nick = nick;
    }
}