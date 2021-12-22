using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for a piece
public abstract class ChessPiece : MonoBehaviour
{
	#region fields
	public int PositionX { get; set; }
	public int PositionY { get; set; }

	public bool isWhite;
	public bool isEnPassantEnabledRight;
	public bool isEnPassantEnabledLeft;
	public bool isInitialMoveDone;

	public bool isPossibleToMoveInCheck;

	public bool[,] kingCapture;

	#endregion

	public ChessPiece()
	{
		isEnPassantEnabledRight = false;
		isEnPassantEnabledLeft = false;
		isInitialMoveDone = false;
		isPossibleToMoveInCheck = false;

		kingCapture = new bool[8, 8];
	}

	public void SetPosition(int x, int y)
	{
		PositionX = x;
		PositionY = y;
	}

	public void SetInitialMoveDone()
	{
		if(!isInitialMoveDone)
			isInitialMoveDone = true;
	}

	public void SetPossibleToMoveInCheck()
	{
		isPossibleToMoveInCheck = true;
	}

	public virtual bool[,] IsLegalMove()
	{
		return new bool [8,8];
	}

	public virtual bool[,] IsPieceDefended()
	{
		return new bool[8, 8];
	}

}
