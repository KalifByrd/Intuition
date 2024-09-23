using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenText : MonoBehaviour
{
    [SerializeField] private float tweenDuration = 1.5f;
    [SerializeField] private CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        // Ensure the text is initially invisible and inactive
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void AnimateText()
    {
        Debug.Log("lets set the gameObject active");
        gameObject.SetActive(true);

        canvasGroup.DOFade(1, tweenDuration).OnComplete(() =>
        {
            Debug.Log("lets fade out");
            //Tween out (fade out) after a delay
            canvasGroup.DOFade(0, tweenDuration). SetDelay(tweenDuration).OnComplete(() =>
            {
                Debug.Log("Lets set the game object nont-active");
                gameObject.SetActive(false);
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
