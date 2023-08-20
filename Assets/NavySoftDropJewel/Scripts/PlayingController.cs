 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public enum GameState : byte
{
    NONE,
    WATING,
    PLAY,
    OTHER_SCREEN
}

[System.Serializable]
public enum BlockState : byte
{
    NONE,
    MOVE_NEXT_LINE,
    FALL,
    DELETE_LINE,
    RESTART_GAME,

    SECOND_CHANCE,
    GAME_OVER,
    TUTORIAL,
    SHOW_GAME_OVER_POPUP

}

[System.Serializable]
public class GridList
{
    public List<Transform> gridList = new List<Transform>();
}
public class PlayingController : MonoBehaviour, ISetUpWhenPlay
{
    public static PlayingController instance;

    public static int gridWidth = 8;

    public static int gridHeight = 12;

    public PopupController screen_manager;

    public List<GridList> gridInGame = new List<GridList>();

    [SerializeField]
    private TextMeshProUGUI bestScoreText;
    [SerializeField]
    private TextMeshProUGUI currentScoreText;
    [SerializeField]
    private AnimationCurve anicurve;

    [SerializeField]
    private GameObject warningArea8x10;
    [SerializeField]
    private GameObject warningArea10x12;

    [SerializeField]
    private Transform MoveGuide_Board_1;
    [SerializeField]
    private Transform MoveGuide_Board_2;


    [Space()]
    [Header("Show Infor When Game Playing")]

    [SerializeField]
    private GameState currentGameState = GameState.NONE;
    [SerializeField]
    private BlockState blockState = BlockState.NONE;
    [SerializeField]
    private bool destroyLineInBoard = false;
    [SerializeField]
    private bool blockFallInBoard = false;
    [SerializeField]
    private int blockMovePointMax = gridWidth;
    [SerializeField]
    private int blockMovePointMin;
    [SerializeField]
    private int highScore;
    [SerializeField]
    private int currentScore;
    [SerializeField]
    private List<BlockContainer> BlocksInGame = new List<BlockContainer>();
    [SerializeField]
    private List<string> actions = new List<string>();

    [SerializeField]
    private bool finishStateInBoard = false;
    [SerializeField]
    private bool pauseGame = false;
    private Transform MoveGuide;
    private bool blockDrag;
    private GameObject selectedBlock;
    private bool tutorialStep2;
    private bool prevUpdateBlockState = false;
    private Vector2 FirstSelectPoint;
    private Vector2 FirstBlockPoint;
    private float waitTimeNextLine = 0.1f;
    private GameObject warningArea;
    private int comboBlockLine;
    private int prevComboScore;
    private Vector3 originPositionBlock = Vector3.zero;
    private Vector3 selectedPositionBlock = Vector3.zero;
    private GameObject shadowBlock;
  
    private bool customPositionBooster = false;
    public bool BlockDrag { get { return blockDrag; } }
    [System.Serializable]
    public class BlockContainer
    {
        public List<BlockObj> blocks = new List<BlockObj>();
    }

    #region Unity Method
    private void Awake()
    {
        Application.targetFrameRate = 60;
        instance = this;
    }

    private void Update()
    {
        SelectedAndDragBlock();
    }
    private void Start()
    {
        PopupController.instance.AddSetUpWhenPlay(this);
        currentGameState = GameState.NONE;
    }
    #endregion

    public void SetUp()
    {
        blockMovePointMax = gridWidth;

        gridInGame.Clear();
        for (int x = 0; x < gridHeight; x++)
        {
            GridList grids = new GridList();
            for (int y = 0; y < gridWidth; y++)
            {
                grids.gridList.Add(null);
            }
            gridInGame.Add(grids);
        }

        MoveGuide_Board_1.gameObject.SetActive(false);
        MoveGuide_Board_2.gameObject.SetActive(false);
        MoveGuide = (PopupController.isBoard8x8) ? MoveGuide_Board_1 : MoveGuide_Board_2;
        bestScoreText.text = (PopupController.isBoard8x8) ? DataInGame.BestScore8x8.ToString() : DataInGame.BestScore10x10.ToString();
        highScore = (PopupController.isBoard8x8) ? DataInGame.BestScore8x8 : DataInGame.BestScore10x10;
        warningArea = (PopupController.isBoard8x8) ? warningArea8x10 : warningArea10x12;

         
    }


