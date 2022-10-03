using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Text _nickText;
    [SerializeField] private Text _scoreText;

    #region Get/Set

    public Player Player { get => _player; }

    public void SetPlayer(Player playerObj)
    {
        _player = playerObj;
    }

    #endregion

    void Update()
    {
        if (!_player)
        {
            Player[] gameObjects = GameObject.FindObjectsOfType<Player>();

            foreach (var player in gameObjects)
            {
                if (player.Nick == _nickText.text || player.Nick == _scoreText.text)
                {
                    SetPlayer(player);
                    break;
                }
            }
        }
        else
        {
            _nickText.text = _player.Nick;
            _scoreText.text = _player.Score.ToString();

            if (_player.Score == InputListener.Instance.WIN_SCORE)
            {
                transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                transform.localScale = new Vector3(3, 3, 3);
            }
        }       
    }
}
