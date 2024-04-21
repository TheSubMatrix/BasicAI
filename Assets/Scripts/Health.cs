using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health :MonoBehaviour, IDamagable
{
    uint _currentHealth;
    public uint CurrentHealth 
    {
        get 
        {
            return _currentHealth; 
        } 
        private set 
        {
            CheckDeathState(_currentHealth, value);
            _currentHealth = value;
        } 
    }
    public uint MaxHealth = 100;
    public bool useInvulnerabilityAfterDamage = true;
    public float inulnerabilityAfterDamageTimer = 3;
    bool _isInvulnerable = false;
    IEnumerator currentInvincibilityTimer = null;
    public bool IsInvulnerable 
    {
        get
        {
            return (_isInvulnerable);
        }
        set
        {
            if(currentInvincibilityTimer != null)
            {
                StopCoroutine(currentInvincibilityTimer);
            }
            if(value)
            {
                OnBecameInvulnerable.Invoke();
            }
            else
            {
                OnBecameVulnerable.Invoke();
            }
            _isInvulnerable = value;
        } 
    }
    public bool IsAlive { get; private set; } = true;

    public UnityEvent OnDeath = new UnityEvent();
    public UnityEvent OnRevive = new UnityEvent();
    public UnityEvent OnBecameInvulnerable = new UnityEvent();
    public UnityEvent OnBecameVulnerable = new UnityEvent();
    public UnityEvent<uint, uint> OnHeal = new UnityEvent<uint, uint>();
    public UnityEvent<uint, uint> OnDamage = new UnityEvent<uint, uint>();
    public UnityEvent<uint, uint> OnHealthComponentInitialized = new UnityEvent<uint, uint>();

    void Start()
    {
        CurrentHealth = MaxHealth;
        IsAlive = CurrentHealth > 0;
        OnHealthComponentInitialized.Invoke(CurrentHealth, MaxHealth);
    }
    public void Damage(uint damage)
    {
        if(!IsInvulnerable)
        {
            uint newHealth = (uint)Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
            CurrentHealth = newHealth;
            OnDamage.Invoke(CurrentHealth, MaxHealth);
            if (gameObject.activeSelf)
            {
                StartCoroutine(InvulnerabilityTimerAsync());
            }
        }
    }
    public void Heal(uint amountToHeal)
    {
        uint newHealth = (uint)Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);;
        CurrentHealth = newHealth;
        OnHeal.Invoke(CurrentHealth, MaxHealth);
    }
    void CheckDeathState(uint previousHealth, uint currentHealth)
    {
        if(currentHealth <= 0 && previousHealth > 0)
        {
            IsAlive = false;
            OnDeath.Invoke();
        }
        if(previousHealth <= 0 && currentHealth > 0) 
        {
            IsAlive = true;
            OnRevive.Invoke();
        }
    }
    public void Reset()
    {
        CurrentHealth = MaxHealth;
        IsAlive = CurrentHealth > 0;
    }
    IEnumerator InvulnerabilityTimerAsync()
    {
        if (useInvulnerabilityAfterDamage)
        {
            _isInvulnerable = true;
            OnBecameInvulnerable.Invoke();
            yield return new WaitForSeconds(inulnerabilityAfterDamageTimer);
            _isInvulnerable = false;
            OnBecameVulnerable.Invoke();
        }
        else
        {
            yield return null;
        }
    }
}