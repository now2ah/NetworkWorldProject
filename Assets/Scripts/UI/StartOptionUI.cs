using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartOptionUI : MonoBehaviour
{
    [SerializeField] Button _startServerButton;
    [SerializeField] Button _startClientButton;
    [SerializeField] TMP_InputField _idInputField;

    private void Awake()
    {
        _startServerButton.onClick.AddListener(_OnStartServerButton);
        _startClientButton.onClick.AddListener(_OnStartClientButton);
    }

    void _OnStartServerButton()
    {
        if (_idInputField != null)
        {
            if (_idInputField.text.Length > 0)
            {
                NetworkManager.Singleton.StartServer(_idInputField.text);
            }
            else
            {
                Debug.LogError("Invalid ID");
            }
        }
    }

    void _OnStartClientButton()
    {
        if (_idInputField != null)
        {
            if (_idInputField.text.Length > 0)
            {
                NetworkManager.Singleton.StartClient(_idInputField.text);
            }
            else
            {
                Debug.LogError("Invalid ID");
            }
        }
    }
}
