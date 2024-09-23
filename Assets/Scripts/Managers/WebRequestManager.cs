using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPage;
    [SerializeField] private GameObject loginFailMessage;
    [System.Serializable]
    public class LoginResponse
    {
        public bool success;
        public string message;
    }
    
    // Function to start the Login Coroutine
    public void StartLogin(string username, string password)
    {
        StartCoroutine(Login(username, password));
    }

    // Coroutine for handling the login process
    private IEnumerator Login(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post("https://toxicteddie.com/unityLoginHandler.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Response received: " + www.downloadHandler.text);
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

                if (response.success)
                {
                    Debug.Log("Login successful! Message: " + response.message);
                    // Handle successful login, such as transitioning to another scene or displaying user info
                    loginPage.SetActive(false);
                    if(loginFailMessage.activeInHierarchy)
                    {
                        loginFailMessage.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogError("Login failed: " + response.message);
                    // Handle failed login, such as displaying an error message to the user
                    loginFailMessage.SetActive(true);
                }
                string jsonResponse = www.downloadHandler.text;
                UserData userData = ParseUserData(jsonResponse);

                // Use userData here
                Debug.Log("Username: " + userData.username);
                Debug.Log("Score: " + userData.score);
                // Process response data here
                // string responseData = www.downloadHandler.text;
                // Process the response data as needed
                
            }
        }
    }
    private UserData ParseUserData(string json)
    {
        return JsonUtility.FromJson<UserData>(json);
    }
}
