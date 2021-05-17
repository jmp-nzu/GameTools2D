/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public bool limitAmmo = false;
    public int ammo = 0;
    public float reloadTime = 0;
    public Transform bulletOffset = null;

    float lastFireTime = 0;

    Vector3 offset;
    Vector3 rotation = Vector3.zero;

    void Start()
    {
        offset = transform.localPosition;
        rotation = transform.eulerAngles;
    }

    public void Fire()
    {
        if (Time.time - lastFireTime < reloadTime)
        {
            return;
        }

        if (!limitAmmo || ammo > 0)
        {
            if (bulletPrefab)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                if (bulletOffset)
                {
                    bullet.transform.position = bulletOffset.position;
                }
                else
                {
                    bullet.transform.position = transform.position;
                }

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    rb.velocity = (Vector2)transform.right * bulletSpeed;
                }
            }
            
            if (limitAmmo)
            {
                ammo--;
            }

            lastFireTime = Time.time;
        }
    }

    void OnFire()
    {
        Fire();
    }

    public void AddAmmo(int count)
    {
        ammo += count;
        if (ammo < 0)
        {
            ammo = 0;
        }
    }
}
