using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    private Button button;
    private GameObject currentScreen;
    [SerializeField] private GameObject previousScreen;
    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
        currentScreen = transform.parent.gameObject;
    }
    private void OnButtonClicked()
    {
        currentScreen.SetActive(false);
        if(!previousScreen.activeInHierarchy)
        {
            previousScreen.SetActive(true);
        }

    }
    
}
