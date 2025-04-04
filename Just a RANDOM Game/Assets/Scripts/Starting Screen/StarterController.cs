using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StarterController : MonoBehaviour
{
    private bool check = false;
    private bool logoBool = false;
    private bool creditsBool = false;
    private bool turn = true;
    private Vector2 screenSize = Vector2.zero;

    public Camera cam;
    public Light spotlight;
    public TMP_Text copyright;
    public TMP_Text pressKey;
    public RawImage blackBackground;
    public RawImage pressKeyBackground;
    public RawImage back;
    public RawImage logo;
    public RawImage logoBig;
    public RawImage logoBackground;
    public RawImage credits;
    public RawImage HHLogo;
    public GameObject pink;
    public GameObject blue;
    public Material mat;
    public float logoFadeInSpeed = 0.1f;
    public float textFadeSpeed = 0.1f;
    public float fadeOutSpeed = 0.01f;
    public float firstScene = 5f;
    public float secondScene = 5f;
    public float scaleSpeed = 0.5f;

    [Header("Audio")]
    public AudioSource sfx;
    public AudioClip[] audioClip = new AudioClip[3];

    // Start is called before the first frame update
    void Start()
    {
        if (Time.time > 10)
        {
            sfx.clip = audioClip[2];
            gameObject.SetActive(false);
            return;
        }

        pink.transform.localPosition = new Vector3(0, 0, 0);
        blue.transform.localPosition = new Vector3(0, 0, 0);
        pink.SetActive(false);
        blue.SetActive(false);
        logoBig.enabled = false;
        copyright.enabled = false;
        pressKey.enabled = false;
        pressKeyBackground.enabled = false;
        logoBackground.enabled = true;
        spotlight.intensity = 0.01f;
        credits.enabled = false;
        HHLogo.enabled = false;
        logo.enabled = true;
        back.color = Color.white;
        logoBig.transform.localScale = new Vector3(5.27f, 5.27f, 5.27f);
        var color = logo.color; color.a = 0; logo.color = color;
        color = copyright.color; color.a = 0; copyright.color = pressKey.color = color;
        color = credits.color; color.a = 0; credits.color = color;
        pressKeyBackground.color = new Color(231 / 255f, 227 / 255f, 216 / 255f, 0);
        StartCoroutine(Scene1());
    }

    IEnumerator Scene1()
    {
        yield return new WaitForSeconds(1.5f);
        logoBool = true;
        yield return new WaitForSeconds(2.2f);
        logoBool = false;
        back.color = Color.black;
        yield return new WaitForSeconds(firstScene);
        StartCoroutine(Scene2());
    }

    IEnumerator Scene2()
    {
        credits.enabled = true;
        creditsBool = true;
        yield return new WaitForSeconds(1);
        sfx.Play();
        yield return new WaitForSeconds(1);
        HHLogo.enabled = true;
        yield return new WaitForSeconds(3);
        creditsBool = false;
        yield return new WaitForSeconds(secondScene);

        pressKeyBackground.enabled = true;
        yield return new WaitForSeconds(1.5f);
        sfx.clip = audioClip[0];
        sfx.Play();
        pink.SetActive(true);
        StartCoroutine(MoveOverSeconds(pink, new Vector3(-580, -580, 0), 0.2f));
        yield return new WaitForSeconds(0.5f);
        sfx.Play();
        blue.SetActive(true);
        StartCoroutine(MoveOverSeconds(blue, new Vector3(580, 580, 0), 0.2f));
        yield return new WaitForSeconds(0.5f);
        logoBig.enabled = true;
        sfx.clip = audioClip[1];
        sfx.Play();
        yield return new WaitForSeconds(0.5f);
        pressKey.enabled = true;
        yield return new WaitForSeconds(0.5f);
        copyright.enabled = true;
        sfx.clip = audioClip[2];
        cam.GetComponent<GameStartup>().played = true;
        yield return new WaitForSeconds(1f);
        check = true;
    }

    IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.localPosition;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.localPosition = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.localPosition = end;
    }

    // Update is called once per frame
    void Update()
    {
        if(screenSize != new Vector2 (Screen.width, Screen.height))
        {
            screenSize = new Vector2 (Screen.width, Screen.height);
            blackBackground.rectTransform.sizeDelta = screenSize / GetComponent<Canvas>().scaleFactor;

        }

        Cursor.visible = false;
        if (logoBool && logo.color.a < 1)
        {
            var color = logo.color;
            color.a += Time.deltaTime * logoFadeInSpeed;
            logo.color = color;
        }
        if (!logoBool && logo.color.a > 0)
        {
            var color = logo.color;
            color.a -= Time.deltaTime * fadeOutSpeed;
            logoBackground.color = logo.color = color;
        }

        if(creditsBool && HHLogo.enabled && HHLogo.color.a < 0.8f)
        {
            var color = HHLogo.color;
            color.a += Time.deltaTime * logoFadeInSpeed * 1.2f;
            HHLogo.color = color;
        }

        if(creditsBool && credits.color.a < 0.08)
        {
            var color = credits.color;
            color.a += Time.deltaTime * fadeOutSpeed / 5f;
            credits.color = color;
        }

        if (creditsBool && credits.material != mat && credits.color.a >= 0.08)
        {
            credits.color = new Color(1, 1, 1, 1);
            credits.material = mat;
        }

        if (creditsBool && credits.material == mat && spotlight.intensity<2)
        {
            spotlight.intensity += Time.deltaTime * logoFadeInSpeed / 0.7f;
        }

        if (!creditsBool && credits.material == mat && spotlight.intensity > 0)
        {
            spotlight.intensity -= Time.deltaTime * fadeOutSpeed * 2;
            var color = credits.color;
            color.a -= Time.deltaTime * fadeOutSpeed;
            credits.color = color;

            color = HHLogo.color;
            color.a -= Time.deltaTime * fadeOutSpeed;
            HHLogo.color = color;
        }

        if (pressKeyBackground.enabled == true && pressKeyBackground.color.a < 1)
        {
            var color = pressKeyBackground.color;
            color.a += Time.deltaTime * logoFadeInSpeed/1.4f;
            pressKeyBackground.color = color;
        }

        if (logoBig.enabled == true && logoBig.transform.localScale.x > 1.25f)
        {
            var scale = logoBig.transform.localScale.x;
            scale -= Time.deltaTime * scaleSpeed;
            logoBig.transform.localScale = new Vector3(scale, scale, scale);
        }

        if (pressKey.enabled == true && pressKey.color.a < 1 && !check)
        {
            var color = pressKey.color;
            color.a += Time.deltaTime * logoFadeInSpeed;
            pressKey.color = color;
        }

        if (copyright.enabled == true && copyright.color.a < 1)
        {
            var color = copyright.color;
            color.a += Time.deltaTime * logoFadeInSpeed;
            copyright.color = color;
        }

        if (check && turn && pressKey.color.a<1)
        {
            var color = pressKey.color;
            color.a += Time.deltaTime * textFadeSpeed*1.5f;
            pressKey.color = color;
        }
        else
        {
            turn = false;
        }
        if (check && !turn && pressKey.color.a > 0.3)
        {
            var color = pressKey.color;
            color.a -= Time.deltaTime * textFadeSpeed*1.5f;
            pressKey.color = color;
        }
        else
        {
            turn = true;
        }
    }
}
