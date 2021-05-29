using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

/// <summary>
///			任务界面左边的格子 （继承LIstViewITem）主要是有被选中的处理
///			保存了一个Quest模板
///			也是一个Set方法 
/// </summary>
public class UIQuestItem : ListView.ListViewItem {
	public Text title;

	public Image background;
	public Sprite normalBg;
	public Sprite selectedBg;

	public override void onSelected(bool selected)
	{
		this.background.overrideSprite = selected ? selectedBg : normalBg;
	}

	public Quest quest;

	bool isEquiped = false;

	public void SetQuestInfo(Quest item)
	{
		this.quest = item;
		if (this.title != null) this.title.text = this.quest.Define.Name;
	}

}
