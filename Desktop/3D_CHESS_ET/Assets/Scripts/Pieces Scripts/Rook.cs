using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class handling rook moves
 */
public class Rook : ChessPiece
{
	public Rook() : base()
	{

	}

	protected Rook(Rook k) : base(k)
	{

	}

	/**
	* Clone method for Rook game object
	*/
	public override ChessPiece Clone()
	{
		var piece = gameObject.AddComponent<Rook>();
		piece.SetValues(this);
		return piece;
	}

	/**
	 * Override method determining allowed moves in given position for rook
	 */
	public override bool[,] IsLegalMove()
	{
		bool [,] b = new bool[8, 8];
		ChessPiece chp;

		//right
		int i = PositionX;
		while (true)
		{
			i++;
			if (i >= 8)
			{
				break;
			}

			
			chp = ChessBoardManager.Instance.Pieces[i, PositionY]; // Piece on checked position
			if (chp == null) // no Piece found
			{
				b[i, PositionY] = true; // movement is allowed
			}
			else
			{
				if(chp.isWhite !=isWhite) // piece found and it is from opposite color
				{
					b[i, PositionY] = true; // movement (piece capture) is allowed 
				}
				break;
			}

		}
		//left
		i = PositionX;
		while (true)
		{
			i--;
			if (i < 0)
			{
				break;
			}

			chp = ChessBoardManager.Instance.Pieces[i, PositionY]; // Piece on checked position
			if (chp == null) // no Piece found
			{
				b[i, PositionY] = true; // movement is allowed
			}
			else
			{
				if (chp.isWhite != isWhite) // piece found and it is from opposite color
				{
					b[i, PositionY] = true; // movement (piece capture) is allowed 
				}
				break;
			}

		}
		//Top
		i = PositionY;
		while (true)
		{
			i++;
			if (i >= 8)
			{
				break;
			}

			chp = ChessBoardManager.Instance.Pieces[PositionX, i]; // Piece on checked position
			if (chp == null) // no Piece found
			{
				b[PositionX, i] = true; // movement is allowed
			}
			else
			{
				if (chp.isWhite != isWhite) // piece found and it is from opposite color
				{
					b[PositionX, i] = true; // movement (piece capture) is allowed 
				}
				break;
			}

		}
		//Down
		i = PositionY;
		while (true)
		{
			i--;
			if (i < 0)
			{
				break;
			}

			chp = ChessBoardManager.Instance.Pieces[PositionX, i]; // Piece on checked position
			if (chp == null)
			{
				b[PositionX, i] = true;  // no Piece found
			}
			else
			{
				if (chp.isWhite != isWhite) // piece found and it is from opposite color
				{
					b[PositionX, i] = true;  // movement (piece capture) is allowed 
				}
				break;
			}

		}

		return b;

	}
	
	/**
	 * Override method determining what pieces are defended by a rook in given position
	 */
	public override bool[,] IsPieceDefended()
	{
		bool[,] defend = new bool[8, 8];
		ChessPiece chp;

		int i = PositionX;
		while (true) // right
		{
			i++;
			if (i >= 8)
			{
				break;
			}
			chp = ChessBoardManager.Instance.Pieces[i, PositionY]; // Piece on checked position
			if (chp != null && isWhite == chp.isWhite) // piece is found and it is from the same color
			{
				defend[i, PositionY] = true; // piece defended
				break;
			}
		}
		i = PositionX;
		while (true) // left
		{
			i--;
			if (i < 0)
			{
				break;
			}
			chp = ChessBoardManager.Instance.Pieces[i, PositionY]; // Piece on checked position
			if (chp != null && isWhite == chp.isWhite) // piece is found and it is from the same color
			{
				defend[i, PositionY] = true; // piece defended
				break;
			}
		}
		i = PositionY;
		while (true) // top
		{
			i++;
			if (i >= 8)
			{
				break;
			}
			chp = ChessBoardManager.Instance.Pieces[PositionX, i]; // Piece on checked position
			if (chp != null && isWhite == chp.isWhite) // piece is found and it is from the same color
			{
				defend[PositionX, i] = true; // piece defended
				break;
			}
		}
		i = PositionY;
		while (true) // down
		{
			i--;
			if (i < 0)
			{
				break;
			}
			chp = ChessBoardManager.Instance.Pieces[PositionX, i]; // Piece on checked position
			if (chp != null && isWhite == chp.isWhite) // piece is found and it is from the same color
			{
				defend[PositionX, i] = true; // piece defended
				break;
			}
		}

		return defend;
	}

}
