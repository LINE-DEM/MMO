using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 class EquipManager :Singleton<EquipManager> {

    public delegate void OnEquipChangeHandler();

    public event OnEquipChangeHandler OnEquipChanged;

    public Item[] Equips = new Item[(int)EquipSlot.SlotMax];

    byte[] Data;

    //******************************初始化只获得字节数据*************************
    unsafe public void Init(byte[] data)
    {
        this.Data = data;
        this.ParseEquipData(data);
    }

    public bool Contains(int equipId)
    {
        for (int i = 0; i < this.Equips.Length; i++)
        {
            if (Equips[i] != null && Equips[i].Id == equipId)
                return true;
        }
        return false;
    }

    public Item GetEquip(EquipSlot slot)
    {
        return Equips[(int)slot];
    }

    //*********************************用data信息  获取Equips*****************************
    unsafe void ParseEquipData(byte[] data)
    {
        fixed(byte* pt = this.Data)
        {
            for (int i = 0; i < this.Equips.Length; i++)
            {
                int itemId = *(int*)(pt + i * sizeof(int));
                if (itemId > 0)
                    Equips[i] = ItemManager.Instance.Items[itemId];
                else
                    Equips[i] = null;

            }
        }
    }

    //传出的信息只需要7个装备ID
    unsafe public byte[] GetEquipData()
    {
        fixed(byte* pt = Data)
        {
            for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
            {
                int* itemid = (int*)(pt + i * sizeof(int));
                if (Equips[i] == null)
                    *itemid = 0;
                else
                    *itemid = Equips[i].Id;
            }
        }
        return this.Data;
    }

    //**************************给装备UI面板调用****************************************************************************
    public void EquipItem(Item equip)
    {
        ItemService.Instance.SendEquipItem(equip, true);
    }

    public void UnEquipItem(Item equip)
    {
        ItemService.Instance.SendEquipItem(equip, false);
    }


    //***************************ItemService判断调用 穿装备 脱装备**********************************************************
    public void OnEquipItem(Item equip)
    {
        if(this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
        {
            return;
        }
        this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

        if (OnEquipChanged != null)
            OnEquipChanged();
    }

    public void OnUnEquipItem(EquipSlot slot)
    {
        if(this.Equips[(int)slot] != null)
        {
            this.Equips[(int)slot] = null;
            if (OnEquipChanged != null)
                OnEquipChanged();
        }
    }
}
