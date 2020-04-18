using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearTranslation : MonoBehaviour
{
    public Vector3 translationOffset = Vector3.forward * .1f;
    public float translationDuration = 1;
    public AnimationCurve translationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator AnimateForwardTranslation()
    {
        float t = 0.0f;
        Vector3 startingPos = transform.localPosition;
        animating = true;
        movingForward = true;
        //GameManager.Instance.RequestPlayButtonClickSound();
        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale / translationDuration);

            transform.localPosition = Vector3.Lerp(startingPos, startingPos + translationOffset, translationCurve.Evaluate(t)); ;
            yield return 0;
        }

        movingForward = false;
        Vector3 newPos = transform.localPosition;
        t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale / translationDuration);

            transform.localPosition = Vector3.Lerp(newPos, startingPos, translationCurve.Evaluate(t)); ;
            yield return 0;
        }
        animating = false;
    }

    private bool animating;
    private bool movingForward;

    // Update is called once per frame
    void Update()
    {
        if (!animating)
        {
            StartCoroutine(AnimateForwardTranslation());
        }
    }
}
