using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    Dictionary<string, uint> items;

    public void AddItem(string type)
    {
        AddItems(type, 1);
    }

    public void AddItems(string type, uint count)
    {
        if (!items.ContainsKey(type))
        {
            items.Add(type, count);
        }
        else
        {
            items[type] += count;
        }
    }

    public void RemoveItems(string type, uint count)
    {
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
}
