﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Common.Data;
using UnityEngine.Analytics;

public class MapTools : MonoBehaviour {

	[MenuItem("Map Tools/Export Teleporters")]
	public static void ExportTeleporters()
	{
		DataManager.Instance.Load();

		Scene current = EditorSceneManager.GetActiveScene();
		string currentScene = current.name;
		if (current.isDirty)
		{
			EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
			return;
		}

		List<TeleporterObject> allTeleporters = new List<TeleporterObject>();
		
		foreach(var map in DataManager.Instance.Maps)
		{
			string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
			if (!System.IO.File.Exists(sceneFile))
			{
				Debug.LogWarningFormat("Scene {0} not existed!", sceneFile);
				continue;
			}
			EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

			TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();
			foreach (var teleporter in teleporters)
			{
				print(teleporter.name);
				if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
				{
					EditorUtility.DisplayDialog("错误", string.Format("地图：{0} 中配置的Teleporter:{1}", map.Value.ID, teleporter.ID),"OK");
					return;
				}
				TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
				if(def.MapID != map.Value.ID)
				{
					EditorUtility.DisplayDialog("错误", string.Format("地图{0} 中配置的 Teleporter:{1}", map.Value.ID, teleporter.ID), "OK");
					return;
				}
				def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
				def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
			}
			
		}
		DataManager.Instance.SaveTeleporters();
		EditorSceneManager.OpenScene("Assets/Levels/"+currentScene + ".unity");
		EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");
	}
}