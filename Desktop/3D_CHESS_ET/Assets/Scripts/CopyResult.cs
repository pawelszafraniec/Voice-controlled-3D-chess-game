using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CopyResult
{
	public static void CopyResultStringToClipBoard(this string result)
	{
		TextEditor editor = new TextEditor();
		editor.text = result;
		editor.SelectAll();
		editor.Copy();
	}

}
