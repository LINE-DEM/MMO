using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class UICharEquip : UIWindow {
	public Text title;
	public Text money;

	public GameObject itemPrefab;
	public GameObject itemEquipPrefab;

	public Transform itemListRoot;

	public List<Transform> slots;

	void Start()
	{
		RefreshUI();
		EquipManager.Instance.OnEquipChanged += RefreshUI;
	}

	private void OnDestroy()
	{
		EquipManager.Instance.OnEquipChanged -= RefreshUI;
	}

	void RefreshUI()
	{
		ClearAllEquipList();
		InitAllEquipItems();
		ClearEquipedList();
		InitEquipedItems();
		this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
	}

	//******************************** 生成左边的列表 **************************
	void InitAllEquipItems()
	{
		foreach (var kv in ItemManager.Instance.Items)
		{
			if(kv.Value.Define.Type == ItemType.Equip)
			{
				// ******************* 如果已经装备上了 *********************
				if (EquipManager.Instance.Contains(kv.Key))
					continue;
				GameObject go = Instantiate(itemPrefab, itemListRoot);
				UIEquipItem ui = go.GetComponent<UIEquipItem>();
				ui.SetEquipItem(kv.Key, kv.Value, this, false);
			}
		}
	}

	//***************************** 删除左边装备 ***************************
	void ClearAllEquipList()
	{
		foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
		{
			Destroy(item.gameObject);
		}
	}

	//****************************** 删除右边的列表 ***************************
	void ClearEquipedList()
	{
		foreach (var item in slots)
		{
			if (item.childCount > 1 )
				Destroy(item.GetChild(1).gameObject);
		}
	}

	//******************** 初始化右边装备 从EquipManager里获取装备对象 **********
	void InitEquipedItems()
	{
		for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
		{
			var item = EquipManager.Instance.Equips[i];
			{
				if(item != null)
				{
					GameObject go = Instantiate(itemEquipPrefab, slots[i]);
					UIEquipItem ui = go.GetComponent<UIEquipItem>();
					ui.SetEquipItem(i, item, this, true);
				}
			}
		}
	}

	//发消息
	public void DoEquip(Item item)
 	{
		EquipManager.Instance.EquipItem(item);
	}

	public void UnEquip(Item item)
	{
		EquipManager.Instance.UnEquipItem(item);
	}


	
}