    public void SetStateGame(GameState state)
    {
        this.currentGameState = state;
    }
    private void SetBlockState(BlockState blockState)
    {
        if (prevUpdateBlockState) return;
        this.blockState = blockState;
#if UNITY_EDITOR
        actions.Add(blockState.ToString());
#endif
    }

    /// <summary>
    /// Method Controller All Block In Playing
    ///  Step of Method
    ///  1.Begin Game Move 4 Line In Board
    ///  2. Loop 2 case :
    ///         Fall ------> delete_line
    ///          ^              |
    ///          |              |
    ///          |              v
    ///   delete_line <------- Fall
    ///  3.if no block can fall player can move block
    ///  4.if player finish move block repeat step 2
    ///  5.Finish Step 2 move 1 next line 
    ///  6.if lineInBoard <2 move continue when has 2 line in board    
    ///  
    /// </summary>
    private IEnumerator ManualUpdate()
    {
        while (!pauseGame)
        {

            yield return new WaitForEndOfFrame();
            switch (blockState)
            {
                case BlockState.FALL:
                    yield return new WaitForEndOfFrame();
                    BlockFall();
                    blockState = BlockState.NONE;

                    if (blockFallInBoard || customPositionBooster)
                    {

                        yield return new WaitForSeconds(.3f);

                        customPositionBooster = false;

                        SetBlockState(BlockState.DELETE_LINE);

                    }
                    else
                    {

                        if (finishStateInBoard)
                        {
                            blockState = BlockState.NONE;
                            //Reset Combo Score
                            if (prevComboScore != comboBlockLine)
                            {
                                comboBlockLine = 0;
                            }


                            if (HasObjLimitHeight())
                            {
                                VisibleWarningArea(false);
                                SetBlockState(BlockState.GAME_OVER);
                                SetStateGame(GameState.WATING);
                            }
                            else
                            {
                                if (tutorialStep2)
                                {
#if UNITY_EDITOR
                                    actions.Add("Tutorial");
#endif
                                    SetBlockState(BlockState.TUTORIAL);
                                    tutorialStep2 = false;
                                }
                                else
                                {
                                    int nextLine = GetLineWhenBoardEmpty();
                                    if (nextLine != 0)
                                    {
                                        for (int i = 0; i < nextLine; i++)
                                        {
                                            yield return new WaitForSeconds(.3f);
                                            SetBlockState(BlockState.MOVE_NEXT_LINE);
                                        }

                                    }
                                    else
                                    {
                                        VisibleWarningArea(HasObjLimitHeight(2));
                                        //Debug.Log("Play State Game");
                                        SetStateGame(GameState.PLAY);
                                        yield break;
                                    }
                                }


                            }
                        }
                        else
                        {
                            SetBlockState(BlockState.MOVE_NEXT_LINE);
                        }

                    }




                    break;
                case BlockState.DELETE_LINE:
                    DeleteLine();
                    blockState = BlockState.NONE;

                    float waitTime = 0.04f;
                    if (destroyLineInBoard)
                    {
                        waitTime = waitTimeNextLine;
                    }

                    yield return new WaitForSeconds(waitTime);
                    SetBlockState(BlockState.FALL);

                    break;

                case BlockState.MOVE_NEXT_LINE:

                    yield return new WaitForSeconds(.04f);
                    MoveNextLine();
                    blockState = BlockState.NONE;
                    yield return new WaitForSeconds(.3f);
                    SetBlockState(BlockState.FALL);
                    finishStateInBoard = true;

                    break;
                case BlockState.TUTORIAL:

                    StartCoroutine(CreateBlockTutorialStep2());

                    blockState = BlockState.NONE;
                    break;

                case BlockState.RESTART_GAME:

                    StartCoroutine(DeleteAllBlock(.1f));

                   
                    yield return new WaitForSeconds(2f);
                    
                    ClearAllDataBeginGame(true);
                    
                    break;

                case BlockState.SECOND_CHANCE:

                    StartCoroutine(DeleteAllBlock(.1f, gridWidth / 2));

                    blockState = BlockState.NONE;
                    yield return new WaitForSeconds(.8f);
                    UpdateAllGridInGame(.02f);
                    yield return new WaitForSeconds(.2f);
                    SetStateGame(GameState.PLAY);
                    TutorialManager.instance.VisibleButton(true);



                    break;
                case BlockState.GAME_OVER:
                    PopupController.instance.GameOver();
                    blockState = BlockState.NONE;
                    break;

            }
        }
    }


