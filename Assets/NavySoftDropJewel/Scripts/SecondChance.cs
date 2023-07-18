using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SecondChance : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI durationText;
 
    [SerializeField]
    private GameObject popupSecondChance;
    [SerializeField]
    private Image fillSecondChance;
 
    private float duration;

    private float timeSecondChance = 5;

    private bool isUpdate = false;

    public void Show()
    {
        isUpdate = false;
        durationText.text = timeSecondChance.ToString();
        fillSecondChance.fillAmount = 1;
        StartCoroutine(StartCountTime());
    }

    public void OnEventReset()
    {
        durationText.text = timeSecondChance.ToString();
        fillSecondChance.fillAmount = 1;
        isUpdate = false;
        duration = timeSecondChance;
    }

    private IEnumerator StartCountTime()
    {
        yield return new WaitForSeconds(1f);
        isUpdate = true;
        duration = timeSecondChance;
       
    }

    private void Update()
    {
        if (!isUpdate) return;
        duration -= Time.deltaTime;
        durationText.text = Mathf.FloorToInt(duration).ToString();
        fillSecondChance.fillAmount = (1f / timeSecondChance) * duration;
        if (duration<=0)
        {
            durationText.text = "0";
            PopupController.instance.OnEventNoTks();
            popupSecondChance.SetActive(false);
            fillSecondChance.fillAmount = 0;
           isUpdate = false;
        }
    }
}
