using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
	public Rook() : base()
	{

	}

	protected Rook(Rook k) : base(k)
	{

	}

	public override ChessPiece Clone()
	{
		var piece = gameObject.AddComponent<Rook>();
		piece.SetValues(this);
		return piece;
	}

	public override bool[,] IsLegalMove()
	{
		bool [,] b = new bool[8, 8];
		ChessPiece chp;
		int i = PositionX;

		//right
		while(true)
		{
			i++;
			if (i >= 8)
			{
				break;
			}

			chp = ChessBoardManager.Instance.Pieces[i, PositionY];
			if (chp == null)
			{
				b[i, PositionY] = true;
			}
			else
			{
				if(chp.isWhite !=isWhite)
				{
					b[i, PositionY] = true;
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

			chp = ChessBoardManager.Instance.Pieces[i, PositionY];
			if (chp == null)
			{
				b[i, PositionY] = true;
			}
			else
			{
				if (chp.isWhite != isWhite)
				{
					b[i, PositionY] = true;
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

			chp = ChessBoardManager.Instance.Pieces[PositionX, i];
			if (chp == null)
			{
				b[PositionX, i] = true;
			}
			else
			{
				if (chp.isWhite != isWhite)
				{
					b[PositionX, i] = true;
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

			chp = ChessBoardManager.Instance.Pieces[PositionX, i];
			if (chp == null)
			{
				b[PositionX, i] = true;
			}
			else
			{
				if (chp.isWhite != isWhite)
				{
					b[PositionX, i] = true;
				}
				break;
			}

		}

		return b;

	}
	public override bool[,] IsPieceDefended()
	{
		bool[,] defend = new bool[8, 8];
		ChessPiece chp;

		int i = PositionX;
		while (true)
		{
			i++;
			if (i >= 8)
			{
				break;
			}
			chp = ChessBoardManager.Instance.Pieces[i, PositionY];
			if (chp != null && isWhite == chp.isWhite)
			{
				defend[i, PositionY] = true;
				break;
			}
		}
		i = PositionX;
		while (true)
		{
			i--;
			if (i < 0)
			{
				break;
			}
			chp = ChessBoardManager.Instance.Pieces[i, PositionY];
			if (chp != null && isWhite == chp.isWhite)
			{
				defend[i, PositionY] = true;
				break;
			}
		}
		i = PositionY;
		while (true)
		{
			i++;
			if (i >= 8)
			{
				break;
			}
			chp = ChessBoardManager.Instance.Pieces[PositionX, i];
			if (chp != null && isWhite == chp.isWhite)
			{
				defend[PositionX, i] = true;
				break;
			}
		}
		i = PositionY;
		while (true)
		{
			i--;
			if (i < 0)
			{
				break;
			}
			chp = ChessBoardManager.Instance.Pieces[PositionX, i];
			if (chp != null && isWhite == chp.isWhite)
			{
				defend[PositionX, i] = true;
				break;
			}
		}

		return defend;
	}

}
