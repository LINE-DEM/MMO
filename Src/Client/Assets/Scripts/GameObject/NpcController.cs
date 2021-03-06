using Common.Data;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NpcController : MonoBehaviour {

	public int npcID;

	SkinnedMeshRenderer renderer;
	Animator anim;
	Color orignColor;

	private bool inInteractive = false;

	NpcDefine npc;

	NpcQuestStatus questStatus;
	// Use this for initialization
	void Start () {
		renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		anim = this.gameObject.GetComponent<Animator>();
		orignColor = renderer.sharedMaterial.color;
		npc = NPCManager.Instance.GetNpcDefine(npcID);
		Debug.Log(npc.Name);
		this.StartCoroutine("Actions");
		QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChange;
		this.RefresNpcStatus();
	}
	
	void OnQuestStatusChange(Quest quest)
	{
		this.RefresNpcStatus();
	}

	void RefresNpcStatus()
	{
		questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.npcID);
		UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform, questStatus);
	}

	void OnDestroy()
	{
		QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChange;
		if(UIWorldElementManager.Instance != null)
		{
			UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
		}
	}

	IEnumerator Actions()
	{
		anim.SetTrigger("Idle");
		while (true)
		{
			
			if (inInteractive)
				yield return new WaitForSeconds(2f);
			else
				yield return new WaitForSeconds(Random.Range(5f, 10f));
			this.Relax();
			yield return new WaitForSeconds(2f);
			
			anim.ResetTrigger("Relax");
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
	
	void Relax()
	{
		anim.SetTrigger("Relax");
	}

	void Interactive()
	{
		if (!inInteractive)
		{
			inInteractive = true;
			StartCoroutine("DoInteractive"); 
		}
	}

	IEnumerator DoInteractive()
	{
		yield return FaceToPlayer();
		if (NPCManager.Instance.Interactive(npc))
		{
			anim.SetTrigger("Talk");
		}
		yield return new WaitForSeconds(3f);
		anim.ResetTrigger("Talk");
		inInteractive = false;
	}

	IEnumerator FaceToPlayer()
	{
		Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
		while(Mathf.Abs(Vector3.Angle(this.gameObject.transform .forward,faceTo)) > 5)
		{
			this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, 5f * Time.deltaTime);
			yield return null;
		}
	}


	void OnMouseDown()
	{
		Debug.Log(this.name);
		Interactive();
	}

	private void OnMouseOver()
	{
		Highlight(true);
	}
	private void OnMouseEnter()
	{
		Highlight(true);
	}
	private void OnMouseExit()
	{
		Highlight(false);
	}
	void Highlight (bool highlight)
	{
		if (highlight)
		{
			if (renderer.sharedMaterial.color != Color.white)
				renderer.sharedMaterial.color = Color.white;
		}
		else
		{
			if (renderer.sharedMaterial.color != orignColor)
				renderer.sharedMaterial.color = orignColor;
		}
	}
}
