using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
/// <summary>
///             挂载在两个Content上 子类挂载到需要管理的Item上
/// </summary>
public class ListView : MonoBehaviour
{
    public UnityAction<ListViewItem> onItemSelected;
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                onSelected(selected);
            }
        }
        public virtual void onSelected(bool selected)
        {
        }

        public ListView owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.selected)
            {
                this.Selected = true;
            }
           
            if (owner != null && owner.SelectedItem != this)
            {
                owner.SelectedItem = this;
            }
            else if(owner.SelectedItem == this)
            {
                owner.selectedItem = null;
                this.Selected = false;
            }
        }
    }

    //管理的列表
    [HideInInspector]
    public  List<ListViewItem> items = new List<ListViewItem>();

    private ListViewItem selectedItem = null;

    // Set SelectedItem 的时候调用注册给别人的事件
    public ListViewItem SelectedItem
    {
        get { return selectedItem; }
        private set
        {
            if (selectedItem != null && selectedItem != value)
            {
                selectedItem.Selected = false;
            }
            selectedItem = value;
            if (onItemSelected != null)
                onItemSelected.Invoke((ListViewItem)value);
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);
    }

    //方便刷新方法 可以直接清除
    public void RemoveAll()
    {
        foreach (var it in items)
        {
            Destroy(it.gameObject);
        }
        items.Clear();
    }
}
