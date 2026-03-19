using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpAndMpPlayer : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill; // Kéo thả thanh HP_Fill vào đây

    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy = 0f;
    public Image energyBarFill; // Kéo thả thanh MP_Fill vào đây

    [Header("Full Energy Effects")] // dùng khi đầy nộ
    [SerializeField] private Color normalEnergyColor = Color.blue;
    [SerializeField] private Color fullEnergyColor = Color.yellow;
    [SerializeField] private GameObject auraEffect; // hiệu ứng aura khi đầy nộ

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentEnergy = 0f;

        if(energyBarFill != null ) energyBarFill.color = normalEnergyColor;
        if (auraEffect != null) auraEffect.SetActive(false);

        if (healthBarFill == null)
            healthBarFill = GameObject.Find("Hp_Fill").GetComponent<Image>();

        if (energyBarFill == null)
            energyBarFill = GameObject.Find("Mp_Fill").GetComponent<Image>();

        UpdateUI();
    }

    public void SetupUI(Image hpImage, Image mpImage)
    {
        healthBarFill = hpImage;
        energyBarFill = mpImage;
    }
    public void TakeDamage(float damageAmount) // hàm trừ hp
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Giữ máu không bị âm

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateUI();
    }
    public void GainEnergy(float energyAmount) //hàm cộng mp
    {
        if (isDead) return;

        currentEnergy += energyAmount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        if(currentEnergy >= maxEnergy)
        {
            if (energyBarFill != null) energyBarFill.color = fullEnergyColor;
            if(auraEffect != null) auraEffect.SetActive(true);
        }
        UpdateUI();
    }
    public void ResetEnergy() //hàm reset mp sau khi dùng nộ
    {
        currentEnergy = 0f;

        if (energyBarFill != null) energyBarFill.color = normalEnergyColor;
        if (auraEffect != null) auraEffect.SetActive(false);
        UpdateUI();
    }
    public void TakeDamageCombo(float damageAmount, bool isComboHit) // hàm trừ hp + nhận biết có dùng combo
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Giữ máu không bị âm

        if(isComboHit && currentHealth > 0)
        {
            //StartCoroutine(HurtRoutine());
            animator.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateUI();
    }
    public void HideAura()
    {
        if (auraEffect != null) auraEffect.SetActive(false);
    }
    //hàm update chỉ số mp và hp trên unity
    private void UpdateUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        if (energyBarFill != null)
        {
            energyBarFill.fillAmount = currentEnergy / maxEnergy;
        }
    }
    private void Die() // xử lý die tuyệt đối không bị bug die rồi lại đứng    dạy 
    {
        isDead = true;
        animator.SetTrigger("Die");
        animator.SetBool("IsDead", true);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        EnemyAI aiScript = GetComponent<EnemyAI>();
        if (aiScript != null)
        {
            aiScript.enabled = false;
        }
        this.enabled = false;
    }
}