    private Coroutine coroutineUpdate = null;
    private void ResetManualUpdate()
    {
        if (coroutineUpdate != null)
            StopCoroutine(coroutineUpdate);
        coroutineUpdate = StartCoroutine(ManualUpdate());
    }


    public void SecondChance()
    {
        SetBlockState(BlockState.SECOND_CHANCE);
    }

    public void PauseGame(bool visible)
    {
        pauseGame = visible;

        if (pauseGame)
        {
            Time.timeScale = 1;
            Timer.Schedule(this, 0.5f, () => {
                Time.timeScale = 0;
            });

            SetStateGame(GameState.WATING);
        }
        else
        {
            Time.timeScale = 1;
            SetStateGame(GameState.PLAY);
        }
    }
    public void NewGame()
    {

        ClearAllDataBeginGame(true);
    }

    public void GameOver()
    {
       

        StartCoroutine(DeleteAllBlock(0.02f));

        Timer.Schedule(this, 2f, () =>
        {
            PopupController.instance.ShowGameOverPopUp();

        });




    }


    public void GoToMainMenu()
    {
        SetStateGame(GameState.OTHER_SCREEN);
        Timer.Schedule(this, 0.15f, () =>
        {

            SetBlockState(BlockState.NONE);
            StopCoroutine(ManualUpdate());
            StartCoroutine(DeleteAllBlock(0.02f,0,false));
            ClearAllDataBeginGame(false);
        });

    }

    public void RestartGame()
    {
        SetStateGame(GameState.OTHER_SCREEN);
        ResetManualUpdate();
        SetBlockState(BlockState.RESTART_GAME);
        prevUpdateBlockState = true;
    }


    public void RemoveBlockInBoard(int x, BlockObj block)
    {
        if (x > BlocksInGame.Count - 1) return;
        BlocksInGame[x].blocks.Remove(block);
        if (BlocksInGame[x].blocks.Count == 0)
        {
            BlocksInGame.RemoveAt(x);
        }
    }

    public void ClearAllDataBeginGame(bool newGame, bool resetScore = true)
    {
        BlockController.instance.Reset();
        BoosterController.instance.Restart();
        prevUpdateBlockState = false;
        tutorialStep2 = false;
        pauseGame = false;
        currentGameState = GameState.NONE;
        blockState = BlockState.NONE;
        selectedBlock = null;
        FirstSelectPoint = Vector2.zero;
        FirstBlockPoint = Vector2.zero;
        blockMovePointMax = gridWidth;
        blockMovePointMin = 0;
        warningArea10x12.gameObject.SetActive(false);
        warningArea8x10.gameObject.SetActive(false);
        if (resetScore)
        {
            currentScore = 0;
            comboBlockLine = 0;
            highScore = (PopupController.isBoard8x8) ? DataInGame.BestScore8x8 : DataInGame.BestScore10x10;
            DataInGame.CurrentScore = 0;
            bestScoreText.text = (PopupController.isBoard8x8) ? DataInGame.BestScore8x8.ToString() : DataInGame.BestScore10x10.ToString();
            currentScoreText.text = currentScore.ToString();
        }
        Destroy(shadowBlock);
        shadowBlock = null;


        //Clear All Block When Begin Game
        for (int i = 0; i < BlocksInGame.Count; i++)
        {
            for (int j = 0; j < BlocksInGame[i].blocks.Count; j++)
            {
                if (BlocksInGame[i].blocks[j] != null)
                {
                    Destroy(BlocksInGame[i].blocks[j].gameObject);
                }
            }
        }

        BlocksInGame.Clear();



        if (newGame)
        {

            StartCoroutine(CreateBlockBeginGame());
        }
    }

    /// <summary>
    /// Check Block Line In Board
    /// </summary>

