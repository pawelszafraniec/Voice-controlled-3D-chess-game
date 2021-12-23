using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{

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
		if (PositionY != 7)
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
		if (PositionX != 0)
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
		if (isInitialMoveDone == false)
		{
			piece = ChessBoardManager.Instance.Pieces[PositionX + 3, PositionY];
			if (piece != null && piece.GetType() == typeof(Rook) && piece.isInitialMoveDone == false
				&& ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY] == null
				&& ChessBoardManager.Instance.Pieces[PositionX + 2, PositionY] == null)
			{
				if (ChessBoardManager.Instance.isCheck == false)
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

	public bool[,] FilterForbiddenMoves(bool[,] allowedByCore, int x, int y)
	{
		bool[,] filtererd = allowedByCore;
		bool[,] forbiddenMoves;
		bool[,] protectedMoves;

		foreach (ChessPiece p in ChessBoardManager.Instance.Pieces)// king forbidden moves because of the capture on them
		{
			if (p != null && p.isWhite != ChessBoardManager.Instance.isWhiteTurn)
			{
				forbiddenMoves = p.IsLegalMove();
				protectedMoves = p.IsPieceDefended();

				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						if (p.GetType() == typeof(Pawn))
						{
							if (p.kingCapture[i, j] == true && filtererd[i, j] == true)
							{
								filtererd[i, j] = false;
							}
							if (protectedMoves[i, j] == true && filtererd[i, j] == true)
							{
								filtererd[i, j] = false;
							}
						}
						else
						{
							if (forbiddenMoves[i, j] == true && filtererd[i, j] == true)
							{
								filtererd[i, j] = false;
							}
							if (protectedMoves[i, j] == true && filtererd[i, j] == true)
							{
								filtererd[i, j] = false;
							}
							if (ChessBoardManager.Instance.Pieces[x, y].isInitialMoveDone == false && filtererd[x + 1, j] == false)
							{
								if (filtererd[x + 1, j] == false)
								{
									filtererd[x + 2, j] = false;
								}
								if (filtererd[x - 1, j] == false || filtererd[x - 2, j] == false)
								{
									filtererd[x - 2, j] = false;
								}
							}
						}
					}

				}
			}

		}

		return filtererd;

	}

	public Tuple<int,int> GetKingPosition()
	{
		int x = -1;
		int y = -1;
		
		foreach(ChessPiece piece in ChessBoardManager.Instance.Pieces)
		{
			if(piece != null && piece.isWhite == ChessBoardManager.Instance.isWhiteTurn)
			{
				if(piece.GetType() == typeof(King))
				{
					x = piece.PositionX;
					y = piece.PositionY;
				}
			}
		}
		return Tuple.Create(x, y);
	}

	public bool SimulateKingMove()
	{
		var kingPos = GetKingPosition();
		// get king position, then
		// check his possible moves - if any possible return true
		// else - return false
		// used in checkmate check
		ChessPiece k = ChessBoardManager.Instance.Pieces[kingPos.Item1, kingPos.Item2];
		bool[,] a = k.IsLegalMove();
		bool[,] b = FilterForbiddenMoves(a, kingPos.Item1, kingPos.Item2);
		for(int i=0; i<b.GetLength(0); i++)
		{
			for(int j=0; j<b.GetLength(1); j++)
			{
				if (b[i, j] == true)
					return true;
			}
		}
		return false;
	}

}