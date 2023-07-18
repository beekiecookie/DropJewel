 
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockBrokenColor
{
    public List<Texture> textures = new List<Texture>();
}
public class BlockController : MonoBehaviour, ISetUpWhenPlay
{
    public static BlockController instance;

    private const float startYPlaceBlock = -2.5f;

    [SerializeField]
    private List<Sprite> block_1_type = new List<Sprite>();
    [SerializeField]
    private List<Sprite> block_2_type = new List<Sprite>();
    [SerializeField]
    private List<Sprite> block_3_type = new List<Sprite>();
    [SerializeField]
    private List<Sprite> block_4_type = new List<Sprite>();
    [SerializeField]
    private List<Sprite> block_5_type = new List<Sprite>();

    [SerializeField]
    private List<BlockObj> blocksInBoard = new List<BlockObj>();
    [SerializeField]
    private List<Texture> crackTextures = new List<Texture>();
    [SerializeField]
    private Transform blockContainer;
    [SerializeField]
    private List<BlockBrokenColor> blockBrokenColors = new List<BlockBrokenColor>();

    public bool tutorial = false;
    public bool Tutorial_step_2 = false;

    private int countLineTutorial = 0;
    public List<BlockObj> GetAllBlocksInBoard
    {
        get
        {
            return blocksInBoard;
        }
    }

    private int gridWidth = 8;

    public Texture GetCrackTexture()
    {
        return crackTextures[Random.Range(0, crackTextures.Count - 1)];
    }

    public class BlockSlotDetail
    {
        public int startIndex = 0;
        public int length = 0;
        //Tutorial
        [HideInInspector]
        public bool blockTutorial = false;
        [HideInInspector]
        public Vector3 positionTutorial = Vector3.zero;
        public float waitTimeTutorial = 1;
        private List<int>GetBlock(int width,int length)
        {
            List<int> blocks = new List<int>();
           
            int limit = (width == 8) ? 3 : 4;
            if (length <= limit)
            {
                bool getAll = Random.Range(0f, 100f) <= 70f ? true : false;
                if (getAll)
                {
                    blocks.Add(length - 1);
                    return blocks;
                }
             
            }
            int value1 = Random.Range(1, length);
            int value2 = length - value1;
            int value3 = 0;
            if (width == 8 && value2 > 4)
            {
                value3 = Random.Range(2, value2);
                value2 = value2 - value3;
            }

            int total = value1 + value2;
            if (total >= 2 && total <= 5)
            {

                bool combine = Random.Range(0f, 100f) <= 50f ? true : false;
                if (combine)
                {

                    value1 = Mathf.FloorToInt((float)total / 2);
                    value2 = total - value1;

                }
            }
          //  Debug.Log(string.Format("{0}-{1}-{2}", value1, value2, value3));
            blocks.Add(value1 - 1);
            if (value2 > 0) blocks.Add(value2 - 1);
            if (value3 > 0) blocks.Add(value3 - 1);
            return blocks;
        }

        
        public List<int> GetBlocksType(int width)
        {
            List<int> blocks = new List<int>();
            if (length == 0) return blocks;
            int length1 = length;
            int length2 = 0;
            int limit = (width == 8) ? 5 : 6;
            
            if (length > limit)
            {
                length1 = Mathf.FloorToInt((float)length / 2);
                length2 = length - length1;
            }
            if (length2 > 0)
            {
                blocks.AddRange(GetBlock(width, length1));
                blocks.AddRange(GetBlock(width, length2));
            }
            else
            {
                blocks.AddRange(GetBlock(width, length1));
            }

            return blocks;
        }
        


    }
    public BlockBrokenColor GetColor(int id)
    {
        return blockBrokenColors[id];
    }

    private void Awake()
    {
      
        instance = this;
        tutorial= PlayerPrefs.GetInt("Tutorial",0 ) == 0 ? true : false;
    }
    

    

    public void SetUp()
    {
        gridWidth = PlayingController.gridWidth;
    }


    public void FinishTutorial()
    {
        PlayerPrefs.SetInt("Tutorial",1);
     
        for (int i = 0; i < blocksInBoard.Count; i++)
        {
            if (blocksInBoard[i] == null) continue;
            blocksInBoard[i].BlockCanMove();
        }
        TutorialManager.instance.VisibleButton(!tutorial);
    }

    public void AddBlockInBoard(BlockObj block)
    {
        blocksInBoard.Add(block);
    }
    public void RemoveBlockInBoard(BlockObj block)
    {
        blocksInBoard.Remove(block);
    }

