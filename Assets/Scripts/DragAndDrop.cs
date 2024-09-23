using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DragAndDrop : MonoBehaviour
{
    private Camera battleCamera;
    private Vector3 offset;
    private int cardId;
    private Transform playerHand;
    private Transform battleArea;

    // Start is called on the frame when a script is enabled
    void Start()
    {
        playerHand = GameObject.Find("Player Hand View").transform.GetChild(0);
        battleCamera = GameObject.Find("Battle Camera").GetComponent<Camera>();
        battleArea = GameObject.Find("Battle Space").transform;
    }

    // OnMouseDown is called when the left mouse button is pressed
    void OnMouseDown()
    {
        if (!gameObject.GetComponent<DragAndDrop>().enabled)
        {
         return;
        }
        // Calculate the offset between the card's position and the mouse position.
        offset = transform.position - battleCamera.ScreenToWorldPoint(Input.mousePosition);
        offset.z = 0; // Prevent changes in the z-coordinate to maintain card's height.

        cardId = gameObject.transform.GetSiblingIndex(); // When a card is clicked, we store its index from the playerHand child list.
        gameObject.transform.SetParent(battleArea); // Move the card to the battle area when it's selected so it dispalys in front of
                                                    // every element in scene when dragged over.
        // Move cardBack on opponent's screen to the battle area so it displays in front of every element in the scene when dragged over.
        NetworkGameManager.Instance.RequestCardBackParentServerRpc(NetworkManager.Singleton.LocalClientId, cardId, false);
    }

    // OnMouseUp is called when the left mouse button is released
    void OnMouseUp()
    {
        if(gameObject.transform.parent == battleArea)
        {
            gameObject.transform.SetParent(playerHand); // Snaps card back to playerHand when card is released.
            // Snaps cardBack on opponent's screen back to opponentHand when card is released.
            NetworkGameManager.Instance.RequestCardBackParentServerRpc(NetworkManager.Singleton.LocalClientId, cardId, true);
        }
        
    }

    // OnMouseDrag is called every frame the left mouse button is held down
    void OnMouseDrag()
    {
        if(gameObject.transform.parent == battleArea)
        {
            // Calculate the new position of the card based on the current mouse position.
            Vector3 newPosition = battleCamera.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = transform.position.z; // Keep the z-coordinate unchanged
            
            transform.position = newPosition + offset;

            // Convert the mouse position to a world position relative to a known reference point.
            // This is used for consistent positioning across different clients in a networked game.
            Vector3 mouseWorldPosition = battleCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, battleCamera.nearClipPlane));
            Vector3 relativePosition = mouseWorldPosition - NetworkGameManager.referencePoint.position; // referencePoint is a Transform at a known fixed position in the world

            // Send updated position data to the server for synchronization across networked clients so that the cardBack's position on the opponent's screen flip/mirrors
            // that of the dragging card on the player's screen.
            NetworkGameManager.Instance.SendCardMovementServerRpc(NetworkManager.Singleton.LocalClientId, relativePosition);
        }
        
    }
}
