using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CardButton : MonoBehaviour
{
    public Button button;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        NetworkGameManager.Instance.CheckForThreeOfAKindNormalDeck();
        if(!NetworkGameManager.Instance.isThreeOfAKind)
        {
            NetworkGameManager.Instance.RequestCardDisplayServerRpc(NetworkManager.Singleton.LocalClientId);
            
        }
        
    }
    
}