    public void Reset()
    {
      blocksInBoard.Clear();
    }
    public List<BlockObj> GetBlockInBoard
    {
        get
        {
            List<BlockObj> blocks = new List<BlockObj>();
            blocks.AddRange(blocksInBoard);
            for (int i = 0; i < blocks.Count; i++)
            {
                BlockObj blockObj = blocks[i].GetComponent<BlockObj>();
                if (blocks[i] == null )
                {
                    blocks.RemoveAt(i);
                  //  blocksInBoard.RemoveAt(i);
                    i--;
                    continue;
                }
                int y = Mathf.RoundToInt(blocks[i].transform.position.y);
                if(y < 0)
                {
                    blocks.RemoveAt(i);
                 
                    i--;
                    continue;
                }
                if (blockObj.Destroy)
                {
                    blocks.RemoveAt(i);

                    i--;
                    continue;
                }



            }

            return blocks;
        }
    }

    public  GameObject GetObjPositionY(int y)
    {
       
        for (int i = 0; i < GetBlockInBoard.Count; i++)
        {
            if (GetBlockInBoard[i] == null) continue;
            if (Mathf.FloorToInt(blocksInBoard[i].transform.position.y) == y)
                return blocksInBoard[i].gameObject;
        }
        return null;
    }

    public List<BlockObj> GetAllBlockChain()
    {
        List<BlockObj> blocks = new List<BlockObj>();
        List<BlockObj> blocksInBoard = new List<BlockObj>();
        blocksInBoard.AddRange(GetBlockInBoard);
        for (int i = 0; i < blocksInBoard.Count; i++)
        {
            if (blocksInBoard[i].hasBlockChain)
            {
                blocksInBoard[i].name = "Block Chain";
                blocks.Add(blocksInBoard[i]);
            }
        }
        return blocks;
    }
    public List<GameObject> GetListObjPositionY(int y)
    {
        List<GameObject> objs = new List<GameObject>();
      



        for (int i = 0; i < GetBlockInBoard.Count; i++)
        {
            if (GetBlockInBoard[i] == null) continue;

            if (Mathf.RoundToInt(GetBlockInBoard[i].transform.position.y) == y)
            {
                BlockObj blockObj = GetBlockInBoard[i];
                if (blockObj.BlockSpecial )
                {
                    objs.AddRange(GetBlockInBoard[i].GetBlockAround());

                }
                else
                {
                   
                    objs.Add(GetBlockInBoard[i].gameObject);
                }
              
            }
        }
        return objs;
    }

    
   

    private void Start()
	{
        PopupController.instance.AddSetUpWhenPlay(this);
        TutorialManager.instance.VisibleButton(!tutorial);
		 
	}

