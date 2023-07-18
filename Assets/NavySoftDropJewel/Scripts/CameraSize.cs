using System;
using UnityEngine;

public class CameraSize : MonoBehaviour,ISetUpWhenPlay,IWindowChangeAction
{
    [SerializeField]
    private Camera cameraObj;
    [SerializeField]
    private GameObject backGround;
    [SerializeField]
    private float sizeCamera = 1;

    private float aspect = 1;
    private Vector3 originPosition;
    private Vector3 originBGPosition;
    private float sizeCameraInBoard10x12 = 0;

 

    private void Start()
    {
        originPosition = transform.position;
        originBGPosition = backGround.transform.position;
        PopupController.instance.AddSetUpWhenPlay(this);
        MultipleWindow.instance.AddWindowChange(this);
    }

    

    private void Initialize()
    {
        aspect = (float)Screen.height / (float)Screen.width;
        aspect = (float)Math.Round(aspect, 2);
        float size = sizeCamera;
        if (aspect>1.1f && aspect <= 1.5f)
        {
            size = (sizeCamera + aspect + 0.5f) -1;
        }
        else if(aspect <=1.1f)
        {
            size = sizeCamera + aspect + 0.5f;
            size += 2;
        }
        cameraObj.orthographicSize = aspect * size + sizeCameraInBoard10x12;
    }

    public void SetUp()
    {
        Vector3 position = originPosition;
        Vector3 positionBG = originBGPosition;
        sizeCameraInBoard10x12 = 0;
        if (!PopupController.isBoard8x8)
        {
            position.x = position.x + 1;
            position.y = position.y + .7f;
            positionBG.x = positionBG.x + 0.3f;
            sizeCameraInBoard10x12 = 2f;
        }

        transform.position = position;
        backGround.transform.position = positionBG;
        Initialize();
    }


    public void WindowChangeStart()
    {
        Initialize();
    }
  
    public void WindowChangeUpdate() { }
   
}
