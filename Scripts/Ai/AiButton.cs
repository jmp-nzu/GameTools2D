/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AiButton : AiInput
{
    public float pressTime = 0.5f;
    public float pressTimeVariation = 0.2f;
    public bool sendReleaseMessage = true;

    public bool isPressed = false;

    public UnityEvent OnPress;
    public UnityEvent OnRelease;

    public void Send()
    {
        if (isPressed && OnPress != null)
        {
            OnPress.Invoke();
        }
        if (!isPressed && OnRelease != null)
        {
            OnRelease.Invoke();
        }

        if (isPressed || sendReleaseMessage)
        {
            Send(isPressed);
        }
    }

    public void Toggle()
    {
        isPressed = !isPressed;
        Send();
    }

    public void Press()
    {
        if (pressCoroutine != null)
        {
            StopCoroutine(pressCoroutine);
        }

        pressCoroutine = PressAndRelease();
        StartCoroutine(pressCoroutine);
    }

    public void Press(float duration)
    {
        pressTime = Mathf.Max(0, duration);
        Press();
    }

    public void Set(bool value)
    {
        if (isPressed != value)
        {
            isPressed = value;
            Send();
        }
    }

    IEnumerator pressCoroutine = null;

    IEnumerator PressAndRelease()
    {
        if (isPressed)
        {
            isPressed = false;
            Send();
        }
        isPressed = true;
        Send();

        float waitTime = Random.Range(Mathf.Max(0, pressTime - pressTimeVariation * 0.5f), pressTime + pressTimeVariation * 0.5f);
        yield return new WaitForSeconds(waitTime);

        isPressed = false;
        Send();
    }
}
