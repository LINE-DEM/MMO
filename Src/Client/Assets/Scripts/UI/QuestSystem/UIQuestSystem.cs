using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///			任务面板主UI
/// </summary>
public class UIQuestSystem : UIWindow {

	public Text title;

	public GameObject itemPrefab;

	
	public ListView listMain;
	public ListView listBranch;

	public UIQuestInfo questInfo;

	

	void Start()
	{
		//在这注册好 给这里面的Item赋值的时候 自动调用
		this.listMain.onItemSelected += this.OnQuestSelected;
		this.listBranch.onItemSelected += this.OnQuestSelected;
		
		RefreshUI();
	}
	
	

	void RefreshUI()
	{
		ClearAllQuestList();
		InitAllQuestItems();
	}
	void InitAllQuestItems()
	{
		
		//这里没看懂
		foreach (var kv in QuestManager.Instance.allQuests)
		{
			
				
				if (kv.Value.Info == null)
					continue;

			if(kv.Value.Info.Status == SkillBridge.Message.QuestStatus.InProgress || kv.Value.Info.Status == SkillBridge.Message.QuestStatus.Complated)
			{
				//如果有info才代表这是已经接了的任务 才生成
				GameObject go = Instantiate(itemPrefab,
					kv.Value.Define.Type == QuestType.Main ?
					this.listMain.transform : this.listBranch.transform);
				UIQuestItem ui = go.GetComponent<UIQuestItem>();
				ui.SetQuestInfo(kv.Value);
				if (kv.Value.Define.Type == QuestType.Main)
					this.listMain.AddItem(ui);
				else
					this.listBranch.AddItem(ui);

			}
			
		}

		if (listMain.items.Count > 0)
		{
			OnQuestSelected(listMain.items[0]);
		}
		else if (listBranch.items.Count > 0)
		{
			OnQuestSelected(listBranch.items[0]);
		}
	}

	void ClearAllQuestList()
	{
		this.listMain.RemoveAll();
		this.listBranch.RemoveAll();
	}

	public void OnQuestSelected(ListView.ListViewItem item)
	{
		if (item.owner == this.listMain)
		{
			foreach (var Item in this.listBranch.items)
			{
				if (Item == this.listBranch.SelectedItem)
					Item.Selected = false;
			}

		}
		if (item.owner == this.listBranch)
		{
			foreach (var Item in this.listMain.items)
			{
				if (Item == this.listMain.SelectedItem)
					Item.Selected = false;
			}
		}
		UIQuestItem questItem = item as UIQuestItem;
		this.questInfo.SetQuestInfo(questItem.quest);
	}
}
