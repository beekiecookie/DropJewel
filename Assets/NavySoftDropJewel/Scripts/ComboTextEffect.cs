 
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class ComboTextEffect : MonoBehaviour
{
    public static ComboTextEffect instance;

    [SerializeField]
    private List<Sprite> textEffect = new List<Sprite>();
 
    [SerializeField]
    private Animation comboxObj;
    [SerializeField]
    private Animation scoreObj;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Image effectImage;

    private void Awake()
    {
        instance = this;
    }



    public void ShowScore(int score)
    {
        scoreText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        scoreText.text = score.ToString();
        scoreObj.Stop();
        scoreObj.Play();
    }
    public void ShowTextEffect(int combo)
    {
        int pos = combo - 2;
        pos = pos >= textEffect.Count ? textEffect.Count - 1 : pos;
        effectImage.sprite = textEffect[pos];
       
        if (combo < 10)
        {
            SoundController.instance.ComboSound(combo - 2);
        }
        else
        {
            SoundController.instance.ComboSound(9);
        }
        comboxObj.gameObject.SetActive(false);
        comboxObj.gameObject.SetActive(true);
        comboxObj.Stop();
        comboxObj.Play();
    }
    public void ShowCombo(int pos_y,int comboScore)
    {
        GameObject gameObject = (GameObject)Instantiate(PrefabManager.instance.textEffectPrefab, new Vector2(3.5f, (float)pos_y), Quaternion.identity);
        Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        text.text = string.Format("{0} COMBO",comboScore);
        gameObject.SetActive(true);

        

        rigidbody2D.isKinematic = false;
        rigidbody2D.gravityScale = 6f;
        rigidbody2D.AddForce(new Vector2(Random.Range(-2f, 2f),Random.Range(22f, 23f)), ForceMode2D.Impulse);
        rigidbody2D.AddTorque( Random.Range(-1f, 1f), ForceMode2D.Impulse);
    }
}
