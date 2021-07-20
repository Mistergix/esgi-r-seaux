using System.Collections;
using System.Collections.Generic;
using PGSauce.Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUI : MonoSingleton<MultiplayerUI>
{
    public Image battleResultPanel;
    public TMP_Text battleResultTitle;
    public TMP_Text battleResultDuration;
    public TMP_Text battleResultKilledEnemies;
    public TMP_Text battleResultKilledAllies;
    public GameObject gamePanel;
    
    public Image leftCastleHealthBar;
    public Image rightCastleHealthBar;
    public Text unitCost;
    public Text goldText;
    public GameObject boxSelectTarget;
    
    public Text leftCastleHealthText;
    public Text rightCastleHealthText;
    

    public void SetVictory(Color color, string text, int killedEnemies, int deadAllies, string time)
    {
        battleResultPanel.color = color;
        battleResultTitle.text = text;
        battleResultKilledEnemies.text = $"Killed Enemies = {killedEnemies.ToString()}";
        battleResultKilledAllies.text = $"Dead allies = {deadAllies.ToString()}";
        battleResultDuration.text = time;
        
        gamePanel.SetActive(false);
        battleResultPanel.gameObject.SetActive(true);
    }

    public void SetHealthBarColors(bool master, Color masterClientColor, Color clientColor)
    {
        leftCastleHealthBar.color = master ? masterClientColor : clientColor;
        rightCastleHealthBar.color = master ? clientColor : masterClientColor;
    }

    public void UpdateGoldText(int goldManagerGold)
    {
        goldText.text = "Gold = "  + goldManagerGold.ToString();
    }

    public void UpdateUI(bool master, float masterCastleHealth, float clientCastleHealth, float masterCastleHealthStart, float clientCastleHealthStart)
    {
        leftCastleHealthText.text = (master ? (int)masterCastleHealth : (int)clientCastleHealth).ToString();
        rightCastleHealthText.text = (master ? (int)clientCastleHealth : (int)masterCastleHealth).ToString();
		
        float clientFillAmount = clientCastleHealth/clientCastleHealthStart;
        float masterFillAmount = masterCastleHealth/masterCastleHealthStart;
		
        rightCastleHealthBar.fillAmount = master ? clientFillAmount : masterFillAmount;
        leftCastleHealthBar.fillAmount = master ? masterFillAmount : clientFillAmount;
    }
}
