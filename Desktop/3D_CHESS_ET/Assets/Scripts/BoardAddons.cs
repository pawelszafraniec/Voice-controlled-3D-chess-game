using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAddons : MonoBehaviour
{
	#region fields
	public static BoardAddons Instance { get; set; }
	public GameObject prefab;

	#endregion
	#region structures
	private List<GameObject> prefabs;

	#endregion
	private void Start()
	{
		Instance = this;
		prefabs = new List<GameObject>();
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
}
