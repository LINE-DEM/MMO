using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

	class UIElement
	{
		public string Resource;
		public bool Cache;
		public GameObject Instance;
	}

	private Dictionary<Type, UIElement> UIResource = new Dictionary<Type, UIElement>();

	public UIManager()
	{
		this.UIResource.Add(typeof(UITest), new UIElement() { Resource = "UI/UITest", Cache = true });
		this.UIResource.Add(typeof(UIBag), new UIElement() { Resource = "UI/UIBag", Cache = false });
		this.UIResource.Add(typeof(UIShop), new UIElement() { Resource = "UI/UIShop", Cache = false });
		this.UIResource.Add(typeof(UICharEquip), new UIElement() { Resource = "UI/UICharEquip", Cache = false });
		this.UIResource.Add(typeof(UIQuestSystem), new UIElement() { Resource = "UI/UIQuestSystem" ,Cache = false});
		this.UIResource.Add(typeof(UIQuestDialog), new UIElement() { Resource = "UI/UIQuestDialog", Cache = false });
		this.UIResource.Add(typeof(UIFriends), new UIElement() { Resource = "UI/UIFriends", Cache = false });
		this.UIResource.Add(typeof(UISkills), new UIElement() { Resource = "UI/UISkill", Cache = false });
	}

	~UIManager()
	{

	}

	public T Show<T>()
	{
		Type type = typeof(T);
		if (this.UIResource.ContainsKey(type))
		{
			UIElement info = this.UIResource[type];
			if(info.Instance != null)
			{
				info.Instance.SetActive(true);
			}
			else
			{
				UnityEngine.Object prefab = Resources.Load(info.Resource);
				if(prefab == null)
				{
					return default(T);
				}
				info.Instance = (GameObject)GameObject.Instantiate(prefab);
			}
			return info.Instance.GetComponent<T>();
		}
		return default(T);
	}

	public void Close(Type type)
	{
		if (this.UIResource.ContainsKey(type))
		{
			UIElement info = this.UIResource[type];
			if (info.Cache)
			{
				info.Instance.SetActive(false);
			}
			else
			{
				GameObject.Destroy(info.Instance);
				info.Instance = null;
			}
		}
	}
}
