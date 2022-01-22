using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
	public King() : base()
	{

	}
	
	protected King(King k) : base(k)
	{

	}

	public override ChessPiece Clone()
	{
		var piece = gameObject.AddComponent<King>();
		piece.SetValues(this);
		return piece;
	}

	public override bool[,] IsLegalMove()
	{
		bool[,] move = new bool[8, 8];

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

		foreach (ChessPiece p in ChessBoardManager.Instance.Pieces)// king forbidden moves because of the capture on him
		{
			if (p != null && p.isWhite != ChessBoardManager.Instance.isWhiteTurn) // for every piece of opposite side
			{
				forbiddenMoves = p.IsLegalMove(); // possible moves
				protectedMoves = p.IsPieceDefended(); // pieces protected - cannot be captured then

				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						if (p.GetType() == typeof(Pawn)) // for pawns' captures
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
						else // other pieces all possible moves
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
	public bool[,] FilterMovesForDraw(bool[,] allowedByCore, int x, int y)
	{
		bool[,] filtererd = allowedByCore;
		bool[,] forbiddenMoves;
		bool[,] protectedMoves;

		foreach (ChessPiece p in ChessBoardManager.Instance.Pieces)// king forbidden moves because of the capture on him
		{
			if (p != null && p.isWhite == ChessBoardManager.Instance.isWhiteTurn) // for every piece of opposite side
			{
				forbiddenMoves = p.IsLegalMove(); // possible moves
				protectedMoves = p.IsPieceDefended(); // pieces protected - cannot be captured then

				for (int i = 0; i < 8; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						if (p.GetType() == typeof(Pawn)) // for pawns' captures
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
						else // other pieces all possible moves
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

	public Tuple<int,int> GetKingPosition() // king position for color which is moving
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
	public Tuple<int, int> GetOppositeKingPosition() // king position for opposite color
	{
		int x = -1;
		int y = -1;

		foreach (ChessPiece piece in ChessBoardManager.Instance.Pieces)
		{
			if (piece != null && piece.isWhite != ChessBoardManager.Instance.isWhiteTurn)
			{
				if (piece.GetType() == typeof(King))
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