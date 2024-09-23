using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistrationRedirect : MonoBehaviour
{
    // The URL you want to open
    private string url = "https://toxicteddie.com/Registration/";

    // Method to open the URL
    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}