    public List<BlockObj> CreateBlockLine(bool createBlockChain = false)
	{

        int numBlockEmpty = Random.Range(1, (gridWidth == 8) ? 4 : 5);

        List<BlockObj> blocks = new List<BlockObj>();
    
        int numBlock = 0;
        Dictionary<int, BlockSlotDetail> blocksSlot = new Dictionary<int, BlockSlotDetail>();
        blocksSlot.Add(0, new  BlockSlotDetail());

        for (int i = 0; i < gridWidth; i++)
        {
            if (Random.Range(0, 2) == 1 && numBlockEmpty > 0)
            {
              
                numBlock++;
                blocksSlot.Add(numBlock, new BlockSlotDetail());
                blocksSlot[numBlock].startIndex = i+1;
                numBlockEmpty--;
            }
            else
            {
             
                blocksSlot[numBlock].length++;
                if (blocksSlot[numBlock].length>=gridWidth)
                {
                    blocksSlot[numBlock].length--;
                }

            }
        }
        if(blocksSlot.Count==0)
        {
            blocksSlot[0].length -=1;
        }
        bool onlyBlockSpecialOneRow = true;
        for (int i = 0; i < blocksSlot.Count; i++)
        {
            List<int> blocksType = blocksSlot[i].GetBlocksType(gridWidth);
            int startPosition = blocksSlot[i].startIndex;
            for (int j = 0; j < blocksType.Count; j++)
            { 
             
                int blockType = blocksType[j];
                startPosition  =  ((j == 0) ? startPosition  : startPosition + blocksType[j-1]+1);
                GameObject gameObject =  Instantiate(PrefabManager.instance.blocks[blockType], new Vector2((float)startPosition, -2.5f), Quaternion.identity) as GameObject;
                BlockObj blockObj = gameObject.GetComponent<BlockObj>();
                blockObj.Initialize(createBlockChain);
    
                gameObject.transform.SetParent(blockContainer, false);
                int colorID = 0;
                bool blockSpecial = false;
                if (onlyBlockSpecialOneRow)
                {
                     blockSpecial = Random.Range(0f, 100f) <= 3 ? true : false;
                }
                if (blockType == 0)
                {
                    colorID = (blockSpecial) ? block_1_type.Count - 1 : Random.Range(0, block_1_type.Count -1);

                    blockObj.SetBlockImage(block_1_type[colorID]);
                  
                }
                else if (blockType == 1)
                {
                    colorID = (blockSpecial) ? block_2_type.Count - 1 : Random.Range(0, block_2_type.Count -1);

                    blockObj.SetBlockImage(block_2_type[colorID]);
                }
                else if (blockType == 2)
                {
                    if (gridWidth != 10) blockSpecial = false;

                    colorID = (blockSpecial) ? block_3_type.Count - 1 : Random.Range(0, block_3_type.Count -1);

                    blockObj.SetBlockImage(block_3_type[colorID]);

                }
                else if (blockType == 3)
                {

                    colorID =   Random.Range(0, block_4_type.Count - 1);
                    blockObj.SetBlockImage(block_4_type[colorID]);
                }
                else if (blockType == 4 && gridWidth==10)
                {

                    colorID = Random.Range(0, block_5_type.Count - 1);
                    blockObj.SetBlockImage(block_5_type[colorID]);
                }
                
                if(blockSpecial)
                {
                    blockObj.SetBlockSpecial();
                    onlyBlockSpecialOneRow = false;
                }
                blockObj.SetColorID(colorID);
                Vector3 position = gameObject.transform.position;
                position.y = startYPlaceBlock;
                gameObject.transform.position = position;
                blocks.Add(blockObj);

            }
        }



        return blocks;
	}

