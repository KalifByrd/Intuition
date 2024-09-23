using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance { get; private set; }

    [Header("Cards Settings")]
    [SerializeField] private static GameObject cardBack;
    [SerializeField] private GameObject playerRevealedCard;
    [SerializeField] private GameObject opponentRevealedCard;
    [SerializeField] private GameObject backOfCard;
    [SerializeField] private GameObject starCard1;
    [SerializeField] private GameObject starCard2;
    [SerializeField] private GameObject starCard3;
    [SerializeField] private GameObject heartCard1;
    [SerializeField] private GameObject heartCard2;
    [SerializeField] private GameObject heartCard3;
    [SerializeField] private GameObject moonCard1;
    [SerializeField] private GameObject moonCard2;
    [SerializeField] private GameObject moonCard3;
    [SerializeField] private GameObject fateCard1;
    [SerializeField] private GameObject fateCard2;
    [SerializeField] private GameObject fateCard3;

    [Header("Card Deck Settings")]
    [SerializeField] private List<GameObject> normalDeck = new List<GameObject>();
    [SerializeField] private List<GameObject> fateDeck = new List<GameObject>();

    [SerializeField] private int playerHandCount = 0;
    [SerializeField] private int opponentHandCount = 0;

    // Hands Variables \\
    private Transform playerHand;
    private Transform opponentHand;

    // Drag And Drop Variables \\
    private Camera battleCamera;
    private Transform battleArea;
    private Transform opponentDropZone;
    private Transform playerDropZone;
    public static Transform referencePoint;
    private bool isDragAndDropEnabled = false;
    public static int dropZoneCount;
    public static string opponentRevealedCardName;
    public static bool isRevealed = false;

    private Transform playerWonView;
    private Transform opponentWonView;

    private int sameCardCount = 1;

    private Dictionary<string, List<GameObject>> playerWonCards = new Dictionary<string, List<GameObject>>();
    private List<GameObject> playerWonHearts = new List<GameObject>();
    private List<GameObject> playerWonStars = new List<GameObject>();
    private List<GameObject> playerWonMoons = new List<GameObject>();
    private List<GameObject> playerWonFates = new List<GameObject>();

    private Dictionary<string, List<GameObject>> opponentWonCards = new Dictionary<string, List<GameObject>>();
    private List<GameObject> opponentWonHearts = new List<GameObject>();
    private List<GameObject> opponentWonStars = new List<GameObject>();
    private List<GameObject> opponentWonMoons = new List<GameObject>();
    private List<GameObject> opponentWonFates = new List<GameObject>();

    public bool isThreeOfAKind = false;

    [SerializeField] private GameObject lobbyScreen;

    [SerializeField] private GameObject broadcastObject;
    [SerializeField] private GameObject startScreen;

    public GameObject opponentCardFlip;

    [SerializeField] private AudioSource[] audioSources;



    // Awake is called to initialize variables or states before the application starts
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called on the frame when a script is enabled
    void Start()
    {

        isRevealed = false;
        playerHand = GameObject.Find("Player Hand View").transform.GetChild(0);
        opponentHand = GameObject.Find("Opponent Hand View").transform.GetChild(0);
        referencePoint = GameObject.Find("Battle Space").transform;
        battleCamera = GameObject.Find("Battle Camera").GetComponent<Camera>();
        battleArea = GameObject.Find("Battle Space").transform;
        opponentDropZone = GameObject.Find("Opponent Play Card").transform.GetChild(0);
        playerDropZone = GameObject.Find("Player Play Card").transform.GetChild(0);
        opponentWonView = GameObject.Find("Opponent Won View").transform.GetChild(0);
        playerWonView = GameObject.Find("Player Won View").transform.GetChild(0);
        lobbyScreen = GameObject.Find("Lobby").transform.GetChild(0).gameObject;
        broadcastObject = GameObject.Find("Broadcast").transform.GetChild(0).gameObject;
        startScreen = GameObject.Find("Choice Relay or Lobby");
        opponentCardFlip = GameObject.Find("Opponent Card Flip");
        opponentCardFlip.SetActive(false);

        // Adding all normal cards to the normal card deck
        normalDeck.Add(starCard1);
        normalDeck.Add(starCard2);
        normalDeck.Add(starCard3);
        normalDeck.Add(heartCard1);
        normalDeck.Add(heartCard2);
        normalDeck.Add(heartCard3);
        normalDeck.Add(moonCard1);
        normalDeck.Add(moonCard2);
        normalDeck.Add(moonCard3);

        // Adding all fate cards to the fate card deck
        fateDeck.Add(fateCard1);
        fateDeck.Add(fateCard2);
        fateDeck.Add(fateCard3);

        playerWonCards.Add("Heart", playerWonHearts);
        playerWonCards.Add("Star", playerWonStars);
        playerWonCards.Add("Moon", playerWonMoons);
        playerWonCards.Add("Fate", playerWonFates);

        opponentWonCards.Add("Heart", opponentWonHearts);
        opponentWonCards.Add("Star", opponentWonStars);
        opponentWonCards.Add("Moon", opponentWonMoons);
        opponentWonCards.Add("Fate", opponentWonFates);

        audioSources = gameObject.GetComponents<AudioSource>();

        
    }

    // Server Rpc's
    // *** The Client askes the Server to do something, then the Server executes it *** \\
    [ServerRpc(RequireOwnership = false)]
    public void RequestCardDisplayServerRpc(ulong requesterClientId)
    {
        DisplayCardClientRpc(requesterClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SendCardMovementServerRpc(ulong requesterClientId, Vector3 relativePosition)
    {
        ReceiveCardMovementClientRpc(requesterClientId, relativePosition);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestCardBackParentServerRpc(ulong requesterClientId, int cardId, bool ishand)
    {
        SetCardBackParentClientRpc(requesterClientId, cardId, ishand);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestCardBackParentDropZoneServerRpc(ulong requesterClientId)
    {
        SetCardBackParentDropZoneClientRpc(requesterClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestCardRevealServerRpc(ulong requesterClientId, string playerCardName, string opponentCardName)
    {
        RevealCardClientRpc(requesterClientId, playerCardName, opponentCardName);
    }
    [ServerRpc(RequireOwnership = false)]
    public void IncrementDropZoneCountServerRpc()
    {
        dropZoneCount++;
        UpdateDropZoneCountClientRpc(dropZoneCount);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestUpdateDropZoneCountServerRpc(int count)
    {
        dropZoneCount = count;
        UpdateDropZoneCountClientRpc(dropZoneCount);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestOpponentCardNameServerRpc(ulong requesterClientId)
    {
        GetOpponentCardNameClientRpc(requesterClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void UpdateOpponentCardNameOnAllClientsServerRpc(string cardName)
    {
        UpdateOpponentCardNameOnAllClientsClientRpc(cardName);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestCardBattleServerRpc(ulong requesterClientId, string playerCardName, string opponentCardName)
    {
        BattleCardsClientRpc(requesterClientId, playerCardName, opponentCardName);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestBoolIsRevealedServerRpc(bool condition)
    {
        isRevealed = condition;
        SetBoolIsRevealedClientRpc(condition);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestBroadCastDrawFromFateDeckServerRpc(ulong requesterClientId)
    {
        BroadCastDrawFromFateDeckClientRpc(requesterClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestRemoveThreeOfAKindServerRpc(ulong requesterClientId)
    {
        RemoveThreeOfAKindClientRpc(requesterClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestBroadCastGameResultServerRpc(ulong requesterClientId)
    {
        BroadCastGameResultClientRpc(requesterClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestLobbyScreenSetActiveServerRpc(ulong requesterClientId, bool condition)
    {
        LobbyScreenSetActiveClientRpc(requesterClientId, condition);
    }
    
    // Client Rpc's
    // *** The Server orders the Clients to do something *** \\
    [ClientRpc]
    private void DisplayCardClientRpc(ulong targetClientId)
    {  
        if (NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            if(playerHandCount < 3)
            {
                audioSources[0].Play();
                int randomIndex = Random.Range(0, normalDeck.Count);
                GameObject drawnCard = normalDeck[randomIndex];
                Instantiate(drawnCard, playerHand);
                playerHandCount++;
                EnableDragAndDrop();
            }
        }
        else
        {
            if(opponentHandCount < 3)
            {
                audioSources[0].Play();
                Instantiate(backOfCard, opponentHand);
                opponentHandCount++;
            }
        }
    }
    [ClientRpc]
    private void ReceiveCardMovementClientRpc(ulong targetClientId, Vector3 relativePosition)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
        {
            if (cardBack != null)
            {
                Vector3 flippedPosition = new Vector3(-relativePosition.x, relativePosition.y, -relativePosition.z);
                Vector3 worldPosition = referencePoint.position + flippedPosition;

                cardBack.transform.position = worldPosition;
            }
        }
    }
    [ClientRpc]
    private void SetCardBackParentClientRpc(ulong targetClientId, int cardId, bool ishand)
    {
        if(NetworkManager.Singleton.LocalClientId != targetClientId)
        {
            if(ishand)
            {
                cardBack.transform.SetParent(opponentHand);
            }
            else
            {
                cardBack = opponentHand.GetChild(cardId).gameObject;
                cardBack.transform.SetParent(battleArea);
            }
        }
    }
    [ClientRpc]
    private void SetCardBackParentDropZoneClientRpc(ulong targetClientId)
    {
        if(NetworkManager.Singleton.LocalClientId != targetClientId)
        {
            cardBack.transform.SetParent(opponentDropZone);
        }
    }
    // private IEnumerator FlipAndInstantiate(FlipCardManager flipManager, GameObject card)
    // {
    //     // Start flipping the card and wait for it to finish
    //     yield return StartCoroutine(flipManager.FlipCard());

    //     // Code to execute after the card flip animation
    //     playerRevealedCard = Instantiate(card, opponentDropZone, false);
    //     playerRevealedCard.GetComponent<DragAndDrop>().enabled = false;
    // }
    [ClientRpc]
    private void RevealCardClientRpc(ulong targetClientId, string playerCardName, string opponentCardName)
    {
        List<GameObject> allCards = new List<GameObject>(normalDeck);
        allCards.AddRange(fateDeck);
        if(NetworkManager.Singleton.LocalClientId != targetClientId)
        {
            
            Destroy(cardBack);
            opponentCardFlip.SetActive(true);
            Debug.Log("(non-local) our animation for flip should be set to true: " + opponentCardFlip.activeInHierarchy);
            foreach(GameObject card in allCards)
            {
                
                if(card.name == playerCardName)
                {
                    
                    //if(opponentDropZone.childCount == 0)
                    //{
                        
                        playerRevealedCard = Instantiate(card, opponentDropZone, false);
                        playerRevealedCard.GetComponent<DragAndDrop>().enabled = false;
                        
                        StartCoroutine(opponentCardFlip.GetComponent<FlipCardManager>().FlipCard(playerRevealedCard, audioSources));

                        //StartCoroutine(FlipAndInstantiate(opponentCardFlip.GetComponent<FlipCardManager>(), card));
                        //opponentCardFlip.SetActive(false);
                        break;
                        
                        
                    //}
                    
                } 
            }
        }
        else
        {
            Destroy(cardBack);
            opponentCardFlip.SetActive(true);
            Debug.Log("(local) our animation for flip should be set to true: " + opponentCardFlip.activeInHierarchy);
            foreach(GameObject card in allCards)
            {
                //Debug.Log("in loop: " + opponentCardName);
                if(card.name == opponentCardName)
                {
                   
                    //if(opponentDropZone.childCount == 0)
                    //{
                        
                        opponentRevealedCard = Instantiate(card, opponentDropZone, false);
                        opponentRevealedCard.GetComponent<DragAndDrop>().enabled = false;
                        
                        StartCoroutine(opponentCardFlip.GetComponent<FlipCardManager>().FlipCard(opponentRevealedCard, audioSources));

                        //StartCoroutine(FlipAndInstantiate(opponentCardFlip.GetComponent<FlipCardManager>(), card));
                        //opponentCardFlip.SetActive(false);
                        break;
                        
                    //}
                    
                }
            }
        }
    }
    [ClientRpc]
    private void UpdateDropZoneCountClientRpc(int newCount)
    {
        dropZoneCount = newCount;
        //Debug.Log("Updated dropZoneCount: " + dropZoneCount);
    }
    [ClientRpc]
    private void GetOpponentCardNameClientRpc(ulong targetClientId) // problem
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
        {
            opponentRevealedCardName = playerDropZone.GetChild(0).gameObject.GetComponent<Card>().cardName;
            Debug.Log("opponentRevealedCardName: " + opponentRevealedCardName);
            UpdateOpponentCardNameOnAllClientsServerRpc(opponentRevealedCardName);
        }
    }
    [ClientRpc]
    private void UpdateOpponentCardNameOnAllClientsClientRpc(string cardName)
    {
        opponentRevealedCardName = cardName;
        Debug.Log("now opponentRevealedCardName: " + opponentRevealedCardName);
    }
    [ClientRpc]
    private void BattleCardsClientRpc(ulong targetClientId, string playerCardName, string opponentCardName)
    {

        if(NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            Debug.Log("(LocalClient)playerCardName: " + playerCardName + ", opponentCardName: " + opponentCardName);
            Debug.Log("(LocalClient) lets compare the cards " + GetCardFromDeck(playerCardName).name + " and " + GetCardFromDeck(opponentCardName).name);
            CompareCards(GetCardFromDeck(playerCardName), GetCardFromDeck(opponentCardName));
            //cardBack = Instantiate(savedCardBack);
            dropZoneCount = 0;
            RequestUpdateDropZoneCountServerRpc(0);
            playerHandCount--;
            opponentHandCount--;
            EnableDragAndDrop();
            
        }
        else
        {
            Debug.Log("(NOT LocalClient)playerCardName: " + opponentCardName + ", opponentCardName: " + playerCardName);
            Debug.Log("(NOT LocalClient) lets compare the cards " + GetCardFromDeck(opponentCardName).name + " and " + GetCardFromDeck(playerCardName).name);
            CompareCards(GetCardFromDeck(opponentCardName), GetCardFromDeck(playerCardName));
            //cardBack = Instantiate(savedCardBack);
            dropZoneCount = 0;
            RequestUpdateDropZoneCountServerRpc(0);
            playerHandCount--;
            opponentHandCount--;
            EnableDragAndDrop();
            
        }
    }
    [ClientRpc]
    private void BroadCastGameResultClientRpc(ulong targetClientId)
    {
        if(NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            Debug.Log("You won the game!");
            broadcastObject.GetComponent<TMP_Text>().text = "You won the game!";
            broadcastObject.GetComponent<TweenText>().AnimateText();
            NetworkManager.Shutdown();
            startScreen.SetActive(true);
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBPLAYER
            Application.OpenURL("some url such as your homepage");
            #else
            Application.Quit();
            #endif
        }
        else
        {
            Debug.Log("You Lost the game :( ");
            broadcastObject.GetComponent<TMP_Text>().text = "You Lost the game!";
            broadcastObject.GetComponent<TweenText>().AnimateText();
            NetworkManager.Shutdown();
            startScreen.SetActive(true);
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBPLAYER
            Application.OpenURL("some url such as your homepage");
            #else
            Application.Quit();
            #endif
        }
    }
    private GameObject GetCardFromDeck(string cardName)
    {
        List<GameObject> allCards = new List<GameObject>(normalDeck);
        allCards.AddRange(fateDeck);
        foreach(GameObject card in allCards)
        {
            if(card.name == cardName)
            {
                return card;
            }
        }
        return null;
    }
    [ClientRpc]
    private void WinConditionClientRpc(ulong targetClientId, string winningCardName)
    {
        GameObject winningCard = GetCardFromDeck(winningCardName);
        
        if(NetworkManager.Singleton.LocalClientId != targetClientId)
        {
            Debug.Log("(Non-Local Client)You lost to: " + winningCardName);
            broadcastObject.GetComponent<TMP_Text>().text = "You lost to: " + winningCardName;
            broadcastObject.GetComponent<TweenText>().AnimateText();
            PopulateWonView(opponentWonView, winningCard, opponentWonCards);
            foreach(Transform child in playerDropZone)
            {
                Debug.Log("(plyr) Lets destroy " + child.gameObject.name);
                Destroy(child.gameObject);
            }
            foreach(Transform child in opponentDropZone)
            {
                Debug.Log("(opp) Lets destroy " + child.gameObject.name);
                Destroy(child.gameObject);
            }
            
            
        }
        else
        {
            Debug.Log("(Local Client)You win with: " + winningCardName);
            broadcastObject.GetComponent<TMP_Text>().text = "You win with: " + winningCardName;
            broadcastObject.GetComponent<TweenText>().AnimateText();
            PopulateWonView(playerWonView, winningCard, playerWonCards);
            foreach(Transform child in opponentDropZone)
            {
                Debug.Log("(opp) Lets destroy " + child.gameObject.name);
                Destroy(child.gameObject);
            }
            foreach(Transform child in playerDropZone)
            {
                Debug.Log("(plyr) Lets destroy " + child.gameObject.name);
                Destroy(child.gameObject);
            }
            
        }
    }
    [ClientRpc]
    private void LoseConditionClientRpc(ulong targetClientId, string winningCardName)
    {
        GameObject winningCard = GetCardFromDeck(winningCardName);
        if(NetworkManager.Singleton.LocalClientId != targetClientId)
        {
            Debug.Log("(Non-Local Client)You win with: " + winningCardName);
            broadcastObject.GetComponent<TMP_Text>().text = "You win with: " + winningCardName;
            broadcastObject.GetComponent<TweenText>().AnimateText();
            PopulateWonView(playerWonView, winningCard, playerWonCards);
            foreach(Transform child in opponentDropZone)
            {
                Debug.Log("(opp) Lets destroy " + child.gameObject.name);
                Destroy(child.gameObject);
            }
            foreach(Transform child in playerDropZone)
            {
                Debug.Log("(plyr) Lets destroy " + child.gameObject.name);
                Destroy(child.gameObject);
            }
            
        }
        else
        {
            Debug.Log("(Local Client)You lost to: " + winningCardName);
            broadcastObject.GetComponent<TMP_Text>().text = "You lost to: " + winningCardName;
            broadcastObject.GetComponent<TweenText>().AnimateText();
            PopulateWonView(opponentWonView, winningCard, opponentWonCards);
            foreach(Transform child in playerDropZone)
            {
                Debug.Log("(plyr) Lets destroy " + child.gameObject.name);
                Destroy(child.gameObject);
            }
            foreach(Transform child in opponentDropZone)
            {
                Debug.Log("(opp) Lets destroy " + child.gameObject.name);
                Destroy(child.gameObject);
            }
            
        }
    }
    [ClientRpc]
    private void TieConditionClientRpc()
    {
        Debug.Log("Tie!");
        broadcastObject.GetComponent<TMP_Text>().text = "Tie!";
        broadcastObject.GetComponent<TweenText>().AnimateText();
        foreach(Transform child in opponentDropZone)
        {
            Debug.Log("(opp) Lets destroy " + child.gameObject.name);
            Destroy(child.gameObject);
        }
        foreach(Transform child in playerDropZone)
        {
            Debug.Log("(plyr) Lets destroy " + child.gameObject.name);
            Destroy(child.gameObject);
        }
    }
    [ClientRpc]
    private void SetBoolIsRevealedClientRpc(bool condition)
    {
        isRevealed = condition;
    }
    [ClientRpc]
    private void BroadCastDrawFromFateDeckClientRpc(ulong targetClientId)
    {
        if(NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            Debug.Log("Draw from the Fate Deck!");
        }
    }
    [ClientRpc]
    private void RemoveThreeOfAKindClientRpc(ulong targetClientId)
    {
        if(NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            foreach(KeyValuePair<string, List<GameObject>> cardStack in playerWonCards)
            {
                if (cardStack.Key == "Fate") continue; // Skip Fate cards
                if(cardStack.Value.Count == 3)
                {
                    isThreeOfAKind = false;
                    List<GameObject> cardsToRemove = new List<GameObject>();
                    foreach(GameObject card in cardStack.Value)
                    {
                        cardsToRemove.Add(card);
                    }
                    foreach(GameObject card in cardsToRemove)
                    {
                        cardStack.Value.Remove(card);
                        Destroy(card);
                    }

                    if(playerHandCount < 3)
                    {
                        int randomIndex = Random.Range(0, fateDeck.Count);
                        GameObject drawnCard = fateDeck[randomIndex];
                        Instantiate(drawnCard, playerHand);
                        playerHandCount++;
                        EnableDragAndDrop();
                    }
                    break;
                }
            }
            
            
            
        }
        else
        {
            foreach(KeyValuePair<string, List<GameObject>> cardStack in opponentWonCards)
            {
                if(cardStack.Value.Count == 3)
                {
                    foreach(GameObject card in cardStack.Value)
                    {
                        Destroy(card);
                    }
                    if(opponentHandCount < 3)
                    {
                        Instantiate(backOfCard, opponentHand);
                        opponentHandCount++;
                    }
                    break;
                }
            }
            
        }
    }
    [ClientRpc]
    private void LobbyScreenSetActiveClientRpc(ulong targetClientId, bool condition)
    {
        if(NetworkManager.Singleton.LocalClientId != targetClientId)
        {
            lobbyScreen.SetActive(condition);
        }
    }
    
    private void CompareCards(GameObject playerCard, GameObject opponentCard)
    {
        Card playerCardComponent = playerCard.GetComponent<Card>();
        Card opponentCardComponent = opponentCard.GetComponent<Card>();
        Debug.Log("We are comparing " + playerCard.name + " and " + opponentCard.name);
        if(playerCardComponent.type == opponentCardComponent.type)
        {
            Debug.Log("the cards are of the same type: " + playerCardComponent.type + " so, we have to compare their numbers.");
            if(playerCardComponent.number > opponentCardComponent.number)
            {
                WinConditionClientRpc(NetworkManager.Singleton.LocalClientId, playerCard.name);
            }
            else if(playerCardComponent.number == opponentCardComponent.number)
            {
                TieConditionClientRpc();
            }
            else{LoseConditionClientRpc(NetworkManager.Singleton.LocalClientId, opponentCard.name);}
        }
        else
        {
            if ((playerCardComponent.type == "Fate" && opponentCardComponent.type != "Fate") ||
                (playerCardComponent.type != "Fate" && opponentCardComponent.type == "Fate"))
            {
                if ((playerCardComponent.type == "Fate" && opponentCardComponent.type != "Star") ||
                    (playerCardComponent.type == "Star" && opponentCardComponent.type == "Fate"))
                {
                    WinConditionClientRpc(NetworkManager.Singleton.LocalClientId, playerCard.name);
                }
                else{LoseConditionClientRpc(NetworkManager.Singleton.LocalClientId, opponentCard.name);}
                
            }
            else
            {
                bool playerWins = false;
                
                if (playerCardComponent.type == "Heart" && opponentCardComponent.type == "Moon")
                {
                    playerWins = true;
                }
                else if (playerCardComponent.type == "Moon" && opponentCardComponent.type == "Star")
                {
                    playerWins = true;
                }
                else if (playerCardComponent.type == "Star" && opponentCardComponent.type == "Heart")
                {
                    playerWins = true;
                }

                if (playerWins)
                {
                    WinConditionClientRpc(NetworkManager.Singleton.LocalClientId, playerCard.name);
                }
                else
                {
                    LoseConditionClientRpc(NetworkManager.Singleton.LocalClientId, opponentCard.name);
                }
            }
        }
    }

    public void EnableDragAndDrop()
    {
        if(!isDragAndDropEnabled && playerHandCount == 3)
        {
            foreach (Transform card in playerHand)
            {
                var dragAndDropScript = card.GetComponent<DragAndDrop>();
                if (dragAndDropScript != null)
                {
                    dragAndDropScript.enabled = true;
                }
            }
            isDragAndDropEnabled = true;
        }
        else
        {
            foreach(Transform card in playerHand)
            {
                var dragAndDropScript = card.GetComponent<DragAndDrop>();
                if(dragAndDropScript != null)
                {
                    dragAndDropScript.enabled = false;
                }
            }
            isDragAndDropEnabled = false;
        }
        
    }
    private void PopulateWonView(Transform cardView, GameObject winningCard, Dictionary<string, List<GameObject>> wonCards)
    {
        bool matchFound = false;
        sameCardCount = 1;
        winningCard = Instantiate(winningCard);
        Debug.Log("Our winning card is: " + winningCard.name);
        Debug.Log("Our card view is: " + cardView.name);
    
        if(cardView.childCount != 0)
        {
            Debug.Log("This player has won some cards!");
            foreach(Transform card in cardView)
            {
                if(card.gameObject.GetComponent<Card>().type == winningCard.GetComponent<Card>().type)
                {
                    matchFound = true;  // Set the flag to true when a match is found
                    //sameCardCount++;
                    Debug.Log("We've won a " + winningCard.GetComponent<Card>().type + " before!");
                    if(winningCard.GetComponent<Card>().type == "Fate"){break;}
                    RecursivePopulateWonViewHelper(winningCard.transform, card, wonCards);
                    // Debug.Log("sameCardCount: " + sameCardCount);
                    // if(sameCardCount > 3)
                    // {
                    //     Debug.Log("We won three " + winningCard.GetComponent<Card>().type + " cards in a row!");
                    //     DeleteMachedCards(cardView, winningCard.GetComponent<Card>().type);
                    //     break;
                    // }
                    break;
                    
                }
               
            }
            if(!matchFound)
            {   
                Debug.Log("We've never won a " + winningCard.GetComponent<Card>().type + " before!");
                winningCard.transform.SetParent(cardView, false);
                wonCards[winningCard.GetComponent<Card>().type].Add(winningCard);
                CheckForAllTypes();
            }
        }
        else
        {
            Debug.Log("This player has never won a card!");
            winningCard.transform.SetParent(cardView, false);
            wonCards[winningCard.GetComponent<Card>().type].Add(winningCard);
            CheckForAllTypes();
        }
        
        
    }
    public void CheckForAllTypes()
    {
        int typeCount = 0;
        foreach(KeyValuePair<string, List<GameObject>> cardStack in playerWonCards)
        {
            if(cardStack.Value.Count > 0)
            {
                typeCount++;
            }
        }
        if(typeCount == 4)
        {
            RequestBroadCastGameResultServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }
    public void CheckForThreeOfAKindNormalDeck()
    {
        foreach(KeyValuePair<string, List<GameObject>> cardStack in playerWonCards)
        {
            if (cardStack.Key == "Fate") continue; // Skip Fate cards
            if(cardStack.Value.Count == 3)
            {
                isThreeOfAKind = true;
                RequestBroadCastDrawFromFateDeckServerRpc(NetworkManager.Singleton.LocalClientId);
                break;
                
            }
            else{isThreeOfAKind = false;}
        }
    }
    
    private void DeleteMachedCards(Transform cardView, string cardType)
    {
        List<Transform> cardsToDelete = new List<Transform>();
        foreach(Transform card in cardView)
        {
            if(card.GetComponent<Card>().type == cardType)
            {
                Debug.Log("ooo " + card.gameObject.name);
                cardsToDelete.Add(card);
            }
        }
        foreach(Transform card in cardsToDelete)
        {
            Debug.Log("We need to delete " + card.gameObject.name);
            Destroy(card.gameObject);
        }
            
    }
    private void RecursivePopulateWonViewHelper(Transform winningCardTransform, Transform sameCard, Dictionary<string, List<GameObject>> wonCards)
    {
        if(sameCard.childCount != 0)
        {
            sameCardCount++;
            Debug.Log("We've won this card more than once before.");
            foreach(Transform card in sameCard)
            {
                if(card.childCount != 0)
                {
                    
                    Debug.Log("There is a card attached to this one");
                    RecursivePopulateWonViewHelper(winningCardTransform, card, wonCards);
                }
                Debug.Log("No more cards attached!");
                sameCardCount++;
                winningCardTransform.SetParent(card, false);
                wonCards[winningCardTransform.gameObject.GetComponent<Card>().type].Add(winningCardTransform.gameObject);
            }
        }
        else
        {
            Debug.Log("We've won this card only once before.");
            winningCardTransform.SetParent(sameCard, false);
            wonCards[winningCardTransform.gameObject.GetComponent<Card>().type].Add(winningCardTransform.gameObject);
        }
        

    }
}