    private int GetLineWhenBoardEmpty()
    {
        for (int i = 2; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                if (gridInGame[i].gridList[j] != null)
                {
                    return 0;
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            bool lineEmpty = true;
            for (int j = 0; j < gridWidth; j++)
            {
                if (gridInGame[i].gridList[j] != null)
                {
                    lineEmpty = false;
                }
            }
            if (lineEmpty)
            {
                return (2 - i);
            }
        }

        return 0;
    }

    private bool HasObjLimitHeight(int height = 1)
    {
        for (int i = 0; i < gridInGame[gridHeight - height].gridList.Count; i++)
        {
            if (gridInGame[gridHeight - height].gridList[i] != null) return true;
        }

        return false;
    }

    private void SelectedAndDragBlock()
    {
        Vector2 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Ray2D ray2D = new Ray2D(vector, Vector2.zero);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(ray2D.origin, ray2D.direction, 0f, 1);
        bool useBooster = false;

      
        if(Input.touchCount>1) return;


        if (Input.GetMouseButtonDown(0))
        {

            if (selectedBlock != null || currentGameState != GameState.PLAY)
            {
                return;
            }




            if (raycastHit2D.collider != null && raycastHit2D.transform.gameObject.tag == "Block" && raycastHit2D.transform.position.y >= 0f)
            {
                FirstSelectPoint = vector;
                blockDrag = true;
                selectedBlock = raycastHit2D.transform.parent.parent.gameObject;
                originPositionBlock = selectedBlock.transform.position;
                BlockObj blockObj = selectedBlock.GetComponent<BlockObj>();

                if (BoosterController.instance.GetBoosterType == BoosterController.BoosterType.ONE_BLOCK_DESTROY ||
                    BoosterController.instance.GetBoosterType == BoosterController.BoosterType.ONE_ROW_DESTROY)
                {
                    BoosterController.instance.destroyObj = selectedBlock;

                    useBooster = true;
                    if (BoosterController.instance.RowPosition >= 0)
                    {

                        selectedBlock = null;
                        blockDrag = false;
                        SetBlockState(BlockState.DELETE_LINE);
                        ResetManualUpdate();
                        finishStateInBoard = false;
                        SetStateGame(GameState.WATING);

                    }
                    else
                    {
                        BoosterController.instance.SetBoosterType(BoosterController.BoosterType.NONE);
                    }
                    return;
                }
                if ( !blockObj.canMoveBlock)//blockObj.hasBlockChain ||
                {
                    selectedBlock = null;
                    blockDrag = false;
                    return;
                }
                else
                {

                    if (BoosterController.instance.GetBoosterType == BoosterController.BoosterType.CUSTOM_MOVE_BLOCK)
                    {
                        BoosterController.instance.ShowGrid(blockObj);
                    }
                }
                FirstBlockPoint = selectedBlock.transform.position;


                shadowBlock = Instantiate(selectedBlock, selectedBlock.transform.position, selectedBlock.transform.rotation);
                BlockObj fakeBlock = shadowBlock.GetComponent<BlockObj>();
                fakeBlock.ActiveOutLineTutorial(false);
                LeanTween.alpha(shadowBlock.transform.Find("BlockImage").gameObject, .4f, 0f)  ;
 
               // shadowBlock.transform.Find("BlockImage").GetComponent<SpriteRenderer>().sortingOrder = 16;

                MoveGuide.position = new Vector3(selectedBlock.transform.position.x, MoveGuide.position.y, 0f);
                MoveGuide.GetChild(0).localScale = new Vector3((float)blockObj.GetBlockType, MoveGuide.GetChild(0).localScale.y, 0f);
                MoveGuide.GetChild(0).localPosition = new Vector3((float)(blockObj.GetBlockType - 1) * 0.5f, MoveGuide.GetChild(0).localPosition.y, 0f);
                MoveGuide.gameObject.SetActive(true);



                //Get Point Block Can Move In Line
                blockMovePointMax = blockObj.PointCanMoveInLine(true);
                blockMovePointMin = blockObj.PointCanMoveInLine(false);

            }

        }



        if (blockDrag && selectedBlock != null)
        {

            float x = (FirstSelectPoint - vector).x;
            BlockObj blockObj = selectedBlock.GetComponent<BlockObj>();

            float y = selectedBlock.transform.position.y;
            int positionY = Mathf.RoundToInt(y);
            int blockRange = blockObj.GetBlockRange;
            if (BoosterController.instance.GetBoosterType == BoosterController.BoosterType.CUSTOM_MOVE_BLOCK)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                y = Mathf.Clamp(pos.y, 0, gridHeight - 1);
                positionY = Mathf.RoundToInt(y);
                blockRange = 0;
                blockObj.SetSortingOrder(7);
                blockMovePointMin = 0;
                blockMovePointMax = gridWidth - blockObj.GetBlockType;
            }

            selectedPositionBlock = new Vector3(FirstBlockPoint.x - x, positionY, 0f);


            selectedBlock.transform.position = new Vector3(FirstBlockPoint.x - x, y, 0f);
            if (selectedBlock.transform.position.x < (float)blockMovePointMin)
            {
                selectedPositionBlock = new Vector3((float)blockMovePointMin, y, 0f);
                selectedBlock.transform.position = new Vector3((float)blockMovePointMin, y, 0f);
                // UnityEngine.//Debug.Log("Drag Min : " + blockMovePointMin);
            }
            else if (selectedBlock.transform.position.x > (float)(blockMovePointMax))
            {
                selectedPositionBlock = new Vector3((float)(blockMovePointMax), y, 0f);
                selectedBlock.transform.position = new Vector3((float)(blockMovePointMax), y, 0f);
                // UnityEngine.//Debug.Log("Drag Max : " + (blockMovePointMax - blockRange));
            }
            MoveGuide.position = new Vector3(selectedBlock.transform.position.x, MoveGuide.position.y, 0f);

        }




        if (Input.GetMouseButtonUp(0) && !useBooster)
        {

            blockDrag = false;
            if (MoveGuide != null)
                MoveGuide.gameObject.SetActive(false);
            if (selectedBlock == null || currentGameState != GameState.PLAY)
            {
                return;
            }
            Destroy(shadowBlock);
            shadowBlock = null;

            Vector3 position = new Vector3(selectedPositionBlock.x, selectedPositionBlock.y, 0);
            bool setPosition = true;

            BlockObj blockObj = selectedBlock.GetComponent<BlockObj>();





            if (BoosterController.instance.GetBoosterType == BoosterController.BoosterType.CUSTOM_MOVE_BLOCK)
            {
                BoosterController.instance.HideGrid(blockObj);
                setPosition = blockObj.CanPlaceBlock(position);
            }




            if (setPosition)
            {
                if (BoosterController.instance.GetBoosterType == BoosterController.BoosterType.CUSTOM_MOVE_BLOCK)
                {
                    BoosterController.instance.HideGrid(blockObj);
                    BoosterController.instance.CompleteUsedBooster((int)BoosterController.BoosterType.CUSTOM_MOVE_BLOCK);
                    customPositionBooster = true;
                }

                Vector3 lastPosition = new Vector3(Mathf.RoundToInt(selectedPositionBlock.x), Mathf.RoundToInt(selectedPositionBlock.y), 0);
                if (BlockController.instance.tutorial)
                {
                    if (!BlockController.instance.Tutorial_step_2)
                    {
                        if (lastPosition.x != 5)
                        {
                            lastPosition = originPositionBlock;
                        }
                    }
                    else
                    {
                        if (lastPosition.x != 3)
                        {
                            lastPosition = originPositionBlock;
                        }
                    }
                }
                //  Debug.Log(string.Format("Last Position : {0}-{1}",originPositionBlock,lastPosition ));
                selectedBlock.transform.position = lastPosition;
                UpdateAllGridInGame(0.01f);

                //   blockObj.UpdateBoxObjBoard(Mathf.FloorToInt(originPositionBlock.x), Mathf.RoundToInt(lastPosition.x), Mathf.FloorToInt(originPositionBlock.y), Mathf.RoundToInt(lastPosition.y));
            }
            else
            {
                selectedBlock.transform.position = originPositionBlock;
            }


            BoosterController.instance.SetBoosterType(BoosterController.BoosterType.NONE);
            if (selectedBlock.transform.position.x == FirstBlockPoint.x && selectedBlock.transform.position.y == FirstBlockPoint.y)
            {

                SoundController.instance.BlockMiss();
                ResetSelectedBlock();
            }
            else
            {



                SoundController.instance.BlockPlace();

#if UNITY_EDITOR
                actions.Add("Move Block");
#endif
                prevComboScore++;

                SetBlockState(BlockState.FALL);


                ResetManualUpdate();
                finishStateInBoard = false;
                SetStateGame(GameState.WATING);
                ResetSelectedBlock();

            }
        }






    }


