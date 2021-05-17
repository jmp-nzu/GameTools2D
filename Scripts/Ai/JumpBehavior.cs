/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpBehavior : MonoBehaviour
{
    public float minimumJumpSeconds = 0f;
    public float maximumJumpSeconds = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void SetJumpTime(float minSeconds, float maxSeconds)
    {
        minimumJumpSeconds = Mathf.Min(minSeconds, maxSeconds);
        maximumJumpSeconds = Mathf.Max(minSeconds, maxSeconds);
    }

    public void SetJumpTime(float seconds)
    {
        SetJumpTime(seconds, seconds);
    }

    void Jump()
    {
        SendMessage("OnJump", true, SendMessageOptions.DontRequireReceiver);

        StartCoroutine(JumpStopCoroutine(Random.Range(minimumJumpSeconds, maximumJumpSeconds)));
    }

    IEnumerator JumpStopCoroutine(float time)
    {
        SendMessage("OnJump", false, SendMessageOptions.DontRequireReceiver);

        time = Mathf.Max(time, 0);
        yield return new WaitForSeconds(time);
    }
}
