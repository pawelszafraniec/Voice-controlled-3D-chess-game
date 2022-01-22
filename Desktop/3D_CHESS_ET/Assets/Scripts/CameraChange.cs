using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    [SerializeField] Camera cameraMain;
    [SerializeField] Camera cameraSecond;

    [SerializeField] Light lightForMainCamera;
    [SerializeField] Light lightForSecondaryCamera;

    [SerializeField] GameObject whiteMarkings;
    [SerializeField] GameObject darkMarkings;

    void Start()
    {
        cameraMain.gameObject.SetActive(true);
        cameraSecond.gameObject.SetActive(false);
        cameraMain.enabled = true;
        cameraSecond.enabled = false;
    }

    void Update()
    {
        if(ChessBoardManager.Instance.isWhiteTurn)
		{
			cameraMain.gameObject.SetActive(true);
			cameraSecond.gameObject.SetActive(false);
			cameraMain.enabled = true;
            cameraSecond.enabled = false;


            
            lightForMainCamera.enabled = true;
            lightForSecondaryCamera.enabled = false;

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

            whiteMarkings.SetActive(false);
            darkMarkings.SetActive(true);
        }
    }
}
