using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject block_type_1;
    [SerializeField]
    private int poolType1 = 1;
    [SerializeField]
    private GameObject block_type_2;
    [SerializeField]
    private int poolType2 = 1;
    [SerializeField]
    private GameObject block_type_3;
    [SerializeField]
    private int poolType3 = 1;
    [SerializeField]
    private GameObject block_type_4;
    [SerializeField]
    private int poolType4 = 1;
    [SerializeField]
    private GameObject block_type_5;
    [SerializeField]
    private int poolType5 = 1;

    [SerializeField]
    private Sprite block_outLine_normal ;

    [SerializeField]
    private Sprite block_outLine_special;

    [SerializeField]
    private List<Color> colorGrid = new List<Color>();

    private List<GameObject> blocksGridType1 = new List<GameObject>();
    private List<GameObject> blocksGridType2 = new List<GameObject>();
    private List<GameObject> blocksGridType3 = new List<GameObject>();
    private List<GameObject> blocksGridType4 = new List<GameObject>();
    private List<GameObject> blocksGridType5 = new List<GameObject>();


    private void Start()
    {
        
        for (int i = 0; i < poolType1; i++)
        {
            GameObject block = Instantiate(block_type_1) as GameObject;
            block.transform.SetParent(block_type_1.transform.parent, false);
            block.SetActive(false);
            blocksGridType1.Add(block);
        }


        for (int i = 0; i < poolType2; i++)
        {
            GameObject block = Instantiate(block_type_2) as GameObject;
            block.transform.SetParent(block_type_2.transform.parent, false);
            block.SetActive(false);
            blocksGridType2.Add(block);
        }

        for (int i = 0; i < poolType3; i++)
        {
            GameObject block = Instantiate(block_type_3) as GameObject;
            block.transform.SetParent(block_type_3.transform.parent, false);
            block.SetActive(false);
            blocksGridType3.Add(block);
        }

        for (int i = 0; i < poolType4; i++)
        {
            GameObject block = Instantiate(block_type_4) as GameObject;
            block.transform.SetParent(block_type_4.transform.parent, false);
            block.SetActive(false);
            blocksGridType4.Add(block);
        }

        for (int i = 0; i < poolType5; i++)
        {
            GameObject block = Instantiate(block_type_5) as GameObject;
            block.transform.SetParent(block_type_5.transform.parent, false);
            block.SetActive(false);
            blocksGridType5.Add(block);
        }

    }

    public void ShowGridTutorial(BlockObj blockObj, Vector3 position)
    {
        List<GameObject> blocksGrid = new List<GameObject>();
        switch (blockObj.GetBlockType)
        {
            case 1: blocksGrid = blocksGridType1; break;
            case 2: blocksGrid = blocksGridType2; break;
            case 3: blocksGrid = blocksGridType3; break;
            case 4: blocksGrid = blocksGridType4; break;
            case 5: blocksGrid = blocksGridType5; break;
        }

        for (int j = 0; j < blocksGrid.Count; j++)
        {
            if (!blocksGrid[j].activeInHierarchy)
            {
                blocksGrid[j].SetActive(true);

                blocksGrid[j].transform.position = position;
                GameObject image = blocksGrid[j].transform.GetChild(0).gameObject;
                image.GetComponent<SpriteRenderer>().color = colorGrid[blockObj.GetColorID];
                image.GetComponent<Animation>().Play();
                break;
            }
        }
    }

    private void OnGUI()
    {
#if UNITY_EDITOR
        int width = PlayingController.gridWidth;
        int height = PlayingController.gridHeight;
        if(GUILayout.Button("Show Grid"))
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if(PlayingController.instance.gridInGame[i].gridList[j]!=null)
                    {
                        Debug.Log("Show Grid nào");
                        ShowGridEditor(PlayingController.instance.gridInGame[i].gridList[j].position);
                    }
                 
                }
            }
        }

        if (GUILayout.Button("Hide Grid"))
        {
            HideGridEditor();
        }
        if (GUILayout.Button("Time Scale"))
        {
            Time.timeScale = 1;
        }

#endif
    }


    public void ShowGridEditor(Vector3 positions)
    {
        List<GameObject> blocksGrid = new List<GameObject>();
        blocksGrid = blocksGridType1;
 


        for (int j = 0; j < blocksGrid.Count; j++)
        {
            if (!blocksGrid[j].activeInHierarchy)
            {
                blocksGrid[j].SetActive(true);

                blocksGrid[j].transform.position = positions;
                GameObject image = blocksGrid[j].transform.GetChild(0).gameObject;
                SpriteRenderer sprite = image.GetComponent<SpriteRenderer>();
                sprite.color = Color.cyan;
                image.GetComponent<Animation>().Play();
                break;

            }

        }
    }
    public void HideGridEditor()
    {
        List<GameObject> blocksGrid = new List<GameObject>();
        blocksGrid = blocksGridType1;

        for (int j = 0; j < blocksGrid.Count; j++)
        {

            blocksGrid[j].SetActive(false);
        }

        
    }


    public void ShowGrid(BlockObj blockObj,List<Vector3> positions)
    {
        List<GameObject> blocksGrid = new List<GameObject>();
        switch(blockObj.GetBlockType)
        {
            case 1: blocksGrid = blocksGridType1; break;
            case 2: blocksGrid = blocksGridType2; break;
            case 3: blocksGrid = blocksGridType3; break;
            case 4: blocksGrid = blocksGridType4; break;
            case 5: blocksGrid = blocksGridType5; break;
        }

        for (int i = 0; i < positions.Count; i++)
        {
            for (int j = 0; j < blocksGrid.Count; j++)
            {
                if (!blocksGrid[j].activeInHierarchy)
                {
                    blocksGrid[j].SetActive(true);
                
                    blocksGrid[j].transform.position = positions[i];
                    GameObject image = blocksGrid[j].transform.GetChild(0).gameObject;
                    SpriteRenderer sprite = image.GetComponent<SpriteRenderer>();
                    if (blockObj.BlockSpecial)
                    {
                        sprite.sprite = block_outLine_special;
                        sprite.color = Color.white;
                    }
                    else
                    {
                        sprite.sprite = block_outLine_normal;
                        sprite.color = colorGrid[blockObj.GetColorID];
                    }
                  
                    image.GetComponent<Animation>().Play();
                    break;
                }
            }
          
        }
    }

    public void HideGrid(BlockObj blockObj)
    {
        List<GameObject> blocksGrid = new List<GameObject>();
        switch (blockObj.GetBlockType)
        {
            case 1: blocksGrid = blocksGridType1; break;
            case 2: blocksGrid = blocksGridType2; break;
            case 3: blocksGrid = blocksGridType3; break;
            case 4: blocksGrid = blocksGridType4; break;
            case 5: blocksGrid = blocksGridType5; break;
        }


        for (int j = 0; j < blocksGrid.Count; j++)
        {

            blocksGrid[j].SetActive(false);


        }


    }
}
