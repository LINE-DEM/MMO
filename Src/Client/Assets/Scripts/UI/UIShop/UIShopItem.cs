using Common.Data;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour ,ISelectHandler{

	public Image icon;
	public Text title;
	public Text price;
	public Text limitClass;
	public Text count;

	public Image background;
	public Sprite normalBg;
	public Sprite selectedBg;

	private bool selected;
	public  bool Selected
	{
		get { return selected; }
		set
		{
			selected = value;
			this.background.overrideSprite = selected ? selectedBg : normalBg;
		}

	}

	public int ShopItemID { get; set; }
	private UIShop shop;
	private ItemDefine item;
	private ShopItemDefine ShopItem { get; set; }

	void Start()
	{

	}

	public void SetShopItem(int id,ShopItemDefine shopItem,UIShop owner)
	{
		this.shop = owner;
		this.ShopItemID = id;
		this.ShopItem = shopItem;
		this.item = DataManager.Instance.Items[this.ShopItem.ItemID];

		this.title.text = this.item.Name;
		this.count.text = "x" + ShopItem.Count.ToString();
		if(this.item.LimitClass != 0)this.limitClass.text = this.item.LimitClass.ToString();
		this.price.text = ShopItem.Price.ToString();
		this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
	}

	public void OnSelect(BaseEventData eventData)
	{
		
		this.Selected = true;
	
		this.shop.SelectShopItem(this);
	}

	
}
