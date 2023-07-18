using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
[System.Serializable]
public class BottomObj
{
    public List<GameObject> objs = new List<GameObject>();
    public int positionY = 0;


}
public class BlockObj : MonoBehaviour
{
    [SerializeField]
    private int colorID;
    [SerializeField]
    private int blockRange = 1;
    [SerializeField]
    private int blockType = 1;
    [SerializeField]
    private SpriteRenderer block;
    [SerializeField]
    private GameObject chain;
    [SerializeField]
    private GameObject bound;
    [SerializeField]
    private GameObject outLine;
    [SerializeField]
    private ParticleSystem fx_block_broken;
    [SerializeField]
    private ParticleSystem fx_broken_chain;
    [SerializeField]
    private ParticleSystem fx_block_crack;
    [SerializeField]
    private ParticleSystem fx_broken_same_color;
    [SerializeField]
    private ParticleSystem fx_block_connect_special;
    [SerializeField]
    private ParticleSystem fx_block_special;
    [SerializeField]
    private List<GameObject> boxObjs = new List<GameObject>();
    [SerializeField]
    private List<ParticleSystem> lightingTrail = new List<ParticleSystem>();

    [SerializeField]
    private List<BottomObj> bottomObjs = new List<BottomObj>();
    public int currentY { get; set; }
    private Transform swordTarget;
    [SerializeField]
    List<Material> mats = new List<Material>();
    [SerializeField]
    List<Material> matCracks = new List<Material>();

    public int X_InBoard { get; set; }

    private List<GameObject> blocksSpecial = new List<GameObject>();

    public bool BlockInGrid;
    public bool DestroyWithBlockSpecial { get; set; }
    public bool hasBlockChain { get; set; }
    public bool Destroy { get; set; }

    public bool canMoveBlock { get; set; }


    public bool moveNextLine1 = false;
    public bool moveNextLine2 = false;
    public int GetIntPositionY { get { return (int)transform.position.y; } }
    public int GetIntPositionX { get { return (int)transform.position.x; } }
    public SpriteRenderer GetBlockImage { get { return block; } }
    public bool BlockSpecial { get; private set; }
    public BoosterController.BoosterType boosterType { get; set; }

    public int GetColorID { get { return colorID; } }
    public int GetBlockRange { get { return blockRange; } }

    public int GetBlockType { get { return blockType; } }

    public void SetColorID(int id)
    {
        colorID = id;
    }
    public void SetBlockSpecial()
    {
        BlockSpecial = true;

    }


    public void SetBlockImage(Sprite image)
    {
        block.sprite = image;
    }
    public void BlockCanMove()
    {
        canMoveBlock = true;
    }
    public void ActiveOutLineTutorial(bool visible)
    {
        outLine.SetActive(visible);
    }


    public void SetSortingOrder(int sorting)
    {
        block.sortingOrder = sorting;
    }
    public void Initialize(bool blockChain)
    {
        Renderer[] renderers = fx_block_broken.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            mats.Add(renderers[i].material);
        }

