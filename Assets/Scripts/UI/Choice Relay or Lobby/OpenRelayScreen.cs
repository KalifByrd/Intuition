using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenRelayScreen : MonoBehaviour
{
    private Button button;
    private Transform relayParent;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
        relayParent = GameObject.Find("Relay").transform;
    }

    private void OnButtonClicked()
    {
        transform.parent.gameObject.SetActive(false);
        if(!relayParent.GetChild(0).gameObject.activeInHierarchy)
        {
            relayParent.GetChild(0).gameObject.SetActive(true);
        }
    }
}
