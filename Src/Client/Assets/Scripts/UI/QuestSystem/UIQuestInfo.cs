using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
///		右边的任务信息 就一个set方法
/// </summary>
public class UIQuestInfo : MonoBehaviour {

	public Text title;

	public Text[] targets;

	public Text description;

	public UIIconItem rewardItems;

	public Text rewardMoney;
	public Text rewardExp;


	//设置右边的 任务信息
	public void SetQuestInfo(Quest quest)
	{
		this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
		if(quest.Info == null)
		{
			this.description.text = quest.Define.Dialog;
		}
		else
		{
			if(quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
			{
				this.description.text = quest.Define.DialogFinish;
			}
			else
			{
				this.description.text = quest.Define.Dialog;
			}
		}

		this.rewardMoney.text = quest.Define.RewardGold.ToString();
		this.rewardExp.text = quest.Define.RewardExp.ToString();
		//每次设置自动适应一下
		//foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
		//{
		//	fitter.SetLayoutVertical();
		//}
	}
}