        canMoveBlock = !BlockController.instance.tutorial;
        Renderer[] crackRenderers = fx_block_crack.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < crackRenderers.Length; i++)
        {
            matCracks.Add(crackRenderers[i].material);
        }

        BlockController.instance.AddBlockInBoard(this);
        Destroy = false;
        hasBlockChain = false;
        if (blockChain)
        {
            bool rateCreateBlock = Random.Range(0f, 100f) <= 20 ? true : false;
            hasBlockChain = rateCreateBlock;
            chain.SetActive(rateCreateBlock);
        }

        for (int i = 0; i < boxObjs.Count; i++)
        {
            bottomObjs.Add(new BottomObj());
        }
    }


    public void ActiveEffectBlockSpecial()
    {
        if (fx_block_connect_special != null)
        {
            fx_block_connect_special.gameObject.SetActive(true);
            fx_block_connect_special.Play();
        }
    }

    public void DestroyBlock()
    {
        outLine.SetActive(false);
        PlayingController.instance.RemoveBlockInBoard(X_InBoard, this);
        StartCoroutine(EffectDestroyBlock());
    }

    //Get Block Around When Destroy Block 7 Color and destroy it
    public List<GameObject> GetBlockAround()
    {
        blocksSpecial.Clear();

        if (hasBlockChain)
        {
            blocksSpecial.Add(gameObject);
            return blocksSpecial;
        }

        for (int i = 0; i < boxObjs.Count; i++)
        {


            Vector2 position = boxObjs[i].transform.position;

            int x = (int)position.x;
            int y = (int)position.y;
            bool top = (y + 1) <= PlayingController.gridHeight - 1 ? true : false;
            bool bottom = (y > 0) ? true : false;
            if (top)
            {
                if (PlayingController.instance.gridInGame[y + 1].gridList[x] != null)
                {
                    if (!blocksSpecial.Contains(PlayingController.instance.gridInGame[y + 1].gridList[x].parent.parent.gameObject))
                        blocksSpecial.Add(PlayingController.instance.gridInGame[y + 1].gridList[x].parent.parent.gameObject);
                }
            }
            if (bottom)
            {
                if (PlayingController.instance.gridInGame[y - 1].gridList[x] != null)
                {
                    if (!blocksSpecial.Contains(PlayingController.instance.gridInGame[y - 1].gridList[x].parent.parent.gameObject))
                        blocksSpecial.Add(PlayingController.instance.gridInGame[y - 1].gridList[x].parent.parent.gameObject);
                }
            }


        }

        blocksSpecial.Add(gameObject);

        return blocksSpecial;
    }

    public float WaitTimeImageFade()
    {
        switch (boosterType)
        {
            case BoosterController.BoosterType.ONE_BLOCK_DESTROY:
                return 0.3f;
            case BoosterController.BoosterType.ONE_ROW_DESTROY:
                return 0.3f;
        }

        return 0.1f;
    }


    public float WaitTimeNextLine()
    {
        switch (boosterType)
        {
            case BoosterController.BoosterType.ONE_BLOCK_DESTROY:
                return 0.7f;
            case BoosterController.BoosterType.ONE_ROW_DESTROY:
                return 0.7f;
            case BoosterController.BoosterType.CUSTOM_MOVE_BLOCK:
                return 0.7f;
        }
        if (BlockSpecial)
        {
            return 0.7f;
        }
        return 0.2f;
    }

    private IEnumerator EffectDestroyBlock()
    {

        block.sortingOrder = 100;
        switch (boosterType)
        {
            case BoosterController.BoosterType.ONE_BLOCK_DESTROY:
                yield return new WaitForSeconds(.2f);
                boosterType = BoosterController.BoosterType.NONE;
                StartCoroutine(AddEffectBroken());
                break;
            case BoosterController.BoosterType.ONE_ROW_DESTROY:

                swordTarget = BoosterController.instance.GetSwordObj.transform;
                break;
            case BoosterController.BoosterType.CUSTOM_MOVE_BLOCK:
                fx_broken_same_color.gameObject.SetActive(true);
                fx_broken_same_color.Play();
                yield return new WaitForSeconds(.2f);
                boosterType = BoosterController.BoosterType.NONE;
                StartCoroutine(AddEffectBroken());
                break;
            default:




                if (BlockSpecial)
                {
                    for (int i = 0; i < blocksSpecial.Count; i++)
                    {
                        BlockObj blockOther = blocksSpecial[i].GetComponent<BlockObj>();
                        blockOther.ActiveEffectBlockSpecial();
                    }
                    SoundController.instance.ExplosionSound();
                    yield return new WaitForSeconds(.23f);
                    StartCoroutine(AddEffectBroken());
                    break;
                }
                else
                {



                    StartCoroutine(AddEffectBroken());
                }

                break;
        }

        Destroy = true;
        BlockController.instance.RemoveBlockInBoard(this);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);

    }



    //When move block with booster check new position can place block
    public bool CanPlaceBlock(Vector3 position)
    {

        int startX = Mathf.RoundToInt(position.x);
        int y = (int)position.y;

        for (int i = 0; i < GetBlockType; i++)
        {
            int x = startX + i;

            if (PlayingController.instance.gridInGame[y].gridList[x] != null)
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator AddEffectBroken()
    {
        chain.SetActive(false);
        BlockBrokenColor blockBrokenColor = BlockController.instance.GetColor(colorID);
        for (int i = 0; i < matCracks.Count; i++)
        {
            matCracks[i].SetTexture("_MainTex", BlockController.instance.GetCrackTexture());
        }
        fx_block_crack.gameObject.SetActive(true);
        fx_block_crack.Play();
        yield return new WaitForSeconds(.3f);


        for (int i = 0; i < mats.Count; i++)
        {
            mats[i].SetTexture("_MainTex", blockBrokenColor.textures[Random.Range(0, blockBrokenColor.textures.Count - 1)]);
        }


        fx_block_broken.gameObject.SetActive(true);
        fx_block_broken.Play();

    }

    private void Update()
    {

        if (swordTarget == null) return;
        float dst = Vector3.Distance(swordTarget.position, transform.position);
        if (dst <= 2)
        {
            StartCoroutine(AddEffectBroken());
            swordTarget = null;
        }

    }

    //Clear Box In Grid of Game
    public void ClearBoxBlock()
    {

        Destroy = true;
        for (int i = 0; i < boxObjs.Count; i++)
        {
            boxObjs[i].GetComponent<BoxCollider2D>().enabled = false;
            Vector2 position = boxObjs[i].transform.position;

            int x = (int)position.x;
            int y = (int)position.y;
            if ((x >= 0 || x < PlayingController.gridWidth) && (y >= 0 || y < PlayingController.gridHeight))
            {
                PlayingController.instance.gridInGame[y].gridList[x] = null;
            }
        }
    }


    /// <summary>
    /// Limit position can move left and right in line
    /// </summary>
    public int PointCanMoveInLine(bool max)
    {
        int startX = GetIntPositionX;
        int y = GetIntPositionY;
        int width = PlayingController.gridWidth;
        int limit = 0;
        if (max)
        {
            startX += blockType;
            int count = 0;
            for (int i = startX; i < width; i++)
            {
                if (PlayingController.instance.gridInGame[y].gridList[i] != null)
                {
                    return GetIntPositionX + count;
                }
                count++;
            }
            limit = GetIntPositionX + count;
        }
        else
        {
            for (int i = startX - 1; i >= 0; i--)
            {
                if (PlayingController.instance.gridInGame[y].gridList[i] != null)
                {
                    return i + 1;
                }
            }
            limit = 0;
        }

        return limit;
    }

    public void UnlockChain()
    {
        fx_broken_chain.gameObject.SetActive(true);
        fx_broken_chain.Play();
        chain.GetComponent<Animation>().Play();
        hasBlockChain = false;
    }

    #region Update box block in Gird of game
    public void UpdateBoxGrid()
    {
        int startX = GetIntPositionX;

        int y = GetIntPositionY;
        for (int i = 0; i < boxObjs.Count; i++)
        {
            int x = startX + i;

            PlayingController.instance.gridInGame[y].gridList[x] = boxObjs[i].transform;
            boxObjs[i].transform.name = string.Format("Grid_{0}_{1}", y, x);
        }
    }
    public void UpdateBoxObjBoard(int startX, int endX, int startY, int endY)
    {

        for (int i = 0; i < blockType; i++)
        {
            int start = startX + i;
            int end = endX + i;
            PlayingController.instance.gridInGame[startY].gridList[start] = null;

            PlayingController.instance.gridInGame[endY].gridList[end] = boxObjs[i].transform;

            boxObjs[i].transform.name = string.Format("Grid_{0}_{1}", endY, start);
        }


    }
    public void UpdateBoxObjBoard(int startX, int startY, int endY)
    {
        UpdateBoxObjBoard(startX, startX, startY, endY);
    }

    #endregion



}

