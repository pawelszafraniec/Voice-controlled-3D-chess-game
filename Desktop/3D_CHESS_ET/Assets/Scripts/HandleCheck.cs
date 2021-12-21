using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCheck : MonoBehaviour
{
    public static HandleCheck Instance { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void CheckKingCheck()
	{

	}

    public void HandleKingCheck(bool handle)
	{
        Debug.Log("Check handling");
        var a = ChessBoardManager.Instance.isCheck;
        Debug.Log("Check = " + a);









	}

}
