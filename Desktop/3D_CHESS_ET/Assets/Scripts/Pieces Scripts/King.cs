using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
	public bool CastleKing = false;
	public bool CastleQueen = false;

	public override bool[,] IsLegalMove()
	{
		bool[,] move = new bool[8, 8];

		ChessPiece piece;
		int i;
		int j;

		//upewnic sie ze komentarze mozna usunac

		//top
		i = PositionX - 1;
		j = PositionY + 1;
		if(PositionY != 7)
		{
			for(int k = 0; k <3; k++)
			{
				if(i>=0 && i<8)
				{
					piece = ChessBoardManager.Instance.Pieces[i, j];
					if (piece == null)
					{
						move[i, j] = true;
					}
					else if (isWhite != piece.isWhite)
					{
						move[i, j] = true;
					}
				}

				i++;
			}
		}
		
		//down
		i = PositionX - 1;
		j = PositionY - 1;
		if (PositionY != 0)
		{
			for (int k = 0; k < 3; k++)
			{
				if (i >= 0 && i < 8)
				{

					piece = ChessBoardManager.Instance.Pieces[i, j];
					if (piece == null)
					{
						move[i, j] = true;
					}
					else if (isWhite != piece.isWhite)
					{
						move[i, j] = true;
					}
				}

				i++;
			}
		}

		//mid-left
		if(PositionX !=0)
		{
		piece = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY];
			if (piece == null)
			{
				move[PositionX - 1, PositionY] = true;
			}
			else if (isWhite != piece.isWhite)
			{
				move[PositionX - 1, PositionY] = true;
			}
		}

		//mid-right
		if (PositionX != 7)
		{
			piece = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY];

			if (piece == null)
			{
				move[PositionX + 1, PositionY] = true;
			}
			else if (isWhite != piece.isWhite)
			{
				move[PositionX + 1, PositionY] = true;

			}
		}

		//castle right
		if(isInitialMoveDone == false)
		{
			piece = ChessBoardManager.Instance.Pieces[PositionX + 3, PositionY];
			if(piece != null && piece.GetType() == typeof(Rook) && piece.isInitialMoveDone == false
				&& ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY] == null
				&& ChessBoardManager.Instance.Pieces[PositionX + 2, PositionY] == null)
			{
				if(ChessBoardManager.Instance.isCheck == false)
					move[PositionX + 2, PositionY] = true;
			}
			piece = ChessBoardManager.Instance.Pieces[PositionX - 4, PositionY];
			if (piece != null && piece.GetType() == typeof(Rook) && piece.isInitialMoveDone == false
				&& ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY] == null
				&& ChessBoardManager.Instance.Pieces[PositionX - 2, PositionY] == null
				&& ChessBoardManager.Instance.Pieces[PositionX - 3, PositionY] == null)
			{
				if (ChessBoardManager.Instance.isCheck == false)
					move[PositionX - 2, PositionY] = true;
			}
		}



		return move;
	}

	public override bool[,] IsPieceDefended()
	{
		bool[,] defend = new bool[8, 8];

		ChessPiece piece;
		int i;
		int j;


		//top
		i = PositionX - 1;
		j = PositionY + 1;
		if (PositionY != 7)
		{
			for (int k = 0; k < 3; k++)
			{
				if (i >= 0 && i < 8)
				{
					piece = ChessBoardManager.Instance.Pieces[i, j];
					if (piece != null && isWhite == piece.isWhite)
					{
						defend[i, j] = true;
					}
				}

				i++;
			}
		}

		//down
		i = PositionX - 1;
		j = PositionY - 1;
		if (PositionY != 0)
		{
			for (int k = 0; k < 3; k++)
			{
				if (i >= 0 && i < 8)
				{

					piece = ChessBoardManager.Instance.Pieces[i, j];
					if (piece != null && isWhite == piece.isWhite)
					{
						defend[i, j] = true;
					}
				}

				i++;
			}
		}

		//mid-left
		if (PositionX != 0)
		{
			piece = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY];
			if (piece != null && isWhite == piece.isWhite)
			{
				defend[PositionX - 1, PositionY] = true;
			}
		}

		//mid-right
		if (PositionX != 7)
		{
			piece = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY];
			if (piece != null && isWhite == piece.isWhite)
			{
				defend[PositionX + 1, PositionY] = true;
			}
		}

		return defend;
	}
}
