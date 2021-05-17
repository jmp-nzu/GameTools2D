/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : UniqueInstance
{
    public string type = "coin";
    public int count = 1;

    [Tooltip("トリガーコライダがオブジェクトに付与されている場合、キャラクターが入った時に自動的に拾うか？")]
    public bool pickupOnEnter = true;

    [Tooltip("アイテムを拾った時にこのオブジェクトを削除するか？")]
    public bool destroyOnPickup = true;

    public UnityEvent OnPickup;

    bool isPickedUp = false;

    public void Pickup(GameObject newOwner)
    {
        if (newOwner)
        {
            Inventory inventory = newOwner.GetComponent<Inventory>();
            Pickup(inventory);
        }
    }

    public void Pickup(Inventory inventory)
    {
        if (inventory)
        {
            inventory.AddItems(type, count, uid);
            OnPickup?.Invoke();


            if (destroyOnPickup)
            {
                Destroy(gameObject);
            }
        }

        isPickedUp = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pickupOnEnter && !isPickedUp)
        {
            Pickup(collision.gameObject);
        }
    }

}
