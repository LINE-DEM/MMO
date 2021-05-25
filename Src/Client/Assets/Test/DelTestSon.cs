using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelTestSon : DelTest {


	// Use this for initialization
	//public override void Start () {
	//	print("重新了");
	//	Show<DelTest>();
	//}

	//public T Show<T>()
	//{
	//	Type type = typeof(T);
	//	print(type);
	//	return default(T);
	//}

	public override void Start()
	{
		base.Start();
		OnClose(HowClose.fast);
	}

}
