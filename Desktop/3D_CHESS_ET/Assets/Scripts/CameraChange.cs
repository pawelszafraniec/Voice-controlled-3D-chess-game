using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    [SerializeField] Camera cameraMain;
    [SerializeField] Camera cameraSecond;
    // Start is called before the first frame update
    void Start()
    {
        cameraMain.gameObject.SetActive(true);
        cameraSecond.gameObject.SetActive(false);
        cameraMain.enabled = true;
        cameraSecond.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(ChessBoardManager.Instance.isWhiteTurn)
		{
            cameraMain.gameObject.SetActive(true);
            cameraSecond.gameObject.SetActive(false);
            cameraMain.enabled = true;
            cameraSecond.enabled = false;
        }
        else
		{
            cameraMain.gameObject.SetActive(false);
            cameraSecond.gameObject.SetActive(true);
            cameraMain.enabled = false;
            cameraSecond.enabled = true;
        }
    }
}
