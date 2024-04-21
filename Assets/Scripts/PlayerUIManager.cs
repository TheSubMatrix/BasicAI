using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] Image m_healthBarMask;
    [SerializeField] TMP_Text m_healthBarText;
    [SerializeField] CanvasGroup m_GameOverCanvasGroup;
    public void UpdateHealthBar(uint currentHealth, uint maxHealth)
    {
        m_healthBarMask.fillAmount = (float)currentHealth / (float)maxHealth;
        m_healthBarText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
    public void OnDeath()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_GameOverCanvasGroup.alpha = 1.0f;
        m_GameOverCanvasGroup.blocksRaycasts = true;
        m_GameOverCanvasGroup.interactable = true;
    }
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
    public void OnRetryButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
