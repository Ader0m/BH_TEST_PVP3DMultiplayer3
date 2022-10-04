using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    #region Singleton
    public static CanvasManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private static CanvasManager _instance;
    #endregion

    [SerializeField] private GameObject _spawnGroup;
    [SerializeField] private GameObject _matchGroup;
    [SerializeField] private InputField _nickInputField;
    [SerializeField] private GameObject _playerStats;

    #region Get/Set

    public string GetNick()
    {
        return _nickInputField.text;
    }

    #endregion

    void Start()
    {
        _instance = this;
    }

    void Update()
    {

    }

    public void SpawnGroupToogle()
    {
        _spawnGroup.SetActive(!_spawnGroup.activeSelf);
    }

    public void MatchGroupToogle()
    {
        _matchGroup.SetActive(!_spawnGroup.activeSelf);
    }

    /// <summary>
    /// Чистит канвас от статистики игроков и заполняет ее заново
    /// </summary>
    public void RefreshPlayersStats()
    {             
        int count = 0;


        for (int i = 0; i < _matchGroup.transform.childCount; i++)
        {            
            Destroy(_matchGroup.transform.GetChild(i).gameObject);
        }

        foreach (var name in NetLobby.Instance.PlayerList)
        {           
            GameObject obj = Instantiate(_playerStats, _matchGroup.transform);
            obj.transform.position = new Vector3(obj.transform.position.x + (count * 300), obj.transform.position.y, obj.transform.position.z);
            obj.GetComponentInChildren<Image>().sprite = Game.Instance.ResourceSpriteDict[count];           
            obj.GetComponentInChildren<Text>().text = name;

            count++;
        }
    }
}
