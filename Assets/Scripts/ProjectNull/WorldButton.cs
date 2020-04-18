using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldButton : MonoBehaviour
{
    public Vector3 pressOffset = Vector3.forward * .1f;
    public float pressDuration = 0.2f;
    public UnityEvent onPress;

    bool pressed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AnimateDepression()
    {
        GameManager.Instance.RequestPlayButtonClickSound();
        float t = 0.0f;
        Vector3 startingPos = transform.localPosition;

        //GameManager.Instance.RequestPlayButtonClickSound();
        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale / pressDuration);

            transform.localPosition = Vector3.Lerp(startingPos, startingPos + pressOffset, t);
            yield return 0;
        }

        onPress.Invoke();

        Vector3 newPos = transform.localPosition;
        t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.timeScale / pressDuration);

            transform.localPosition = Vector3.Lerp(newPos, startingPos, t);
            yield return 0;
        }
        pressed = false;
    }

    public void Press()
    {
        if (pressed)
        {
            return;
        }

        pressed = true;

        StartCoroutine(AnimateDepression());
    }
}
