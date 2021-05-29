using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendManager : Singleton<FriendManager> {

	public List<NFriendInfo> allFriends;

	public void Init(List<NFriendInfo> friends)
	{
		this.allFriends = friends;
	}
	
}
