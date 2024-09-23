using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlipCardManager : MonoBehaviour
{
    [SerializeField] public Animator animator;
    private bool isDone = false;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

    }
    public IEnumerator FlipCard(GameObject cardToFlip, AudioSource[] audioSources)
    {
        audioSources[1].Play();
        animator = gameObject.GetComponent<Animator>();
        cardToFlip.SetActive(false);
        Debug.Log("our revealed card should be set false: " + cardToFlip.activeInHierarchy);
        animator.SetBool("Flip", true);
        yield return new WaitUntil(() => isDone);
        cardToFlip.SetActive(true);
        Debug.Log("our revealed card should be set true: " + cardToFlip.activeInHierarchy);
        Color finalColor = gameObject.GetComponent<SpriteRenderer>().color;
        finalColor.a = 0;
        gameObject.GetComponent<SpriteRenderer>().DOColor(finalColor, 2.1f).OnComplete(() =>
        {
            isDone = false;
            gameObject.SetActive(false);
            finalColor.a = 255;
            gameObject.GetComponent<SpriteRenderer>().DOColor(finalColor, 0.1f);
            Debug.Log("our animation for flip should be set to false: " + gameObject.activeInHierarchy);
        });
        

        //animator.SetBool("Flip", false);
        //animator.SetBool("isDone", true);
    }
    public void DoneFlip()
    {
        isDone = true;
        animator.SetBool("isDone", true);
        animator.SetBool("Flip", false);
    }
    
}
