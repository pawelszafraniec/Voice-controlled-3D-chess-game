using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Class handling reading moves system
 */
public class AudioSourceManager : MonoBehaviour
{

	public Button controlButton;

	public List<AudioClip> Figureclips;
    public Dictionary<String, AudioClip> FiguresClips;

	public List<AudioClip> Positionclips;
	public Dictionary<String, AudioClip> PositionClips;

	public List<AudioClip> Additionalclips;
	public Dictionary<String, AudioClip> AdditionalClips;

	/**
	 * Method mapping figures audio clips
	 */
	private void MapFiguresToString()
	{
		FiguresClips = new Dictionary<string, AudioClip>();
		FiguresClips.Add("Pawn", Figureclips[0]);
		FiguresClips.Add("Bishop", Figureclips[1]);
		FiguresClips.Add("Knight", Figureclips[2]);
		FiguresClips.Add("Rook", Figureclips[3]);
		FiguresClips.Add("Queen", Figureclips[4]);
		FiguresClips.Add("King", Figureclips[5]);
	}

	/**
	 * Method mapping position audio clips
	 */
	private void MapPositionToString()
	{
		PositionClips = new Dictionary<string, AudioClip>();
		//A
		PositionClips.Add("A1", Positionclips[0]);
		PositionClips.Add("A2", Positionclips[1]);
		PositionClips.Add("A3", Positionclips[2]);
		PositionClips.Add("A4", Positionclips[3]);
		PositionClips.Add("A5", Positionclips[4]);
		PositionClips.Add("A6", Positionclips[5]);
		PositionClips.Add("A7", Positionclips[6]);
		PositionClips.Add("A8", Positionclips[7]);
		//B
		PositionClips.Add("B1", Positionclips[8]);
		PositionClips.Add("B2", Positionclips[9]);
		PositionClips.Add("B3", Positionclips[10]);
		PositionClips.Add("B4", Positionclips[11]);
		PositionClips.Add("B5", Positionclips[12]);
		PositionClips.Add("B6", Positionclips[13]);
		PositionClips.Add("B7", Positionclips[14]);
		PositionClips.Add("B8", Positionclips[15]);
		//C
		PositionClips.Add("C1", Positionclips[16]);
		PositionClips.Add("C2", Positionclips[17]);
		PositionClips.Add("C3", Positionclips[18]);
		PositionClips.Add("C4", Positionclips[19]);
		PositionClips.Add("C5", Positionclips[20]);
		PositionClips.Add("C6", Positionclips[21]);
		PositionClips.Add("C7", Positionclips[22]);
		PositionClips.Add("C8", Positionclips[23]);
		//D
		PositionClips.Add("D1", Positionclips[24]);
		PositionClips.Add("D2", Positionclips[25]);
		PositionClips.Add("D3", Positionclips[26]);
		PositionClips.Add("D4", Positionclips[27]);
		PositionClips.Add("D5", Positionclips[28]);
		PositionClips.Add("D6", Positionclips[29]);
		PositionClips.Add("D7", Positionclips[30]);
		PositionClips.Add("D8", Positionclips[31]);
		//E
		PositionClips.Add("E1", Positionclips[32]);
		PositionClips.Add("E2", Positionclips[33]);
		PositionClips.Add("E3", Positionclips[34]);
		PositionClips.Add("E4", Positionclips[35]);
		PositionClips.Add("E5", Positionclips[36]);
		PositionClips.Add("E6", Positionclips[37]);
		PositionClips.Add("E7", Positionclips[38]);
		PositionClips.Add("E8", Positionclips[39]);
		//F
		PositionClips.Add("F1", Positionclips[40]);
		PositionClips.Add("F2", Positionclips[41]);
		PositionClips.Add("F3", Positionclips[42]);
		PositionClips.Add("F4", Positionclips[43]);
		PositionClips.Add("F5", Positionclips[44]);
		PositionClips.Add("F6", Positionclips[45]);
		PositionClips.Add("F7", Positionclips[46]);
		PositionClips.Add("F8", Positionclips[47]);
		//G
		PositionClips.Add("G1", Positionclips[48]);
		PositionClips.Add("G2", Positionclips[49]);
		PositionClips.Add("G3", Positionclips[50]);
		PositionClips.Add("G4", Positionclips[51]);
		PositionClips.Add("G5", Positionclips[52]);
		PositionClips.Add("G6", Positionclips[53]);
		PositionClips.Add("G7", Positionclips[54]);
		PositionClips.Add("G8", Positionclips[55]);
		//H
		PositionClips.Add("H1", Positionclips[56]);
		PositionClips.Add("H2", Positionclips[57]);
		PositionClips.Add("H3", Positionclips[58]);
		PositionClips.Add("H4", Positionclips[59]);
		PositionClips.Add("H5", Positionclips[60]);
		PositionClips.Add("H6", Positionclips[61]);
		PositionClips.Add("H7", Positionclips[62]);
		PositionClips.Add("H8", Positionclips[63]);
	}

	/**
	 * Method mapping remaining ("special") audio clips
	 */
	private void MapAdditionalClips()
	{
		AdditionalClips = new Dictionary<string, AudioClip>();
		AdditionalClips.Add("check", Additionalclips[0]);
		AdditionalClips.Add("mateWhite", Additionalclips[1]);
		AdditionalClips.Add("mateBlack", Additionalclips[2]);
		AdditionalClips.Add("draw", Additionalclips[3]);
		AdditionalClips.Add("castle", Additionalclips[4]);
	}