    private void VisibleWarningArea(bool visible)
    {
        warningArea.gameObject.SetActive(visible);
    }


    private void ResetSelectedBlock()
    {
        selectedBlock = null;
        FirstSelectPoint = Vector2.zero;
        FirstBlockPoint = Vector2.zero;
        blockMovePointMax = gridWidth;
        blockMovePointMin = 0;
    }



    private IEnumerator CreateBlockBeginGame()
    {
        int numberBlocksLine = (BlockController.instance.tutorial) ? 3 : 4;

        for (int i = 0; i < numberBlocksLine; i++)
        {
            yield return new WaitForSeconds(.3f);
            MoveNextLine(false);
        }
        UpdateAllGridInGame(.4f);
        yield return new WaitForSeconds(.4f);
        finishStateInBoard = true;

        if (BlockController.instance.tutorial)
        {
            TutorialManager.instance.ActiveHandMove(.1f);
        }
        else
        {
            TutorialManager.instance.VisibleButton(true);
        }
        ResetManualUpdate();
        SetBlockState(BlockState.FALL);
    }
    private IEnumerator CreateBlockTutorialStep2()
    {
        int numberBlocksLine = 2;

        for (int i = 0; i < numberBlocksLine; i++)
        {
#if UNITY_EDITOR
            actions.Add("Move Next Line Tutorial");
#endif
            yield return new WaitForSeconds(.3f);
            MoveNextLine(false);
        }
        comboBlockLine = 1;
        UpdateAllGridInGame(.4f);
        yield return new WaitForSeconds(.4f);
        TutorialManager.instance.ActiveHandMove(0.02f);
        finishStateInBoard = true;
        SetBlockState(BlockState.FALL);
    }



