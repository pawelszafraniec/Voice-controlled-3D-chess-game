using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class supporting the gameplay with addon functions
 */
public class BoardAddons : MonoBehaviour
{
	#region fields
	public static BoardAddons Instance { get; set; }
	public GameObject prefab;
	public GameObject marking;
	public GameObject markingCopy;

	#endregion
	#region structures
	private List<GameObject> prefabs;

	#endregion
	
	/**
	 * START method - runs when script is being enabled
	 */
	private void Start()
	{
		Instance = this;
		prefabs = new List<GameObject>();
	}

	/**
	 * Method showing check marking
	 */
	public void ShowCheck(GameObject marking, int x, int y)
	{

		marking.SetActive(true);
		marking.transform.position = new Vector3(x + 0.25f, 1, y); // set position
		marking.GetComponent<Renderer>().material.SetColor("_Color", Color.blue); // set color

	}
	
	/**
	 * Method showing checkmate markings
	 */
	public void ShowMate(GameObject markingWinning, int winningKingX, int winningKingY, GameObject markingLoosing, int loosingKingX, int loosingKingY)
	{
		markingWinning.SetActive(true);
		markingWinning.transform.position = new Vector3(winningKingX + 0.25f, 1, winningKingY);
		markingLoosing.SetActive(true);
		markingLoosing.transform.position = new Vector3(loosingKingX + 0.25f, 1, loosingKingY);

		if(ChessBoardManager.Instance.isWhiteTurn)
		{
			// set colors
			markingWinning.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
			markingLoosing.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
		}
		else
		{
			// set colors
			markingWinning.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
			markingLoosing.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
		}
	}

	/**
	 * Method showing draw markings
	 */
	public void ShowDraw(GameObject mark1, int kingAX, int kingAY, GameObject mark2, int kingBX, int kingBY)
	{
		mark1.SetActive(true);
		mark1.transform.position = new Vector3(kingAX + 0.25f, 1, kingAY);
		mark1.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);

		mark2.SetActive(true);
		mark2.transform.position = new Vector3(kingBX + 0.25f, 1, kingBY);
		mark2.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
	}

	/**
	 * Method hiding check marking
	 */
	public void HideCheck(GameObject marking)
	{
		marking.SetActive(false);
	}

	/**
	 * Method highlighting piece
	 */
	private GameObject HighlightPiece()
	{
		GameObject o = prefabs.Find(g => !g.activeSelf);

		if(o == null)
		{
			o = Instantiate(prefab);
			prefabs.Add(o);
		}
		return o;
	}

	/**
	 * Method highlighting allowed moves for a piece
	 */
	public void HighlightAllowedMoves(bool[,] moves)
	{
		for(int i=0; i<8; i++)
		{
			for(int j=0; j<8; j++)
			{
				if(moves[i,j])
				{
					GameObject ob = HighlightPiece();
					ob.SetActive(true);
					ob.transform.position = new Vector3(i+0.5f, 0, j+0.5f);
				}
			}
		}
	}

	/**
	 * Method hiding highlights for a piece
	 */
	public void HideHighlights()
	{
		foreach(GameObject go in prefabs)
		{
			go.SetActive(false);
		}
	}

	/**
	 * Method changing color of cube indicating turns
	 */
	public void ChangeTurnCubeColor(bool condition, GameObject gameObject)
	{
		var render = gameObject.GetComponent<Renderer>();
		if(condition)
		{
			render.material.SetColor("_Color", Color.black);
		}
		else
		{
			render.material.SetColor("_Color", Color.white);
		}
	}

	/**
	 * Method returning letter notation for a chess piece
	 */
	public string GetNotationForAPiece(string piece)
	{
		if(piece == "Pawn")
		{
			return "P";
		}
		else if(piece == "King")
		{
			return "K";
		}
		else if(piece == "Queen")
		{
			return "Q";
		}
		else if (piece == "Rook")
		{
			return "R";
		}
		else if (piece == "Bishop")
		{
			return "B";
		}
		else if (piece == "Knight")
		{
			return "N";
		}
		else
		{
			return "ERROR READING TYPE";
		}
	}
}
