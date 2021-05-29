using Common.Data;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UISkills : UIWindow {

	public Image[] skills;
	public ListView listView;

	public Text cost;
	public Text description;

	UISkillItem skillItem;

	void Start()
	{
		listView.onItemSelected += OnSelectItem;
		SkillService.Instance.OnSkillRefrash += Init;
		Init();
	}
	public void Init()
	{
		for (int i = 0; i < 8; i++)
		{
			if(SkillManager.Instance.skills[i].Unlocked != 1)
				skills[i].color = Color.black;
			else
				skills[i].color = Color.white;


			listView.AddItem(skills[i].gameObject.GetComponent<UISkillItem>());
		}
	}

	public void OnClickOpen()
	{
		if (skillItem == null)
			MessageBox.Show("请先选择要学习的技能哦");

		//NSkillInfo temp = new NSkillInfo();
		//SkillManager.Instance.skills.TryGetValue(skillItem.id, out temp);
		SkillService.Instance.SendSkillOpenRequest(skillItem.id);
	}

	public void OnSelectItem(ListView.ListViewItem item)
	{
		skillItem = item as UISkillItem;
		SkillDefine define;
		DataManager.Instance.Skills.TryGetValue(skillItem.id, out define);
		description.text = define.Description;
		cost.text = define.Cost.ToString();
		Init();
	}

	void OnRefsh()
	{

	}
}
