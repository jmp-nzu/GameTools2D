/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void Fire()
    {
        BroadcastMessage("OnFire");
    }

    public void JumpStart()
    {
        BroadcastMessage("OnJump", true);
    }

    public void JumpEnd()
    {
        BroadcastMessage("OnJump", false);
    }

    public void RunStart()
    {
        BroadcastMessage("OnRun", true);
    }

    public void RunEnd()
    {
        BroadcastMessage("OnRun", false);
    }
}
