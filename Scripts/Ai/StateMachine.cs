/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public int activeState;
    public GameObject[] states;

    // Start is called before the first frame update
    void Start()
    {
        SetState(activeState);
    }

    public void SetState(int state) {
        if (state < 0) {
            state = 0;
        }
        if (state > states.Length - 1) {
            state = states.Length - 1;
        }
        activeState = state;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < states.Length; i++)
        {
            if(i != activeState) {
                states[i].SetActive(false);
            } 
        }
        if (activeState < states.Length) {
            states[activeState].SetActive(true);
        }
    }
}