	/**
	 * START method - runs when script is being enabled
	 */
	private void Start()
	{
		MapFiguresToString();
		MapPositionToString();
		MapAdditionalClips();
	}

	/**
	 * Method playing audio clip for a figure
	 */
	public void PlayFigure(string name)
	{
		gameObject.GetComponent<AudioSource>().clip = FiguresClips[name];
		gameObject.GetComponent<AudioSource>().Play();
	}

	/**
	 * Method playing audio clip for a position
	 */
	public void PlayPosition(string name)
	{
		gameObject.GetComponent<AudioSource>().clip = PositionClips[name];
		gameObject.GetComponent<AudioSource>().Play();

	}

	/**
	 * Coroutine reading move of a piece
	 */
	public IEnumerator ReadMove(string nameFig, string namePos)
	{
		gameObject.GetComponent<AudioSource>().clip = FiguresClips[nameFig];
		gameObject.GetComponent<AudioSource>().Play();

		yield return new WaitForSeconds(gameObject.GetComponent<AudioSource>().clip.length); // wait until previous one is finished

		gameObject.GetComponent<AudioSource>().clip = PositionClips[namePos];
		gameObject.GetComponent<AudioSource>().Play();

	}

	/**
	 * Coroutine reading checkmate
	 */
	public IEnumerator ReadMate(string nameFig, string namePos, string mate)
	{
		gameObject.GetComponent<AudioSource>().clip = FiguresClips[nameFig];
		gameObject.GetComponent<AudioSource>().Play();

		yield return new WaitUntil(() => gameObject.GetComponent<AudioSource>().isPlaying == false); // wait until previous audio clip is finished

		gameObject.GetComponent<AudioSource>().clip = PositionClips[namePos];
		gameObject.GetComponent<AudioSource>().Play();

		yield return new WaitUntil(() => gameObject.GetComponent<AudioSource>().isPlaying == false); // wait until previous audio clip is finished

		gameObject.GetComponent<AudioSource>().clip = AdditionalClips[mate];
		gameObject.GetComponent<AudioSource>().Play();
	}

	/**
	 * Corouting reading check state
	 */
	public IEnumerator ReadCheck(string nameFig, string namePos, string check)
	{
		gameObject.GetComponent<AudioSource>().clip = FiguresClips[nameFig];
		gameObject.GetComponent<AudioSource>().Play();

		yield return new WaitForSeconds(gameObject.GetComponent<AudioSource>().clip.length); // wait until previous audio clip is finished

		gameObject.GetComponent<AudioSource>().clip = PositionClips[namePos];
		gameObject.GetComponent<AudioSource>().Play();

		yield return new WaitForSeconds(gameObject.GetComponent<AudioSource>().clip.length); // wait until previous audio clip is finished

		gameObject.GetComponent<AudioSource>().clip = AdditionalClips[check];
		gameObject.GetComponent<AudioSource>().Play();
	}
	
	/**
	 * Coroutine reading draw
	 */
	public IEnumerator ReadDraw(string nameFig, string namePos, string draw)
	{
		gameObject.GetComponent<AudioSource>().clip = FiguresClips[nameFig];
		gameObject.GetComponent<AudioSource>().Play();

		yield return new WaitUntil(() => gameObject.GetComponent<AudioSource>().isPlaying == false); // wait until previous audio clip is finished

		gameObject.GetComponent<AudioSource>().clip = PositionClips[namePos];
		gameObject.GetComponent<AudioSource>().Play();

		yield return new WaitUntil(() => gameObject.GetComponent<AudioSource>().isPlaying == false); // wait until previous audio clip is finished

		gameObject.GetComponent<AudioSource>().clip = AdditionalClips[draw];
		gameObject.GetComponent<AudioSource>().Play();
	}

	/**
	 * Coroutine reading castling
	 */
	public IEnumerator ReadCastle(string nameFig, string namePos, string castle)
	{
		gameObject.GetComponent<AudioSource>().clip = FiguresClips[nameFig];
		gameObject.GetComponent<AudioSource>().Play();

		yield return new WaitUntil(() => gameObject.GetComponent<AudioSource>().isPlaying == false); // wait until previous audio clip is finished

		gameObject.GetComponent<AudioSource>().clip = PositionClips[namePos];
		gameObject.GetComponent<AudioSource>().Play();

		yield return new WaitUntil(() => gameObject.GetComponent<AudioSource>().isPlaying == false); // wait until previous audio clip is finished

		gameObject.GetComponent<AudioSource>().clip = AdditionalClips[castle];
		gameObject.GetComponent<AudioSource>().Play();
	}

	/**
	 * Method muting or enabling reading system during the game
	 */
	public void MuteOrEnableReadingMovesDuringGame()
	{
		if(controlButton.image.color == Color.red) // if inactive
		{
			// set active
			controlButton.image.color = Color.green;
			gameObject.GetComponent<AudioSource>().mute = false;

			 
		}
		else if (controlButton.image.color == Color.green) // if active
		{
			// set inactive
			controlButton.image.color = Color.red;
			gameObject.GetComponent<AudioSource>().mute = true;

		}
	}
}
