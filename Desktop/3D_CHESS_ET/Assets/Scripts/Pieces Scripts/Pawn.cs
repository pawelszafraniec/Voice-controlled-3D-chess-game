using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
	public override bool[,] IsLegalMove()
	{
		bool[,] move = new bool[8, 8];

		ChessPiece p1, p2, p3, p4;
		Array.Clear(kingCapture, 0, kingCapture.Length);
			
		if(isWhite)
		{
			//capture left
			if (PositionX != 0 && PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY + 1];
				if (p1 != null && !p1.isWhite)
				{
					move[PositionX - 1, PositionY + 1] = true;
				}
				if(p1 == null)
				{
					kingCapture[PositionX - 1, PositionY + 1] = true;
				}
			}

			//capture right
			if (PositionX != 7 && PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY + 1];
				if(p1 != null && !p1.isWhite)
				{
					move[PositionX + 1, PositionY + 1] = true;
				}
				if(p1 == null)
				{
					kingCapture[PositionX + 1, PositionY + 1] = true;
				}
			}

			//Forward move by 1
			if(PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX, PositionY + 1];
				if(p1 == null)
				{
					move[PositionX, PositionY + 1] = true;
				}
			}

			//Forward move by 2
			if(PositionY == 1)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX, PositionY + 1];
				p2 = ChessBoardManager.Instance.Pieces[PositionX, PositionY + 2];
				if(p1 == null && p2 == null)
				{
					move[PositionX, PositionY + 2] = true;


					//En passant check for a black pawn
					if (PositionX > 0 && PositionX < 7)
					{
						p3 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY + 2];
						p4 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY + 2];
						if (p3 != null)
						{
							p3.isEnPassantEnabledRight = true;
						}
						if (p4 != null)
						{
							p4.isEnPassantEnabledLeft = true;
						}

					}
					else if (PositionX == 0)
					{
						//prawy
						p3 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY + 2];
						if (p3 != null)
						{
							p3.isEnPassantEnabledRight = true;
						}

					}
					else if (PositionX == 7)
					{
						//lewy
						p4 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY + 2];
						if (p4 != null)
						{
							p4.isEnPassantEnabledLeft = true;
							
						}
					}
				}			
			}

			//en passant move right 
			if(PositionY == 4)
			{
				if (isEnPassantEnabledRight)
				{
					move[PositionX + 1, PositionY + 1] = true;
				}				
			}

			//en passant move left 
			if (PositionY == 4 && PositionX < 7)
			{
				if (isEnPassantEnabledLeft)
				{
					move[PositionX - 1, PositionY + 1] = true;
				}
			}

		}
		else 
		{
			//capture left
			if (PositionX != 7 && PositionY != 0)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY - 1];
				if (p1 != null && p1.isWhite)
				{
					move[PositionX + 1, PositionY - 1] = true;
				}
				if(p1 == null)
				{
					kingCapture[PositionX + 1, PositionY - 1] = true;
				}
			}

			//capture left
			if (PositionX != 0 && PositionY != 0)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY - 1];
				if (p1 != null && p1.isWhite)
				{
					move[PositionX - 1, PositionY - 1] = true;
				}
				if(p1 == null)
				{
					kingCapture[PositionX - 1, PositionY - 1] = true;

				}
			}

			//Forward move by 1
			if (PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX, PositionY - 1];
				if (p1 == null)
				{
					move[PositionX, PositionY - 1] = true;
				}
			}

			//Middle move by 2
			if (PositionY == 6)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX, PositionY - 1];
				p2 = ChessBoardManager.Instance.Pieces[PositionX, PositionY - 2];
				if (p1 == null && p2 == null)
				{
					move[PositionX, PositionY - 2] = true;

					//En passant check for a white pawn
					if (PositionX > 0 && PositionX < 7)
					{
						p3 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY - 2];
						p4 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY - 2];
						//left
						if (p3 != null)
						{
							p3.isEnPassantEnabledLeft = true;
						}
						//right
						if (p4 != null)
						{
							p4.isEnPassantEnabledRight = true;
						}

					}
					else if(PositionX == 0)
					{
						//left only
						p3 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY - 2];
						if (p3 != null)
						{
							p3.isEnPassantEnabledLeft = true;
						}

					}
					else if(PositionX == 7)
					{
						//right only
						p4 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY - 2];
						if (p4 != null)
						{
							p4.isEnPassantEnabledRight = true;
						}

					}
				}
			}

			//en passant move right 
			if (PositionY == 3)
			{
				if (isEnPassantEnabledRight)
				{
					move[PositionX - 1, PositionY - 1] = true;
				}
			}

			//en passant move left 
			if (PositionY == 3)
			{
				if (isEnPassantEnabledLeft)
				{
					move[PositionX + 1, PositionY - 1] = true;
				}
			}
		}

		return move;
	}
	public override bool[,] IsPieceDefended()
	{
		bool[,] defend = new bool[8, 8];

		ChessPiece p1;;

		if (isWhite)
		{
			//capture left
			if (PositionX != 0 && PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY + 1];
				if (p1 != null && p1.isWhite)
				{
					defend[PositionX - 1, PositionY + 1] = true;
				}
			}

			//capture right
			if (PositionX != 7 && PositionY != 7)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY + 1];
				if (p1 != null && p1.isWhite)
				{
					defend[PositionX + 1, PositionY + 1] = true;
				}
			}
		}
		else
		{
			//capture left
			if (PositionX != 7 && PositionY != 0)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX + 1, PositionY - 1];
				if (p1 != null && !p1.isWhite)
				{
					defend[PositionX + 1, PositionY - 1] = true;
				}
			}

			//capture left
			if (PositionX != 0 && PositionY != 0)
			{
				p1 = ChessBoardManager.Instance.Pieces[PositionX - 1, PositionY - 1];
				if (p1 != null && !p1.isWhite)
				{
					defend[PositionX - 1, PositionY - 1] = true;
				}
			}
		}


		return defend;

	}
}
