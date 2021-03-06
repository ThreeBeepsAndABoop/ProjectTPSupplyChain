﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class StatusToolCameraInteraction : MonoBehaviour
{

    public FirstPersonController fpsController;
    public GameObject crosshair;

    public Vector3 closedPosition;
    public Quaternion closedRotation;

    public Vector3 openPosition;
    public Quaternion openRotation;

    public float animationDuration;

    public AnimationCurve animationCurve;

    // Goes from closed (0.0f) to open (1.0f)
    [Range(0f, 1f)]
    public float animationProgress;

    public bool isOpen;

    public List<GameObject> statusIndicators;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                GameManager.Instance.RequestPlayPickUpSound();
            }
            else
            {
                GameManager.Instance.RequestPlayDropSound();
            }
            fpsController.enabled = !isOpen;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = isOpen;
            crosshair.SetActive(!isOpen);
        }

        if (isOpen)
        {
            animationProgress = Math.Min(1, animationProgress + (Time.deltaTime / animationDuration));
        } else
        {
            animationProgress = Math.Max(0, animationProgress - (Time.deltaTime / animationDuration));
        }

        transform.localPosition = Vector3.Lerp(closedPosition, openPosition, animationCurve.Evaluate(animationProgress));
        transform.localRotation = Quaternion.Lerp(closedRotation, openRotation, animationCurve.Evaluate(animationProgress));
    }
}
