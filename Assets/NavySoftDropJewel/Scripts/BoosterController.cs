using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BoosterController : MonoBehaviour,ISetUpWhenPlay
{
    public static BoosterController instance;

    private const int maxAdsGetBoosterOneGame = 2;
    [System.Serializable]
    public class BoosterInfor
    {
        public string nameBooster;
        public int numberBoosterStartGame = 1;
        public int numberTakeAds = 1;
    }

    [System.Serializable]
    public class HintInfor
    {
        public BoosterType boosterType;
        public int start = 1;

        public int current = 0;
        public int adsCanGet = 0;
        public void SaveHintInfor()
        {
            PlayerPrefs.SetInt(boosterType.ToString(), current);
        }
        public void LoadHintInfor()
        {
            current = PlayerPrefs.GetInt(boosterType.ToString(), start);
        }
    }

    [System.Serializable]
    public enum BoosterType
    {
        NONE = 0,
        ONE_BLOCK_DESTROY,
        ONE_ROW_DESTROY,
        CUSTOM_MOVE_BLOCK,
    }

    [SerializeField]
    private BoosterType boosterType = BoosterType.NONE;

    [SerializeField]
    private List<HintInfor> hintInfors = new List<HintInfor>();
    [SerializeField]
    private GameObject boardBoosterSelectedBlock;
    [SerializeField]
    private TextMeshProUGUI textBoosterOneBlock;
    [SerializeField]
    private TextMeshProUGUI textBoosterOneRow;
    [SerializeField]
    private TextMeshProUGUI textBoosterCustomBlock;
 


    [SerializeField]
    private RectTransform[] rectButtonsBooster;

    [Header("Hammer Booster")]
    [SerializeField]
    private GameObject hammerObj;
    [SerializeField]
    private Animation hammerAnimation;
    [Header("Sword Booster")]
    [SerializeField]
    private Animation swordAnimation;
    [SerializeField]
    private GameObject swordObj;
    [SerializeField]
    private MakeBoardLevel board8x10;
    [SerializeField]
    private MakeBoardLevel board10x12;
    [SerializeField]
    private List<GridList> gridInGame = new List<GridList>();
    [SerializeField]
    private bool testMode;
    private GridEffect gridEffect;
    private Vector2 mousePosition;
    private int currentBooster = -1;
    public GameObject GetSwordObj { get { return swordAnimation.gameObject; } }
    public GameObject destroyObj { get; set; }
    [SerializeField]
    private List<Vector3> gridsPosition = new List<Vector3>();

    private MakeBoardLevel GetCurrentBoard
    {
        get

        {
            return (PopupController.isBoard8x8) ? board8x10 : board10x12;
        }
    }


  
    

    public int RowPosition {
        get
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Debug.Log("MousePosition : "+( Mathf.FloorToInt(mousePosition.y) == -1 ? 0 : Mathf.FloorToInt(mousePosition.y)));
            return Mathf.RoundToInt( mousePosition.y) == -1 ? 0 : Mathf.RoundToInt(mousePosition.y);
        }
    }

    public BoosterType GetBoosterType { get { return boosterType; } }
    public void SetBoosterType(BoosterType boosterType) {
        this.boosterType = boosterType;
        if (boosterType == BoosterType.NONE)
        {
            boardBoosterSelectedBlock.SetActive(false);
            for (int i = 0; i < rectButtonsBooster.Length; i++)
            {
                AnimationScale(rectButtonsBooster[i], false);
            }
           
            destroyObj = null;
        }


    }
    private void Awake()
    {
        instance = this;
    }

  
  
    private void Start()
    {
        if (testMode)
        {
            for (int i = 0; i < hintInfors.Count; i++)
            {
                hintInfors[i].start = 10000;
            }
        }


        //update remain number booster
        for (int i = 0; i < hintInfors.Count; i++)
        {
            hintInfors[i].LoadHintInfor();
        }

        UpdateTexNumberBooster();

        gridEffect = GetComponent<GridEffect>();
        PopupController.instance.AddSetUpWhenPlay(this);
    }


    public void SetUp()
    {
        Timer.Schedule(this, 0.1f, () => {

            int width = PlayingController.gridWidth;
            int height = PlayingController.gridHeight;

            for (int i = 0; i < hintInfors.Count; i++)
            {
                hintInfors[i].adsCanGet = maxAdsGetBoosterOneGame;
            }

            gridInGame.Clear();
            for (int y = 0; y < height; y++)
            {
                GridList grids = new GridList();
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width) + x;

                  

                    grids.gridList.Add(GetCurrentBoard.GetBlocks[index].transform);
                }
                gridInGame.Add(grids);
            }

        });
     


    }
    private void UpdateTexNumberBooster()
    {
        textBoosterOneBlock.text = (hintInfors[0].current==0)? string.Format("AD + {0}", hintInfors[0].adsCanGet.ToString()) : string.Format("x{0}", hintInfors[0].current.ToString())  ;
        textBoosterOneRow.text = (hintInfors[1].current == 0) ? string.Format("AD + {0}", hintInfors[1].adsCanGet.ToString()) : string.Format("x{0}", hintInfors[1].current.ToString());
        textBoosterCustomBlock.text = (hintInfors[2].current == 0) ? string.Format("AD + {0}", hintInfors[2].adsCanGet.ToString()) : string.Format("x{0}", hintInfors[2].current.ToString());
       
    }

  

    //Show grid can move when select booster first
    public void ShowGrid(BlockObj block)
    {
        int width = PlayingController.gridWidth;
        int height = PlayingController.gridHeight;
        int blockWidth = block.GetBlockType;
        gridsPosition.Clear();
        for (int y = 0; y < height; y++)
        {
            int count = 0;
            Vector3 positionPlaceGrid = Vector3.zero;
            for (int x = 0; x < width; x++)
            {
                if (PlayingController.instance.gridInGame[y].gridList[x] != null)
                {
                    count = 0;
                }
                else
                {
                    if (count == 0)
                    {
                        positionPlaceGrid = gridInGame[y].gridList[x].position;
                    }
                    count++;
                    if (count >= blockWidth)
                    {
                        count = 0;
                        gridsPosition.Add(positionPlaceGrid);
                    }
                }

               
            }

        }

        gridEffect.ShowGrid(block, gridsPosition);
    }

    public void HideGrid(BlockObj block)
    {
        gridEffect.HideGrid(block);
    }


    //show effect of booster two
    public void EffectOneBlockDestroy(Vector3 position)
    {
        position.x += .5f;
        hammerObj.transform.position = position;
        hammerObj.SetActive(true);
        hammerAnimation.Play();
    }
    //show effect of booster three
    public void EffectOneRowDestroy(Vector3 position)
    {
        
        swordObj.transform.position = position;
        swordObj.SetActive(true);
        swordAnimation.Play();
    }

    public void Restart()
    {
        boosterType = BoosterType.NONE;
        for (int i = 0; i < hintInfors.Count; i++)
        {
            hintInfors[i].adsCanGet = maxAdsGetBoosterOneGame;
        }
        UpdateTexNumberBooster();
    }


    public void CompleteUsedBooster(int Booster)
    {
        AnimationScale(rectButtonsBooster[currentBooster], false);
        if (Booster == 0) return;
       
        Booster -= 1;
        hintInfors[Booster].current--;
        hintInfors[Booster].SaveHintInfor();
        UpdateTexNumberBooster();

    }

  

    public void OnEventUseBooster(int booster)
    {
        if (Input.touchCount > 1) return;
        if (currentBooster>=0)
        AnimationScale(rectButtonsBooster[currentBooster], false);

        booster -= 1;
        currentBooster = booster;
        if (hintInfors[booster].adsCanGet <= 0 && hintInfors[booster].current <=0) return;
        //Reset Scale
        switch (booster)
        {
            case 0:
                if (hintInfors[booster].current > 0)
                {
                    SetBoosterType(BoosterType.ONE_BLOCK_DESTROY);
                    AnimationScale(rectButtonsBooster[currentBooster], true);
                }
                else
                {
#if UNITY_EDITOR
                    AddHint(booster);
#endif
                    //Watch Ads Get Rewards
                    GoogleMobileAdsScript.instance.ShowRewardBasedVideo(() => { AddHint(booster); });

                }
                break;
            case 1:
                if (hintInfors[booster].current > 0)
                {
                    SetBoosterType(BoosterType.ONE_ROW_DESTROY);
                    AnimationScale(rectButtonsBooster[currentBooster], true);
                }
                else
                {
#if UNITY_EDITOR
                    AddHint(booster);
#endif
                    //Watch Ads Get Rewards
                    GoogleMobileAdsScript.instance.ShowRewardBasedVideo(() => { AddHint(booster); });
                }
                break;
            case 2:
                if (hintInfors[booster].current > 0)
                {
                    SetBoosterType(BoosterType.CUSTOM_MOVE_BLOCK);
                    boardBoosterSelectedBlock.SetActive(true);
                    AnimationScale(rectButtonsBooster[currentBooster], true);
                }
                else
                {
#if UNITY_EDITOR
                    AddHint(booster);
#endif
                    //Watch Ads Get Rewards
                    GoogleMobileAdsScript.instance.ShowRewardBasedVideo(() => { AddHint(booster); });
                }
                break;
  
        }
    }

    //Effect when select booster
    private void AnimationScale(RectTransform rect,bool scale)
    {
        float scaleValue = (scale) ? 1.12f : 1f;
        rect.transform.GetChild(0).gameObject.SetActive(scale);
        UIAnimation scaleX = UIAnimation.ScaleX(rect, scaleValue, 0.2f);
        UIAnimation scaleY = UIAnimation.ScaleY(rect, scaleValue, 0.2f);
        scaleX.Play();
        scaleY.Play();
    }

    private void AddHint(int booster)
    {
        hintInfors[booster].adsCanGet--;
        hintInfors[booster].current++;
        hintInfors[booster].SaveHintInfor();
        UpdateTexNumberBooster();
    }

   

}
