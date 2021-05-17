/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBehavior : MonoBehaviour
{
    public Transform target;
    public float minAngleDegrees = -45;
    public float maxAngleDegrees = 45;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /**
     * このメソッドは角度を（0~360）の間の数値に変換します。例えば：
     * -90° -> 270°
     * 380° -> 20°
     * 640° -> 280° 
     */
    float NormalizeAngle(float angle)
    {
        return angle - Mathf.Floor(angle / 360f) * 360f;
    }

    /**
     * このメソッドは2つの角度の差を返します。2つの角度は必ず、時計回り、
     * 反時計回り、2つの距離を計ることができますが、ここではその最も短い
     * 値を返します。
     *
     * aとbは0~360の間でなければなりません。
     */
    float AngleDistance(float a, float b)
    {
        float dist = a - b;
        if (dist > 180)
        {
            return dist - 360;
        }
        else if (dist < -180)
        {
            return dist + 360;
        }
        else
        {
            return dist;
        }
    }

    /**
     * このメソッドは指定の角度をminとmaxの間の値に制限します。ここでいう「間」は
     * min°から反時計回りにmax°までの弧のことです。
     */
    float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);
        min = NormalizeAngle(min);
        max = NormalizeAngle(max);

        float range = NormalizeAngle(AngleDistance(max, min));
        float d1 = AngleDistance(angle, min);

        if (NormalizeAngle(d1) < range) {
            return angle;
        }

        float d2 = AngleDistance(angle, max);

        if (Mathf.Abs(d1) <= Mathf.Abs(d2)) {
            return min;
        }
        else {
            return max;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            
            if (transform.parent != null) {
                direction = transform.parent.InverseTransformDirection(direction.normalized);
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * 180f / Mathf.PI;

            Vector3 eulerAngles = transform.localEulerAngles;
            eulerAngles.z = ClampAngle(angle, minAngleDegrees, maxAngleDegrees);
           
            transform.localEulerAngles = eulerAngles;
        }
    }
}
