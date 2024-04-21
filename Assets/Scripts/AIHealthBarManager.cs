using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AIHealthBarManager : MonoBehaviour
{
    [SerializeField]Image m_healthBarMask;
    [SerializeField] TMP_Text m_healthBarText;
    public void UpdateHealthBar(uint currentHealth, uint maxHealth)
    {
        m_healthBarMask.fillAmount = (float)currentHealth / (float)maxHealth;
        m_healthBarText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
    public void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
