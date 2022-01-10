using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpecialMoves : MonoBehaviour
{
	Button abc;	
	public void SpawnQueen()
	{
		int xpos = ChessBoardManager.Instance.XLocation;
		int ypos = ChessBoardManager.Instance.YLocation;
		if (ChessBoardManager.Instance.isWhiteTurn)
		{
			//light queen
			ChessBoardManager.Instance.SpawnChessPiece(4, xpos, ypos, "WhiteQueen");
			ChessBoardManager.Instance.popUpWindowForWhitePromotion.SetActive(false);
		}
		else
		{
			//dark queen
		}
	}
	public void SpawnRook()
	{

	}
	public void SpawnKnight()
	{

	}
	public void SpawnBishop()
	{

	}


}
