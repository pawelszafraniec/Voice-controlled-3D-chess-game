using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class handling camera rotation functionality
 */
public class CameraChange : MonoBehaviour
{
    [SerializeField] Camera cameraMain;
    [SerializeField] Camera cameraSecond;

    [SerializeField] Light lightForMainCamera;
    [SerializeField] Light lightForSecondaryCamera;

    [SerializeField] GameObject whiteMarkings;
    [SerializeField] GameObject darkMarkings;

    /**
     * START method - runs when script is being enabled
     */
    void Start()
    {
        //set default camera position
        cameraMain.gameObject.SetActive(true);
        cameraSecond.gameObject.SetActive(false);
        cameraMain.enabled = true;
        cameraSecond.enabled = false;
    }

    /**
     * UPDATE method - runs on each frame of the game
     */
    void Update()
    {
        // change camera position depending color that is currently moving

        if(ChessBoardManager.Instance.isWhiteTurn)
		{
			cameraMain.gameObject.SetActive(true);
			cameraSecond.gameObject.SetActive(false);
			cameraMain.enabled = true;
            cameraSecond.enabled = false;


            
            lightForMainCamera.enabled = true;
            lightForSecondaryCamera.enabled = false;

            // show field markings for white camera position
            whiteMarkings.SetActive(true);
            darkMarkings.SetActive(false);
        }
        else
		{
			cameraMain.gameObject.SetActive(false);
			cameraSecond.gameObject.SetActive(true);
			cameraMain.enabled = false;
            cameraSecond.enabled = true;


            lightForMainCamera.enabled = false;
            lightForSecondaryCamera.enabled = true;

            // show field markings for dark camera position
            whiteMarkings.SetActive(false);
            darkMarkings.SetActive(true);
        }
    }
}