    private void MoveNextLine(bool updateGridIngame = true)
    {

        if (currentGameState == GameState.OTHER_SCREEN) return;
        List<BlockObj> blocksMove = BlockController.instance.CreateLine(currentScore);
        BlockContainer blockContainer = new BlockContainer();
        blockContainer.blocks = blocksMove;
        BlocksInGame.Add(blockContainer);
        int start = (BlockController.instance.tutorial) ? 0 : 1;
        for (int i = start; i < BlocksInGame.Count; i++)
        {

            for (int j = 0; j < BlocksInGame[i].blocks.Count; j++)
            {
                if (BlocksInGame[i].blocks[j] == null)
                {
                    continue;
                }
                BlockObj blockObj = BlocksInGame[i].blocks[j];
                blockObj.X_InBoard = i;

                blockObj.BlockInGrid = false;
                if (blockObj.Destroy) continue;
                Transform block = BlocksInGame[i].blocks[j].transform;
                Vector3 position = block.position;
                float y = block.position.y + 1f;

                float timeAnimation = .3f;
                if (y >= -0.5f && y < 0)
                {
                    y = 0;

                    block.transform.position = new Vector3(position.x, -1, 0);
                    timeAnimation = .3f;
                }

                position.y = y;

                int startX = (int)block.transform.position.x;
                int startY = (int)block.transform.position.y;





 
                BlocksInGame[i].blocks[j].currentY = (int)position.y;
                LeanTween.move(block.gameObject, position, timeAnimation);
               
 

            }
        }

        if (updateGridIngame)
        {
            UpdateAllGridInGame(.3f);
        }


        //  Time.timeScale = 0;



    }

    private void UpdateAllGridInGame(float waitTime = 0.6f)
    {

        Timer.Schedule(this, waitTime, () =>
        {
            CLearAllGrid();
            for (int i = 0; i < BlockController.instance.GetBlockInBoard.Count; i++)
            {
                BlockObj blockObj = BlockController.instance.GetBlockInBoard[i];
                blockObj.UpdateBoxGrid();
            }
        });
    }

