/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour
{
    public float maxLife = 1;
    public float life = 1;

    public UnityEvent<float> OnHeal;
    public UnityEvent<float> OnDammage;
    public UnityEvent OnDie;

    void Start()
    {
        maxLife = Mathf.Max(0, maxLife);
        life = Mathf.Clamp(life, 0, maxLife);
    }

    void Heal(float value)
    {
        life += value;
        life = Mathf.Clamp(life, 0, maxLife);
        OnHeal?.Invoke(value);
    }

    void OnHit(float dammage)
    {
        life -= dammage;
        OnDammage?.Invoke(dammage);
        if (life <= 0) {
            life = 0;
            OnDie?.Invoke();
        }
    }
}
