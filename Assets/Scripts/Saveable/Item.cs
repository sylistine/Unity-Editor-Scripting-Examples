using UnityEngine;
using UnityEditor;
using System.Collections;
using Aria;

public class Item : ScriptableObject
{
    /* Fields */
    public int itemId;

    [SerializeField]
    private string _itemName;

    [SerializeField]
    private ItemType _itemType = ItemType.Null;

    [SerializeField]
    private Equipment _eqSlot = Equipment.Null;

    [SerializeField]
    private QLvl _qLvl = QLvl.normal;

    [SerializeField]
    private Sprite _itemIcon;

    [SerializeField]
    private string _description;
    
    /* Properties */
    public string itemName
    {
        get { return _itemName; }
        set
        {
            if (_itemName != value)
            {
                _itemName = value;
                EditorUtility.SetDirty(this);
            }
        }
    }

    public ItemType itemType
    {
        get { return _itemType; }
        set
        {
            if(_itemType != value)
            {
                _itemType = value;
                EditorUtility.SetDirty(this);
            }
        }
    }

    public Equipment eqSlot
    {
        get { return _eqSlot; }
        set
        {
            if (_eqSlot != value)
            {
                _eqSlot = value;
                EditorUtility.SetDirty(this);
            }
        }
    }

    public QLvl qLvl
    {
        get { return _qLvl; }
        set
        {
            if(_qLvl != value)
            {
                _qLvl = value;
                EditorUtility.SetDirty(this);
            }
        }
    }

    public Sprite itemIcon
    {
        get { return _itemIcon; }
        set
        {
            if (_itemIcon != value)
            {
                _itemIcon = value;
                EditorUtility.SetDirty(this);
            }
        }
    }

    public string description
    {
        get { return _description; }
        set
        {
            if(_description != value)
            {
                _description = value;
                EditorUtility.SetDirty(this);
            }
        }
    }
}
