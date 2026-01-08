using System;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

public static class CopyPathMenuItem
{
	[MenuItem("GameObject/Copy Path")]
	private static void CopyPath()
	{
		var go = Selection.activeGameObject;

		if (go == null)
		{
			return;
		}

		var path = go.name;

		while (go.transform.parent != null)
		{
			go = go.transform.parent.gameObject;
			path = string.Format("{0}/{1}", go.name, path);
		}

		EditorGUIUtility.systemCopyBuffer = path;
	}

	[MenuItem("GameObject/2D Object/Copy Path", true)]
	private static bool CopyPathValidation()
	{
		// We can only copy the path in case 1 object is selected
		return Selection.gameObjects.Length == 1;
	}

	static StringBuilder sb = new();

	[MenuItem("GameObject/Generate LocString class")]
	public static void ExportLocStringsClass()
	{
		var go = Selection.activeGameObject;
		sb.Clear();

		if (go == null)
		{
			return;
		}
		AppendStringForGORecursive(go);

		EditorGUIUtility.systemCopyBuffer = sb.ToString();
	}
	static void AppendStringForGORecursive(GameObject go, int depth = 0)
	{
		void AppendTabs() => sb.Append(new string('\t', depth));

		if (go.TryGetComponent<TextMeshProUGUI>(out var tmp))
		{
			AppendTabs();
			sb.Append("public static LocString ");
			sb.Append(go.name.ToUpperInvariant());
			sb.Append(" = \"");
			sb.Append(tmp.text.Trim('\n'));
			sb.Append("\";");
			sb.AppendLine();

		}
		else if (go.GetComponentInChildren<TextMeshProUGUI>(true) != null)
		{
			AppendTabs();
			sb.Append("public class ");
			sb.Append(go.name.ToUpperInvariant());
			sb.AppendLine();
			AppendTabs();
			sb.AppendLine("{");
			foreach(Transform child in go.transform)
			{
				AppendStringForGORecursive(child.gameObject, depth+1);
			}
			AppendTabs();
			sb.AppendLine("}");
		}
	}
}
