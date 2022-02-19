using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Helper class handling operations on strings
 */
public class StringHelper : MonoBehaviour
{
	public static StringHelper Instance;
	public void Start()
	{
		Instance = this;
	}

	/**
	 * Method splitting string by half and returing result as a tuple 
	 */
	public Tuple<int,int> GetBoardCoordinates(string input)
	{
		var first = Int32.Parse(input.Substring(0, (int)(input.Length / 2)));
		var last = Int32.Parse(input.Substring((int)(input.Length / 2), (int)(input.Length / 2)));
		return Tuple.Create(first,last);
	}
	/**
	 * Method splitting string by half and returing result as a tuple 
	 */
	public Tuple<String,String> SplitString(string input)
	{
		var first = input.Substring(0, (int)(input.Length / 2));
		var last = input.Substring((int)(input.Length / 2), (int)(input.Length / 2));

		return Tuple.Create(first, last);
	}

}
