using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Base class for chess pieces
 */
public abstract class ChessPiece : MonoBehaviour
{
	#region fields

	public int Id { get; set; }
	public int PositionX { get; set; }
	public int PositionY { get; set; }

	public bool isWhite;
	public bool isEnPassantEnabledRight;
	public bool isEnPassantEnabledLeft;
	public bool isInitialMoveDone;
	public bool isPossibleToMoveInCheck;

	public bool[,] kingCapture;

	#endregion
	/**
	 * Method setting values to ChessPiece game object
	 */
	protected void SetValues(ChessPiece piece)
	{
		PositionX = piece.PositionX;
		PositionY = piece.PositionY;
		isWhite = piece.isWhite;
		isEnPassantEnabledRight = piece.isEnPassantEnabledRight;
		isEnPassantEnabledLeft = piece.isEnPassantEnabledLeft;
		isInitialMoveDone = piece.isInitialMoveDone;
		isPossibleToMoveInCheck = piece.isPossibleToMoveInCheck;
		kingCapture = piece.kingCapture;
	}

	public ChessPiece()
	{
		isEnPassantEnabledRight = false;
		isEnPassantEnabledLeft = false;
		isInitialMoveDone = false;
		isPossibleToMoveInCheck = false;

		kingCapture = new bool[8, 8];
	}

	protected ChessPiece(ChessPiece piece)
	{
		PositionX = piece.PositionX;
		PositionY = piece.PositionY;
		isWhite = piece.isWhite;
		isEnPassantEnabledRight = piece.isEnPassantEnabledRight;
		isEnPassantEnabledLeft = piece.isEnPassantEnabledLeft;
		isInitialMoveDone = piece.isInitialMoveDone;
		isPossibleToMoveInCheck = piece.isPossibleToMoveInCheck;
		kingCapture = piece.kingCapture;
	}

	public abstract ChessPiece Clone();

	/**
	 * Set ChessPiece position method
	 */
	public void SetPosition(int x, int y)
	{
		PositionX = x;
		PositionY = y;
	}

	/**
	 * Method setting initial move of a piece done
	 */
	public void SetInitialMoveDone()
	{
		if(!isInitialMoveDone)
			isInitialMoveDone = true;
	}

	/**
	 * Method setting a ChessPiece able to move during check state
	 */
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
