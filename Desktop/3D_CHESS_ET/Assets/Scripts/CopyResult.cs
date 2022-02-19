using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class handling copying to clipboard
 */
public static class CopyResult
{
	/**
	 * Method copying string value to clipboard
	 */
	public static void CopyResultStringToClipBoard(this string result)
	{
		TextEditor editor = new TextEditor();
		editor.text = result;
		editor.SelectAll(); // select all
		editor.Copy(); // copy
	}

}
