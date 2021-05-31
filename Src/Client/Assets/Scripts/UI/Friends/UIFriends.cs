using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriends : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIFriendItem selectedItem;

    void Start()
    {
        FriendService.Instance.OnFriendUpdate = RefreshUI;
        this.listMain.onItemSelected += this.OnFriendSelected;
        RefreshUI();
    }


    public void OnFriendSelected(ListView.ListViewItem item)
    {
        //安全的显示转换   把指向父类的引用   变成  指向子类的引用
        this.selectedItem = item as UIFriendItem;
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("输入要添加的好友名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
    }

    private bool OnFriendAddSubmit(string input,out string tips)
    {
        tips = "";//out 参数需要在内部赋值 
        int friendId = 0;
        string friendName = "";
        //如果 不能 转换 成int类型 就
        if (!int.TryParse(input, out friendId))
            friendName = input;

        if(friendId == User.Instance.CurrentCharacter.Id||friendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "不能添加自己，傻逼吧你";
            return false;
        }

        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("暂时未开放");
    }

    public void OnClickFriendTeamInvite()
    {
        if(selectedItem == null)
        {
            MessageBox.Show("请先选择要邀请的好友哦");
            return;
        }
        if (selectedItem.Info.Status == 0)
        {
            MessageBox.Show("对方不在线啊 邀请不了-_-");
            return;
        }
        MessageBox.Show(string.Format("确定要邀请好友【{0}】加入吗？", selectedItem.Info.friendInfo.Name),
            "邀请好友组队", MessageBoxType.Confirm, "邀请", "取消").OnYes =
            () =>
            {
                TeamService.Instance.SendTeamInviteRequest
            (this.selectedItem.Info.friendInfo.Id, this.selectedItem.Info.friendInfo.Name);
            };
    }

    public void OnClickFriendRemove()
    {
        if(selectedItem == null)
        {
            MessageBox.Show("请先选择要删除的好友");
            return;
        }
        MessageBox.Show(
            string.Format("确定要删除好友【{0}】吗？", selectedItem.Info.friendInfo.Name),
            "删除好友",
            MessageBoxType.Confirm,
            "删除",
            "取消").OnYes =
            () => { FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.Info.Id, this.selectedItem.Info.friendInfo.Id); };
    }

    void RefreshUI()
    {
        ClearFriendList();
        InitFriendItems();
    }

    void InitFriendItems()
    {
        foreach (var item in FriendManager.Instance.allFriends)
        {
            GameObject go = Instantiate(itemPrefab, this.itemRoot.transform);
            UIFriendItem ui = go.GetComponent<UIFriendItem>();
            ui.SetFriendInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    void ClearFriendList()
    {
        this.listMain.RemoveAll();
    }
}
