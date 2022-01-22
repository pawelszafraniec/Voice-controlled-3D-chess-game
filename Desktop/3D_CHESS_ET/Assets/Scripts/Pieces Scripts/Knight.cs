using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
	public Knight() : base()
	{

	}

	protected Knight(Knight k) : base(k)
	{

	}

	public override ChessPiece Clone()
	{
		var piece = gameObject.AddComponent<Knight>();
		piece.SetValues(this);
		return piece;
	}

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

	public void Move(int x, int y, ref bool [,] arr)
	{
		ChessPiece piece;
		if(x >= 0 && x < 8 && y >= 0 && y < 8 )
		{
			piece = ChessBoardManager.Instance.Pieces[x, y];
			if(piece == null)
			{
				arr[x, y] = true;
			}
			else if(isWhite != piece.isWhite)
			{
				arr[x, y] = true;
			}
		}
	}
	public void Defend(int x, int y, ref bool[,] arr)
	{
		ChessPiece piece;
		if (x >= 0 && x < 8 && y >= 0 && y < 8)
		{
			piece = ChessBoardManager.Instance.Pieces[x, y];
			if (piece != null && isWhite == piece.isWhite)
			{
				arr[x, y] = true;
			}
		}
	}
}
