using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{

    public static TutorialManager instance;
    [SerializeField]
    private GameObject handObject;
    [SerializeField]
    private Vector3 from, to;
    [SerializeField]
    private bool isUpdate = false;
    [SerializeField]
    private Transform []  highLightTutorial8x10;
    [SerializeField]
    private Transform[] highLightTutorial10x12;
    [SerializeField]
    private float smoothSpeed = 1;
    [SerializeField]
    private GridEffect gridEffect;
    private Vector3 velocity = Vector3.zero;
    private int countTutorial = 0;
    private BlockObj blockObj;

    public List<BlockObj> blockObjs = new List<BlockObj>();
    
    public List<Vector3> positionTo = new List<Vector3>();
    [SerializeField]
    private List<Button> buttonsDisable = new List<Button>();
    private void Awake()
    {
        instance = this;
    }

    //When in tutorial Mode Button in playing disable
    public void VisibleButton(bool visible)
    {
        for (int i = 0; i < buttonsDisable.Count; i++)
        {
            buttonsDisable[i].interactable = visible;
        }
    }

 

    public void ActiveHandMove(float waitTime)
    {
        Timer.Schedule(this, waitTime, () =>
        {
            Transform[] handPosition = PopupController.isBoard8x8 ? highLightTutorial8x10 : highLightTutorial10x12;
            this.blockObj = blockObjs[countTutorial];
            Vector3 position = blockObj.transform.position;
            position.x = position.x + ((float)blockObj.GetBlockType / 2);
            this.from = position;
            this.to = positionTo[countTutorial];
            blockObj.ActiveOutLineTutorial(true);
            isUpdate = true;
            handObject.SetActive(true);
            handObject.transform.position = from;
            gridEffect.ShowGridTutorial(blockObj, handPosition[countTutorial].position);
            countTutorial++;
        });


     
    }

    public void Reset()
    {
        handObject.SetActive(false);
        isUpdate = false;
        gridEffect.HideGrid(blockObj);
    }


    private void Update()
    {
        if (!isUpdate) return;
        handObject.transform.position = Vector3.SmoothDamp(handObject.transform.position, to, ref velocity, smoothSpeed);
        float dst = Vector3.Distance(handObject.transform.position, to);
        if (dst <= 0.5f)
        {
            handObject.transform.position = from;
        }
    }
}
