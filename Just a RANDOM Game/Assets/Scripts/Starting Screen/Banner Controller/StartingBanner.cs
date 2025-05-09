using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingBanner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text text;
    public float moveDuration = 0.1f; // Duration for the text movement
    public float fadeDuration = 0.1f; // Duration for the fade in/out effect

    public Canvas canvas;
    public AudioSource sfx;

    public void Reverse()
    {
        foreach (var temp in GameObject.FindGameObjectsWithTag("StartingBanner"))
        {
            StartCoroutine(FadeImage(temp.GetComponent<RawImage>(), 0f, fadeDuration));
        }
        foreach (var temp in GameObject.FindGameObjectsWithTag("StartingBannerText"))
        {
            temp.GetComponent<TMP_Text>().color = Color.white;
            StartCoroutine(MoveText(temp.transform, new Vector3(-647f, temp.transform.localPosition.y, temp.transform.localPosition.z), moveDuration));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sfx.Stop();
        sfx.Play();
        Reverse();
        StartCoroutine(FadeImage(GetComponent<RawImage>(), 1f, fadeDuration));
        text.color = new Color(1f, 140f / 255f, 0f);
        StartCoroutine(MoveText(text.transform, new Vector3(-617f, text.transform.localPosition.y, text.transform.localPosition.z), moveDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(FadeImage(GetComponent<RawImage>(), 0f, fadeDuration));
        text.color = Color.white;
        StartCoroutine(MoveText(text.transform, new Vector3(-647f, text.transform.localPosition.y, text.transform.localPosition.z), moveDuration));
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        
    }

    IEnumerator MoveText(Transform targetTransform, Vector3 targetPosition, float duration)
    {
        float timer = 0f;
        Vector3 initialPosition = targetTransform.localPosition;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            targetTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        targetTransform.localPosition = targetPosition;
    }

    IEnumerator FadeImage(RawImage image, float targetAlpha, float duration)
    {
        float timer = 0f;
        Color initialColor = image.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            image.color = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }

        image.color = targetColor;
    }
}