    private void CLearAllGrid()
    {
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                gridInGame[i].gridList[j] = null;
            }
        }
    }

    private List<GameObject> GetListObjPositionY(int y) //각 라인에 있는 블럭을 넣어서 반환함
    {
        List<GameObject> blocks = new List<GameObject>();
        for (int i = 0; i < gridWidth; i++)
        {
            if (gridInGame[y].gridList[i] != null)
            {
                GameObject b = gridInGame[y].gridList[i].parent.parent.gameObject;
                if (!blocks.Contains(b))
                {
                    blocks.Add(b);
                }
            }
        }

        return blocks;
    }

    private void BlockFall()
    {

        blockFallInBoard = false;

        for (int i = 0; i < gridHeight; i++)
        {
            List<GameObject> blocksObj = GetListObjPositionY(i);
            for (int j = 0; j < blocksObj.Count; j++)
            {

                BlockObj blockObj = blocksObj[j].GetComponent<BlockObj>();
                bool blockCanFall = true;
                int yBlock = 0;
                int startY = blockObj.GetIntPositionY;


                for (int k = startY; k >= 1; k--) //예) 3번째 라인일 경우 k=3, 3부터 작아지면서, 아래로 떨어질 수 있는지 체크
                {
                    int startx = blockObj.GetIntPositionX;
                    for (int h = 0; h < blockObj.GetBlockType; h++) //예) 3칸짜리 블럭이라면, 바로 아래 라인 1,2,3칸을 하나씩 체크
                    {
                        int position = startx + h;


                        if (gridInGame[k - 1].gridList[position] != null) //블럭이 한칸이라도 있으면 
                        {

                            blockCanFall = false; //떨어질 수 없음
                            break;
                        }
                    }

                    if (!blockCanFall) break;

                    yBlock++; // 블럭이 없기 때문에 떨어질 수 있고, 몇 칸이나 떨어질 수 있는지 체크 (맨 위에서 맨 아래까지 떨어지는 경우도 있으니까)
                }





                if (yBlock > 0)
                {


                    blockFallInBoard = true;

                    blockObj.currentY = startY - yBlock; //예) 2줄 떨어질 수 있으면 y값을 떨어진 이후로 변경 해줌.
                    int startX = (int)blockObj.transform.position.x;
                    if (blockObj.Destroy) continue; // 블럭이... 없다면??? 이 경우는 뭐지
                    blockObj.UpdateBoxObjBoard(startX, startY, blockObj.currentY);



                    LeanTween.moveY(blockObj.gameObject, (float)(startY - yBlock), 0.3f).setEase(anicurve);
                    //Animation Fall Block
                    //blockObj.transform.DOMoveY((float)(startY - yBlock), 0.3f, false).SetEase(anicurve);
 

                }
            }
        }

    }
    /// <summary>
    /// Delete All Block In Game When Game Over
    /// </summary>
    private IEnumerator DeleteAllBlock(float waitTime = 0.1f, int limit = 0, bool useEffect = true)
    {
        TutorialManager.instance.VisibleButton(false);
        List<BlockObj> deleteObjs = new List<BlockObj>();

        for (int i = gridHeight - 1; i >= limit; i--)
        {
            List<GameObject> blocksObj = BlockController.instance.GetListObjPositionY(i);

            for (int j = 0; j < blocksObj.Count; j++)
            {
                if (!useEffect)
                {
                    Destroy(blocksObj[j]);
                }
                else
                {
                    BlockObj blockObj = blocksObj[j].GetComponent<BlockObj>();
                    blockObj.DestroyBlock();
                    LeanTween.alpha(blockObj.GetBlockImage.gameObject, 0, 0.2f).setEase(LeanTweenType.easeOutQuad).setDelay(blockObj.WaitTimeImageFade());
                    

 
                    SoundController.instance.BlockBroken();
                }
            }

            if (blocksObj.Count > 0 && useEffect)
            {
                yield return new WaitForSeconds(waitTime);
            }
        }






        yield break;
    }



    private void DeleteLine()
    {

        waitTimeNextLine = 0;
        List<GameObject> deleteObjs = new List<GameObject>();
        if (BoosterController.instance.GetBoosterType == BoosterController.BoosterType.ONE_ROW_DESTROY && BoosterController.instance.RowPosition >= 0)
        {

            //Delete Line Use Booster
            int y = BoosterController.instance.RowPosition;
            deleteObjs.AddRange(BlockController.instance.GetListObjPositionY(y));
            BoosterController.instance.EffectOneRowDestroy(new Vector3(-5, y));
            for (int i = 0; i < deleteObjs.Count; i++)
            {
                deleteObjs[i].GetComponent<BlockObj>().boosterType = BoosterController.BoosterType.ONE_ROW_DESTROY;
            }

            BoosterController.instance.CompleteUsedBooster((int)BoosterController.BoosterType.ONE_ROW_DESTROY);


        }
        else if (BoosterController.instance.GetBoosterType == BoosterController.BoosterType.ONE_BLOCK_DESTROY)
        {  //Delete Line Use Booster
            if (BoosterController.instance.destroyObj == null)
            {
                BoosterController.instance.SetBoosterType(BoosterController.BoosterType.NONE);
            }
            deleteObjs.Add(BoosterController.instance.destroyObj);
            BlockObj blockObj = BoosterController.instance.destroyObj.GetComponent<BlockObj>();
            blockObj.boosterType = BoosterController.BoosterType.ONE_BLOCK_DESTROY;
            BoosterController.instance.EffectOneBlockDestroy(BoosterController.instance.destroyObj.transform.position);
            BoosterController.instance.CompleteUsedBooster((int)BoosterController.BoosterType.ONE_BLOCK_DESTROY);
        }
        else
        {
            //Destroy Line Normal
            List<int> rows = new List<int>();
            //Get Line Can Destroy
            for (int i = 0; i < gridHeight; i++)
            {

                bool destroyLine = true;
                for (int j = 0; j < gridWidth; j++)
                {
                    if (gridInGame[i].gridList[j] == null)//한 줄에(i) 한 칸(j)이라도 비어져 있다면
                    {
                        destroyLine = false;
                        break;
                    }
                }

                if (destroyLine)
                {
                    for (int j = 0; j < gridWidth; j++)
                    {
                        BlockObj deleteBlock = gridInGame[i].gridList[j].parent.parent.gameObject.GetComponent<BlockObj>();
                        if (!deleteObjs.Contains(deleteBlock.gameObject))
                        {
                            if (deleteBlock.BlockSpecial)
                            {
                                deleteObjs.AddRange(deleteBlock.GetBlockAround());
                            }
                            else
                            {
                                deleteObjs.Add(gridInGame[i].gridList[j].parent.parent.gameObject);
                            }
                        }

                    }

                    //Add Combo Line
                    comboBlockLine++;
                    if (comboBlockLine > 1)
                    {
                        ComboTextEffect.instance.ShowCombo(i, comboBlockLine);
                    }
                }
            }



        }
        BoosterController.instance.SetBoosterType(BoosterController.BoosterType.NONE);
        //Sort Object




        int score = 0;
        destroyLineInBoard = (deleteObjs.Count == 0) ? false : true;


        //Destroy Block 
        for (int i = 0; i < deleteObjs.Count; i++)
        {

            BlockObj blockObj = deleteObjs[i].GetComponent<BlockObj>();

            waitTimeNextLine = (waitTimeNextLine < blockObj.WaitTimeNextLine()) ? blockObj.WaitTimeNextLine() : waitTimeNextLine;

            if (!blockObj.hasBlockChain)
            {
                blockObj.ClearBoxBlock();
                blockObj.DestroyBlock();
 
                 LeanTween.alpha(blockObj.GetBlockImage.gameObject, 0, 0.2f).setEase(LeanTweenType.easeOutQuad).setDelay(blockObj.WaitTimeImageFade());

            }
            else
            {
                blockObj.UnlockChain();
            }

            SoundController.instance.BlockBroken();

            //Caculator Score In Game
            if (comboBlockLine > 1)
            {
                score += blockObj.GetBlockType * comboBlockLine;
            }
            else
            {
                score += blockObj.GetBlockType;
            }

        }

        if (destroyLineInBoard)
        {
            prevComboScore = comboBlockLine;
            if (comboBlockLine > 1)
            {

                ComboTextEffect.instance.ShowTextEffect(comboBlockLine);
            }

            if (score > 0)
            {
                ComboTextEffect.instance.ShowScore(score);


                UpdateScore(score);
            }
        }


        //Tutorial
        if (BlockController.instance.tutorial && destroyLineInBoard)
        {
            if (!BlockController.instance.Tutorial_step_2)
            {
                BlockController.instance.Tutorial_step_2 = true;

                tutorialStep2 = true;

            }
            else
            {
                BlockController.instance.Tutorial_step_2 = false;
                BlockController.instance.tutorial = false;
                BlockController.instance.FinishTutorial();
            }
            TutorialManager.instance.Reset();
        }


    }


    /// <summary>
    /// Update Score In Game And Save Best Score
    /// </summary>
    /// <param name="score"></param>
    private void UpdateScore(int score)
    {
        currentScore += score;
        DataInGame.CurrentScore = currentScore;
        highScore = (currentScore > highScore) ? currentScore : highScore;

        //Save High Score
        if (PopupController.isBoard8x8)
        {
            DataInGame.BestScore8x8 = highScore;
            PlayerPrefs.SetInt("BestScore8x8", highScore);
        }
        else
        {
            DataInGame.BestScore10x10 = highScore;
            PlayerPrefs.SetInt("BestScore10x10", highScore);

        }
        bestScoreText.text = highScore.ToString();
        currentScoreText.text = currentScore.ToString();
    }
}
