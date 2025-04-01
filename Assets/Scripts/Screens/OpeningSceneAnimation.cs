using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OpeningSceneAnimation : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        StartCoroutine(ChooseLandAnimation());
    }

    private IEnumerator ChooseLandAnimation()
    {
        WaitForSeconds wait = new WaitForSeconds(.5f);
        while (true)
        {
            image.enabled = false;
            yield return wait;
            image.enabled = true;
            yield return wait;
        }
    }
}
