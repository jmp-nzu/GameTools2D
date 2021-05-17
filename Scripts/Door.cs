/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();    
    }

    public void Open()
    {
        animator.SetBool("open", true);
    }

    public void Close()
    {
        animator.SetBool("open", false);
    }

    public void Toggle()
    {
        animator.SetBool("open", !animator.GetBool("open"));
    }
}