    public List<BlockObj> CreateBlockLineTutorial(Dictionary<int, BlockSlotDetail> blocksSlot, List<int> blocksType)
    {
        

        List<BlockObj> lines = new List<BlockObj>();

   
        if (blocksSlot.Count == 0)
        {
            blocksSlot[0].length -= 1;
        }

        for (int i = 0; i < blocksSlot.Count; i++)
        {
            int startPosition = blocksSlot[i].startIndex;
            int blockType = blocksType[i];
            GameObject gameObject =  Instantiate(PrefabManager.instance.blocks[blockType], new Vector2((float)startPosition, -2.5f), Quaternion.identity) as GameObject;
            BlockObj blockObj = gameObject.GetComponent<BlockObj>();
            
            blockObj.Initialize(false); 
            if (blocksSlot[i].blockTutorial)
            {
                TutorialManager.instance.blockObjs.Add(blockObj);


                TutorialManager.instance.positionTo.Add(blocksSlot[i].positionTutorial);
                blockObj.BlockCanMove();

            }
            gameObject.transform.SetParent(blockContainer, false);
            int colorID = 0;

            if (blockType == 0)
            {
                colorID =  Random.Range(0, block_1_type.Count - 1);

                blockObj.SetBlockImage(block_1_type[colorID]);
            }
            else if (blockType == 1)
            {
                colorID =  Random.Range(0, block_2_type.Count - 1);

                blockObj.SetBlockImage(block_2_type[colorID]);
            }
            else if (blockType == 2)
            {
                colorID =  Random.Range(0, block_3_type.Count - 1);

                blockObj.SetBlockImage(block_3_type[colorID]);

            }
            else if (blockType == 3)
            {
                colorID = Random.Range(0, block_4_type.Count - 1);

                blockObj.SetBlockImage(block_4_type[colorID]);
            }
            else if (blockType == 4 && gridWidth == 10)
            {
                colorID =  Random.Range(0, block_5_type.Count - 1);

                blockObj.SetBlockImage(block_5_type[colorID]);
            }
            blockObj.SetColorID(colorID);
            Vector3 position = gameObject.transform.position;
            position.y = startYPlaceBlock;
            gameObject.transform.position = position;
            lines.Add(blockObj);


        }



        return lines;
    }

  
	public List<BlockObj> CreateLine(int score)
	{
		List<BlockObj> result = new List<BlockObj>();

        if (tutorial)
        {
            Dictionary<int, BlockSlotDetail> blocksSlot = new Dictionary<int, BlockSlotDetail>();
            List<int> blocksType = new List<int>();
          
            switch (countLineTutorial)
            {
                case 0:
                    //Tutorial Line 1
                    blocksSlot.Clear();
                    blocksSlot.Add(0, new BlockSlotDetail());
                    blocksSlot.Add(1, new BlockSlotDetail());
                   
                    blocksSlot[0].length = 2;
                    blocksSlot[0].startIndex = 0;

                    blocksSlot[1].length = 3;
                    blocksSlot[1].blockTutorial = true;
                    blocksSlot[1].waitTimeTutorial = 1.5f;
                    blocksSlot[1].positionTutorial = new Vector3(7f,1);
                    blocksSlot[1].startIndex = 3;

                  

                    blocksType.Clear();
                    blocksType.Add(1);
                    blocksType.Add(2);
                

                    result = CreateBlockLineTutorial(blocksSlot, blocksType);
                    break;
                case 1:
                    //Tutorial Line 2

                    Debug.Log("Line 2");
                    blocksSlot.Clear();
                    blocksSlot.Add(0, new BlockSlotDetail());
                    blocksSlot.Add(1, new BlockSlotDetail());
                    blocksSlot[0].length = 2;
                    blocksSlot[0].startIndex = 0;
                    blocksSlot[1].length = 3;
                    blocksSlot[1].startIndex = 2;
                    blocksType.Clear();

                    blocksType.Add(1);
                    blocksType.Add(2);
                    if (!PopupController.isBoard8x8)
                    {
                        blocksSlot.Add(2, new BlockSlotDetail());
                        blocksSlot[2].length = 2;
                        blocksSlot[2].startIndex = 8;
                        blocksType.Add(1);

                    }
                    result = CreateBlockLineTutorial(blocksSlot, blocksType);
                    break;

                case 2:
                    //Tutorial Line 2

                    blocksSlot.Clear();
                    blocksSlot.Add(0, new BlockSlotDetail());
                  //  blocksSlot.Add(1, new BlockSlotDetail());
                    blocksSlot[0].length = 2;
                    blocksSlot[0].startIndex = 6;
                    blocksSlot[0].blockTutorial = true;
                    blocksSlot[0].waitTimeTutorial = 8f;
                    blocksSlot[0].positionTutorial = new Vector3(3f, 2);
                    blocksType.Clear();

                    blocksType.Add(1);
                   // blocksType.Add(1);

                    result = CreateBlockLineTutorial(blocksSlot, blocksType);
                    break;
                case 3:
                    //Tutorial Line 3

                    blocksSlot.Clear();
                    blocksSlot.Add(0, new BlockSlotDetail());
                   blocksSlot.Add(1, new BlockSlotDetail());
                    blocksSlot[0].length = 3;
                    blocksSlot[0].startIndex = 0;
                    blocksSlot[1].length = 3;
                   blocksSlot[1].startIndex = 5;
                    blocksType.Clear();

                    blocksType.Add(2);
                     blocksType.Add(2);
                    if (!PopupController.isBoard8x8)
                    {
                        blocksSlot.Add(2, new BlockSlotDetail());
                        blocksSlot[2].length = 2;
                        blocksSlot[2].startIndex = 8;
                        blocksType.Add(1);

                    }
                    result = CreateBlockLineTutorial(blocksSlot, blocksType);
                    break;
                case 4:
                    //Tutorial Line 4

                    blocksSlot.Clear();
                    blocksSlot.Add(0, new BlockSlotDetail());
                    blocksSlot.Add(1, new BlockSlotDetail());
                    blocksSlot[0].length = 2;
                    blocksSlot[0].startIndex = 2;
                    blocksSlot[1].length = 4;
                    blocksSlot[1].startIndex = 4;
                    blocksType.Clear();

                    blocksType.Add(1);
                    blocksType.Add(3);
                    if (!PopupController.isBoard8x8)
                    {
                        blocksSlot.Add(2, new BlockSlotDetail());
                        blocksSlot[2].length = 2;
                        blocksSlot[2].startIndex = 8;
                        blocksType.Add(1);

                    }
                    result = CreateBlockLineTutorial(blocksSlot, blocksType);
                    break;
 
                default:
                    result = CreateBlockLine();
                    break;

            }

            countLineTutorial++;
          //  if (countLineTutorial >= 1) countLineTutorial = 1;
        }
        else
        {

            if (score <= 150)
            {
                result = CreateBlockLine();

            }
            else
            {
                result = CreateBlockLine(true);
            }
        }
        return result;
	}

	 

 

	 
}
