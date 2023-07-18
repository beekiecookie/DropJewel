 
 
using System.Collections.Generic;
using System.Diagnostics;
 
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PopupController : MonoBehaviour
{
    public const int WIDTH_BOARD_1 = 8;

    public const int HEIGHT_BOARD_1 = 10;

    public const int WIDTH_BOARD_2 = 10;

    public const int HEIGHT_BOARD_2 = 12;

    public static bool isBoard8x8 = true;

    public static PopupController instance;




    [SerializeField]
    private SecondChance secondChancePopup;
    [SerializeField]
    private GameObject pausePopup;

    [SerializeField]
    private GameObject gameOverPopup;

    [SerializeField]
    private TextMeshProUGUI scoreGameOver;
    [SerializeField]
    private TextMeshProUGUI bestScoreGameOver;



    [SerializeField]
    private GameObject canvasMain;
 
  
 

    [SerializeField]
    private GameObject board8x8;
    [SerializeField]
    private GameObject board10x10;

    [SerializeField]
    private GameObject[] boardsSelected;

 

    private List<ISetUpWhenPlay> setUpWhenPlays = new List<ISetUpWhenPlay>();
    public bool watchAdSecondChance { get; set; }
    public bool secondChance { get; set; }
    private void Awake()
    {
        instance = this;
      
    }
  
    public void AddSetUpWhenPlay(ISetUpWhenPlay play)
    {
        setUpWhenPlays.Add(play);
    }
 

	 


    public void OnEventRestart()
    {
        if (Input.touchCount > 1) return;
        GoogleMobileAdsScript.instance.ShowInterstitial();
        BoosterController.instance.Restart();
 
        PlayingController.instance.RestartGame();
  
        secondChance = false;
    }


    //When click button replay game in playing disable button replay a short while
    public void OnEventButtonReplayInPlaying(Button obj)
    {
        if (PlayingController.instance.BlockDrag) return;
        obj.interactable = false;
       
        Timer.Schedule(this, 5f, () => {
            obj.interactable = true;
        });

    }

    public void OnEventPause(bool visible)
    {
        if (Input.touchCount>1) return;
        pausePopup.gameObject.SetActive(visible);
        PlayingController.instance.PauseGame(visible);
    }

    public void OnEventChangeBoard(bool Board8x8)
    {
        isBoard8x8 = Board8x8;
        if(isBoard8x8)
        {
            boardsSelected[0].SetActive(true);
            boardsSelected[1].SetActive(false);
        }
        else
        {
            boardsSelected[0].SetActive(false);
            boardsSelected[1].SetActive(true);
        }
    }

	public void GameOver()
	{
        bool watchAds = true; // GoogleMobileAdsScript.instance.CheckRewardBasedVideo(); @@¼öÁ¤
#if UNITY_EDITOR
        watchAds = true;
#endif
        if (watchAds && !secondChance  )
        {
            secondChance = true;
            secondChancePopup.gameObject.SetActive(true);
            secondChancePopup.Show();
        }
        else
        {
            PlayingController.instance.GameOver();
        }
	}

    public void ShowGameOverPopUp()
    {
        gameOverPopup.gameObject.SetActive(true);
        bestScoreGameOver.text = (isBoard8x8) ? DataInGame.BestScore8x8.ToString() : DataInGame.BestScore10x10.ToString();
        scoreGameOver.text = DataInGame.CurrentScore.ToString();
        SoundController.instance.GameOverSound();
    }

 

 

    public void OnEventStart()
    {
        if (GoogleMobileAdsScript.instance != null)
        {
            GoogleMobileAdsScript.instance.ShowInterstitial();
        }
        BlockController.instance.Reset();
        BoosterController.instance.Restart();
        canvasMain.SetActive(false);
        board10x10.SetActive(!isBoard8x8);
        board8x8.SetActive(isBoard8x8);
        PlayingController.gridWidth = (isBoard8x8) ? WIDTH_BOARD_1 : WIDTH_BOARD_2;
        PlayingController.gridHeight = (isBoard8x8) ? HEIGHT_BOARD_1 : HEIGHT_BOARD_2;
        watchAdSecondChance = false;
        for (int i = 0; i < setUpWhenPlays.Count; i++)
        {
            setUpWhenPlays[i].SetUp();
        }



        secondChance = false;
        PlayingController.instance.NewGame();

    }
 
 

    public void OnEventMainMenu()
    {
        canvasMain.SetActive(true);
        Time.timeScale = 1;
        PlayingController.instance.GoToMainMenu();
    }
 
    public void OnEventWatchAdsSecondChance()
    {
       
#if UNITY_EDITOR
        PlayingController.instance.SecondChance();
#else
        if (GoogleMobileAdsScript.instance.CheckRewardBasedVideo())
        {
            watchAdSecondChance =true ; 
            GoogleMobileAdsScript.instance.ShowRewardBasedVideo(()=> { PlayingController.instance.SecondChance(); });
        }
#endif
    }

    public void  OnEventNoTks()
    {
        PlayingController.instance.GameOver();
        
    }

}
