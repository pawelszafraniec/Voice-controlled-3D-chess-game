using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	private void Start()
	{
		Instance = this;
		prefabs = new List<GameObject>();
	}

	public void ShowCheck(GameObject marking, int x, int y)
	{

		marking.SetActive(true);
		marking.transform.position = new Vector3(x + 0.25f, 1, y);
		marking.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);

	}

	public void ShowMate(GameObject markingWinning, int winningKingX, int winningKingY, GameObject markingLoosing, int loosingKingX, int loosingKingY)
	{
		markingWinning.SetActive(true);
		markingWinning.transform.position = new Vector3(winningKingX + 0.25f, 1, winningKingY);
		markingLoosing.SetActive(true);
		markingLoosing.transform.position = new Vector3(loosingKingX + 0.25f, 1, loosingKingY);

		if(ChessBoardManager.Instance.isWhiteTurn)
		{
			markingWinning.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
			markingLoosing.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
		}
		else
		{
			markingWinning.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
			markingLoosing.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
		}
	}

	public void ShowDraw(GameObject mark1, int kingAX, int kingAY, GameObject mark2, int kingBX, int kingBY)
	{
		mark1.SetActive(true);
		mark1.transform.position = new Vector3(kingAX + 0.25f, 1, kingAY);
		mark1.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);

		mark2.SetActive(true);
		mark2.transform.position = new Vector3(kingBX + 0.25f, 1, kingBY);
		mark2.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
	}

	public void HideCheck(GameObject marking)
	{
		marking.SetActive(false);
	}

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

	public void HideHighlights()
	{
		foreach(GameObject go in prefabs)
		{
			go.SetActive(false);
		}
	}

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
