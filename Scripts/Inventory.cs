using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    Dictionary<string, int> items;

    [System.Serializable]
    public struct InventoryAction
    {
        public string item;
        public int count;
        public bool clearItems;
        public UnityEvent actions;
    }

    public InventoryAction[] actions;

    public void AddItem(string type)
    {
        AddItems(type, 1);
    }

    public void AddItems(string type, int count)
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
}
