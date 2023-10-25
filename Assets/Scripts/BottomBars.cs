using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class BottomBars : MonoBehaviour
{
    private Color orgColor;

    [SerializeField] private float timeToColor;
    [SerializeField] private float minAlpha;
    [SerializeField] private float maxAlpha;

    // Start is called before the first frame update
    void Start()
    {
        orgColor = this.GetComponent<Image>().color;

        StartCoroutine(FadeAlpha(RandomAlpha()));
    }

    private float RandomAlpha()
    {
        return Random.Range(minAlpha, maxAlpha);
    }

    IEnumerator FadeAlpha(float alpha)
    {
        float timeElapsed = 0;
        Color startVal = this.GetComponent<Image>().color;
        while (timeElapsed < timeToColor)
        {
            this.GetComponent<Image>().color = Color.Lerp(startVal, new Color(orgColor.r, orgColor.g, orgColor.b, alpha), timeElapsed / timeToColor);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FadeAlpha(RandomAlpha()));
    }
}
