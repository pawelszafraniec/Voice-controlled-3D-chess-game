using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionsManager : MonoBehaviour
{
	public Dropdown timeDropdown;
	
	public static string timeValue;
	public void Start()
	{
		Debug.Log(timeDropdown.value);
	}
	public void Update()
	{
		Debug.Log(timeDropdown.options[timeDropdown.value].text);
		timeValue = timeDropdown.options[timeDropdown.value].text;
	}

}
