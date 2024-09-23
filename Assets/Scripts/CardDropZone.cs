using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CardDropZone : MonoBehaviour
{
    //private static bool isRevealed = false;
    private Transform playerDropZone;
    private bool isGood = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Something entered: " + other.gameObject);
        if(other.gameObject.CompareTag("Card"))
        {
            
            Debug.Log(transform.childCount);
            if(transform.childCount < 1)
            {
                Debug.Log("we mad it here hehe");
                isGood = false;
                NetworkGameManager.Instance.RequestBoolIsRevealedServerRpc(false);
                other.gameObject.GetComponent<DragAndDrop>().enabled = false;
                other.gameObject.transform.SetParent(transform);
                NetworkGameManager.Instance.IncrementDropZoneCountServerRpc();
                NetworkGameManager.Instance.RequestCardBackParentDropZoneServerRpc(NetworkManager.Singleton.LocalClientId);
                
            }
            
            
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.transform.parent == transform)
        {
            other.gameObject.GetComponent<DragAndDrop>().enabled = false;
            Debug.Log("dropZoneCount: " + NetworkGameManager.dropZoneCount + ", isRevealed: " + NetworkGameManager.isRevealed + ", isGood: " + isGood );
            if(NetworkGameManager.dropZoneCount == 2 && !NetworkGameManager.isRevealed && !isGood)
            {
                isGood = true;
                StartCoroutine(CheckOpponentCardName(other.gameObject));
                
                
            }
        }
    }
    IEnumerator CheckOpponentCardName(GameObject cardObject)
    {
        Debug.Log("Request Opponent Revealed Card name");
        NetworkGameManager.Instance.RequestOpponentCardNameServerRpc(NetworkManager.Singleton.LocalClientId);
        yield return new WaitForSeconds(0.5f);
        Debug.Log("opponentRevealedCardName: " + NetworkGameManager.opponentRevealedCardName);
        if(NetworkGameManager.opponentRevealedCardName != null)
        {
            
            NetworkGameManager.Instance.RequestCardRevealServerRpc(NetworkManager.Singleton.LocalClientId, cardObject.GetComponent<Card>().cardName, NetworkGameManager.opponentRevealedCardName);
            yield return new WaitForSeconds(5.5f);
            NetworkGameManager.Instance.RequestCardBattleServerRpc(NetworkManager.Singleton.LocalClientId, cardObject.GetComponent<Card>().cardName, NetworkGameManager.opponentRevealedCardName);
            NetworkGameManager.Instance.RequestBoolIsRevealedServerRpc(true);
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        playerDropZone = GameObject.Find("Player Play Card").transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
