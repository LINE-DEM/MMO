using Models;
using Network;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamService : Singleton<TeamManager> {

	public void Init()
	{

	}

	public TeamService()
	{
		MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
		MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
		MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
		MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);
	}

	public void Dispose()
	{
		MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
		MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
		MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
		MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);
	}

	public void SendTeamInviteRequest(int friendId,string friendName)
	{
		Debug.Log("发送组队邀请");
		NetMessage message = new NetMessage();
		message.Request = new NetMessageRequest();
		message.Request.teamInviteReq = new TeamInviteRequest();
		message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacter.Id;
		message.Request.teamInviteReq.FromName = User.Instance.CurrentCharacter.Name;
		message.Request.teamInviteReq.ToId = friendId;
		message.Request.teamInviteReq.ToName = friendName;
		NetClient.Instance.SendMessage(message);
	}

	public void SendTeamInviteResponse(bool accept,TeamInviteRequest request)
	{
		Debug.Log("发送组队响应");
		NetMessage message = new NetMessage();
		message.Request = new NetMessageRequest();
		message.Request.teamInviteRes = new TeamInviteResponse();
		message.Request.teamInviteRes.Result= accept ? Result.Success : Result.Failed;
		message.Request.teamInviteRes.Errormsg = accept ? "组队成功" : "对方拒绝";
		message.Request.teamInviteRes.Request = request;
		NetClient.Instance.SendMessage(message);
	}

	/// <summary>
	/// ***********************收到添加组队请求的响应*******************
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="request"></param>
	private void OnTeamInviteRequest(object sender,TeamInviteRequest request)
	{
		var confirm = MessageBox.Show(string.Format("{0} 邀请你加入队伍", request.FromName), "组队请求", MessageBoxType.Confirm, "接受", "拒绝");
		confirm.OnYes = () =>
		{
			this.SendTeamInviteResponse(true, request);
		};
		confirm.OnNo = () =>
		{
			this.SendTeamInviteResponse(false, request);
		};
	
	}

	/// <summary>
	/// ***************************最后的结果响应********
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="message"></param>
	private void OnTeamInviteResponse(object sender,TeamInviteResponse message)
	{
		if (message.Result == Result.Success)
			MessageBox.Show(message.Request.ToName + "加入你的队伍", "邀请组队成功");
		else
			MessageBox.Show(message.Errormsg, "邀请组队失败");
	}
	

	

	private void OnTeamInfo(object sender,TeamInfoResponse message)
	{
		Debug.Log("更新队伍信息");
		TeamManager.Instance.UpdateTeamInfo(message.Team);
	}

	public void SendTeamLeaveRquest(int id)
	{
		Debug.Log("发送离开队伍请求");
		NetMessage message = new NetMessage();
		message.Request = new NetMessageRequest();
		message.Request.teamLeave = new TeamLeaveRequest();
		message.Request.teamLeave.TeamId = User.Instance.TeamInfo.Id;
		message.Request.teamLeave.characterId = User.Instance.CurrentCharacter.Id;
		NetClient.Instance.SendMessage(message);
	}

	private void OnTeamLeave(object sender,TeamLeaveResponse message)
	{
		if(message.Restult == Result.Success)
		{
			TeamManager.Instance.UpdateTeamInfo(null);
			MessageBox.Show("退出成功", "退出队伍");

		}
		else
		{
			MessageBox.Show("退出失败", "退出队伍", MessageBoxType.Error);
		}
	}
}
