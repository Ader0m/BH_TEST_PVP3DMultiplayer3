using Mirror;
using UnityEngine;


public class PlayerCamera : NetworkBehaviour
{
    #region Singltone
    public static PlayerCamera Instance
    {
        get
        {
            return _instance;
        }
    }
    private static PlayerCamera _instance;
    #endregion

    [SerializeField] public GameObject CameraObj;
    [SerializeField] private Camera _myCamera;
    [SerializeField] private Camera _mainCamera;    
    private float _mouseX;
    private float _mouseYRoration;


    void Start()
    {
        _instance = this;
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (isLocalPlayer)
        {
            _myCamera = CameraObj.AddComponent<Camera>();
            _myCamera.enabled = false;
            SwitchCamera();
        }       
        
    }

    public void SwitchCamera()
    {
        _mainCamera.enabled = !_mainCamera.enabled;
        _myCamera.enabled = !_myCamera.enabled;
    }

    void Update()
    {
        if (isLocalPlayer)
        {

            _mouseX = Input.GetAxis("Mouse Y") * InputListener.Instance.SetSitivity * Time.deltaTime;

            _mouseYRoration -= _mouseX;
            _mouseYRoration = Mathf.Clamp(_mouseYRoration, -45f, 45f);

            _myCamera.transform.localRotation = Quaternion.Euler(_mouseYRoration, 0f, 0f);
        }
    }
}
