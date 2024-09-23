using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class DrawFateCard : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        NetworkGameManager.Instance.RequestRemoveThreeOfAKindServerRpc(NetworkManager.Singleton.LocalClientId);
    }
}
