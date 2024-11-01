using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WerewolfBearer;

public class PowerUpDatabaseHolder : MonoBehaviour
{
    public static PowerUpDatabaseHolder Instance { get; private set; }

    public PowerUpDatabase powerUpDatabase;

    public int amountLevel = 0;
    public int areaLevel = 0;
    public int armorLevel = 0;
    public int cooldownLevel = 0;
    public int durationLevel = 0;
    public int greedLevel = 0;
    public int growthLevel = 0;
    public int magnetLevel = 0;
    public int maxHealthLevel = 0;
    public int mightLevel = 0;
    public int moveSpeedLevel = 0;
    public int recoveryLevel = 0;
    public int revivalLevel = 0;
    public int speedLevel = 0;

    public TMP_Text[] displayLevel;


    public int currentLevel = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnClick_PowerUp(int index)
    {
        //currentLevel++;

        switch (index)
        {
            case 0:
                amountLevel++;
                displayLevel[index].text = amountLevel.ToString();
                break;
            case 1:
                areaLevel++;
                displayLevel[index].text = areaLevel.ToString();
                break;
            case 2:
                armorLevel++;
                displayLevel[index].text = armorLevel.ToString();
                break;
            case 3:
                cooldownLevel++;
                displayLevel[index].text = cooldownLevel.ToString();
                break;
            case 4:
                durationLevel++;
                displayLevel[index].text = durationLevel.ToString();
                break;
            case 5:
                greedLevel++;
                displayLevel[index].text = greedLevel.ToString();
                break;
            case 6:
                growthLevel++;  
                displayLevel[index].text = growthLevel.ToString();
                break;
            case 7:
                magnetLevel++;
                displayLevel[index].text = magnetLevel.ToString();
                break;
            case 8:
                maxHealthLevel++;   
                displayLevel[index].text = maxHealthLevel.ToString();
                break;
            case 9:
                mightLevel++;
                displayLevel[index].text = mightLevel.ToString();
                break;
            case 10:
                moveSpeedLevel++;
                displayLevel[index].text = moveSpeedLevel.ToString();
                break;
            case 11:
                recoveryLevel++;
                displayLevel[index].text = recoveryLevel.ToString();
                break;
            case 12:
                revivalLevel++;
                displayLevel[index].text = revivalLevel.ToString();
                break;
            case 13:
                speedLevel++;
                displayLevel[index].text = speedLevel.ToString();
                break;
        }
    }
}
