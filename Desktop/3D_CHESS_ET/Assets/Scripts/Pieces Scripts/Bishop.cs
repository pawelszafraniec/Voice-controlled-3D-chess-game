using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Class handling bishop moves
 */
public class Bishop : ChessPiece
{
	public Bishop() : base()
	{

	}

	protected Bishop(Bishop bishop) : base(bishop)
	{
		
	}

	/**
	 * Clone method for Bishop game object
	 */
	public override ChessPiece Clone()
	{
		var piece = gameObject.AddComponent<Bishop>();
		piece.SetValues(this);
		return piece;
	}

	/**
	 * Override method determining what pieces are defended by a bishop in given position
	 */
	public override bool[,] IsPieceDefended()
	{
		bool[,] defend = new bool[8, 8];

		ChessPiece piece;

		int i;
		int j;

		//top-left diagonal
		i = PositionX;
		j = PositionY;
		while (true)
		{
			i--;
			j++;
			if (i < 0 || j >= 8)
			{
				break;
			}
			piece = ChessBoardManager.Instance.Pieces[i, j]; // check if there is a piece on checked position
			if(piece != null && isWhite == piece.isWhite) // piece of the same color found
			{ 
				defend[i, j] = true; // found piece defended
				break;
			}
		}

		//top-right diagonal
		i = PositionX;
		j = PositionY;
		while (true)
		{
			i++;
			j++;
			if (i >= 8 || j >= 8)
			{
				break;
			}
			piece = ChessBoardManager.Instance.Pieces[i, j]; // check if there is a piece on checked position
			if (piece != null && isWhite == piece.isWhite) // piece of the same color found
			{
				defend[i, j] = true; // found piece defended
				break;
			}
		}

		//down-left diagonal
		i = PositionX;
		j = PositionY;
		while (true)
		{
			i--;
			j--;
			if (i < 0 || j < 0)
			{
				break;
			}
			piece = ChessBoardManager.Instance.Pieces[i, j]; // check if there is a piece on checked position
			if ( piece != null && isWhite == piece.isWhite) // piece of the same color found
			{
				defend[i, j] = true; // found piece defended
				break;
			}
		}

		//down-right diagonal
		i = PositionX;
		j = PositionY;
		while (true)
		{
			i++;
			j--;
			if (i >= 8 || j < 0)
			{
				break;
			}
			piece = ChessBoardManager.Instance.Pieces[i, j]; // check if there is a piece on checked position
			if ((piece != null) && isWhite == piece.isWhite) // piece of the same color found
			{
				defend[i, j] = true; // found piece defended
				break;
			}

		}

		return defend;
	}

	/**
	 * Override method determining allowed moves in given position for bishop. 
	 */
	public override bool[,] IsLegalMove()
	{
		bool[,] move = new bool[8, 8];

		ChessPiece piece;

		int i;
		int j;


		i = PositionX;
		j = PositionY;
		//top-left diagonal
		while (true)
		{
			i--;
			j++;
			if (i < 0 || j >= 8)
			{
				break;
			}
			piece = ChessBoardManager.Instance.Pieces[i, j]; // check if there is a piece on given position
			if (piece == null) // no piece found
			{
				move[i, j] = true; // move allowed
			}
			else
			{
				if (isWhite != piece.isWhite) // piece of opposite color found
				{
					move[i, j] = true; // move allowed
				}
				break;
			}

		}

		i = PositionX;
		j = PositionY;
		//top-right diagonal
		while (true)
		{
			i++;
			j++;
			if (i >= 8 || j >= 8)
			{
				break;
			}
			piece = ChessBoardManager.Instance.Pieces[i, j]; // check if there is a piece on given position
			if (piece == null)
			{
				move[i, j] = true; // move allowed
			}
			else
			{
				if (isWhite != piece.isWhite) // piece of opposite color found
				{
					move[i, j] = true; // move allowed
				}
				break;
			}

		}

		i = PositionX;
		j = PositionY;
		//down-left diagonal
		while (true)
		{
			i--;
			j--;
			if (i < 0 || j < 0)
			{
				break;
			}
			piece = ChessBoardManager.Instance.Pieces[i, j]; // check if there is a piece on given position
			if (piece == null)
			{
				move[i, j] = true; // move allowed
			}
			else
			{
				if (isWhite != piece.isWhite) // piece of opposite color found
				{
					move[i, j] = true; // move allowed
				}
				break;
			}

		}

		i = PositionX;
		j = PositionY;
		//down-right diagonal
		while (true)
		{
			i++;
			j--;
			if (i >= 8 || j < 0)
			{
				break;
			}
			piece = ChessBoardManager.Instance.Pieces[i, j]; // check if there is a piece on given position
			if (piece == null)
			{
				move[i, j] = true; // move allowed
			}
			else
			{
				if (isWhite != piece.isWhite) // piece of opposite color found
				{
					move[i, j] = true; // move allowed
				}
				break;
			}

		}

		return move;

	}
}
