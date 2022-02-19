using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class handling knight moves
 */
public class Knight : ChessPiece
{
	public Knight() : base()
	{

	}

	protected Knight(Knight k) : base(k)
	{

	}

	/**
	 * Clone method for Knight game object
	 */
	public override ChessPiece Clone()
	{
		var piece = gameObject.AddComponent<Knight>();
		piece.SetValues(this);
		return piece;
	}

	/**
	 * Override method determining allowed moves in given position for knight. 
	 */
	public override bool[,] IsLegalMove()
	{
		bool[,] move = new bool[8, 8];

		//knight movement - L shaped
		#region up
		//up - left (two tiles up, one tile left)
		Move(PositionX - 1, PositionY + 2, ref move);
		//up- right
		Move(PositionX + 1, PositionY + 2, ref move);
		//right-up
		Move(PositionX + 2, PositionY + 1, ref move);
		//right-down
		Move(PositionX + 2, PositionY - 1, ref move);
		#endregion
		#region down
		//down - left (two tiles up, one tile left)
		Move(PositionX - 1, PositionY - 2, ref move);
		//down- right
		Move(PositionX + 1, PositionY - 2, ref move);
		//left-up
		Move(PositionX - 2, PositionY + 1, ref move);
		//left-down
		Move(PositionX - 2, PositionY - 1, ref move);
		#endregion
		return move;
	}

	/**
	 * Override method determining what pieces are defended by a knight in given position
	 */
	public override bool[,] IsPieceDefended()
	{
		bool[,] defend = new bool[8, 8];
		//up - left (two tiles up, one tile left)
		Defend(PositionX - 1, PositionY + 2, ref defend);
		//up- right
		Defend(PositionX + 1, PositionY + 2, ref defend);
		//right-up
		Defend(PositionX + 2, PositionY + 1, ref defend);
		//right-down
		Defend(PositionX + 2, PositionY - 1, ref defend);
		//down - left (two tiles up, one tile left)
		Defend(PositionX - 1, PositionY - 2, ref defend);
		//down- right
		Defend(PositionX + 1, PositionY - 2, ref defend);
		//left-up
		Defend(PositionX - 2, PositionY + 1, ref defend);
		//left-down
		Defend(PositionX - 2, PositionY - 1, ref defend);
		return defend;
	}

	/**
	 * Method handling single move of a knight in position
	 */
	public void Move(int x, int y, ref bool [,] arr)
	{
		ChessPiece piece;
		if(x >= 0 && x < 8 && y >= 0 && y < 8 )
		{
			piece = ChessBoardManager.Instance.Pieces[x, y]; // check if piece exist on given position
			if(piece == null) // no piece found
			{
				arr[x, y] = true;
			}
			else if(isWhite != piece.isWhite) // piece of opposite color found
			{
				arr[x, y] = true;
			}
		}
	}

	/**
	 * Method handling single defend of a knight in given position
	 */
	public void Defend(int x, int y, ref bool[,] arr)
	{
		ChessPiece piece;
		if (x >= 0 && x < 8 && y >= 0 && y < 8)
		{
			piece = ChessBoardManager.Instance.Pieces[x, y];
			if (piece != null && isWhite == piece.isWhite) // check if piece exist on given position and is from the same color
			{
				arr[x, y] = true;
			}
		}
	}
}
