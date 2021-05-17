/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : UniqueInstance
{
    public Dictionary<string, int> items = new Dictionary<string, int>();
    public HashSet<string> pickedUpItemIds = new HashSet<string>();

    [System.Serializable]
    public struct InventoryAction
    {
        public string item;
        public int count;
        public bool clearItems;
        public UnityEvent actions;
    }

    public InventoryAction[] actions;

    public void AddItem(string type, string uid)
    {
        AddItems(type, 1, uid);
    }

    public void AddItems(string type, int count, string uid)
    {
        if (count < 0) { count = 0; }

        if (!items.ContainsKey(type))
        {
            items.Add(type, count);
        }
        else
        {
            items[type] += count;
        }

        pickedUpItemIds.Add(uid);

        foreach(InventoryAction action in actions)
        {
            if (action.item == type && items[type] >= action.count)
            {
                action.actions.Invoke();
                if (action.clearItems)
                {
                    ClearItems(type);
                }
            }
        }
    }

    public void ExecuteAllActions()
    {
        foreach (InventoryAction action in actions)
        {
            if (items.ContainsKey(action.item))
            {
                if (items[action.item] > action.count)
                {
                    action.actions.Invoke();
                }
            }
        }
    }

    public void RemoveItems(string type, int count)
    {
        if (count < 0) { count = 0; }

        if (items.ContainsKey(type))
        {
            if (count <= items[type])
            {
                items[type] -= count;
            }
            else
            {
                items[type] = 0;
            }
        }
    }

    public void ClearItems(string type)
    {
        items.Remove(type);
    }

    public int GetItemCount(string type)
    {
        if (items.ContainsKey(type))
        {
            return items[type];
        }
        else
        {
            return 0;
        }
    }

    void Save(GameObject checkPoint)
    {
         SceneControl.singleton.SaveInventoryItems(this);
    }
}
