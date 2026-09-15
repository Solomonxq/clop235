using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [System.Serializable]
    public struct ComboStep
    {
        [Tooltip("Множник шкоди (1.0 = 100%, 1.5 = 150%, 2.0 = 200%)")]
        public float damageMultiplier;

        [Tooltip("Множник радіусу кола атаки")]
        public float radiusMultiplier;

        [Tooltip("Затримка (cooldown) ПІСЛЯ цього удару в секундах")]
        public float cooldown;

        [Tooltip("Максимальна пауза після цього удару. Якщо чекати довше — комбо збивається!")]
        public float maxPauseToNextCombo;

        [Tooltip("Колір візуалу для цього удару")]
        public Color visualColor;
    }

    [Header("Базові налаштування")]
    public float baseAttackRadius = 1.2f;    
    public float attackOffset = 1.0f;    
    public float baseAttackDamage = 30f;  

    [Header("Налаштування комбо")]
    [Tooltip("Вікно буферу (захист від занадто раннього кліка)")]
    public float inputBufferWindow = 0.1f;

    public ComboStep[] comboSteps;

    private int currentComboIndex = 0;
    private float lastAttackTime = 0f;
    private float nextAttackTime = 0f;
    private float lastClickTime = -10f; 

    [Header("Візуал")]
    public GameObject attackVisual;   
    public float visualDuration = 0.2f; 
    private SpriteRenderer visualSpriteRenderer;

    public static event Action<Vector2, Vector2, float, float> OnPlayerAttacked;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        if (attackVisual != null)
        {
            visualSpriteRenderer = attackVisual.GetComponent<SpriteRenderer>();
        }

        // Налаштування комбо за замовчуванням (якщо масив порожній в Інспекторі)
        if (comboSteps == null || comboSteps.Length == 0)
        {
            comboSteps = new ComboStep[]
            {
                new ComboStep { damageMultiplier = 1.0f, radiusMultiplier = 1.0f, cooldown = 0.3f, maxPauseToNextCombo = 0.8f, visualColor = Color.white },
                new ComboStep { damageMultiplier = 1.3f, radiusMultiplier = 1.15f, cooldown = 0.3f, maxPauseToNextCombo = 0.8f, visualColor = Color.yellow },
                new ComboStep { damageMultiplier = 2.0f, radiusMultiplier = 1.4f, cooldown = 0.6f, maxPauseToNextCombo = 0.0f, visualColor = Color.red }
            };
        }
    }

    void Update()
    {
        // 1. Фіксуємо натискання ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            lastClickTime = Time.time;
        }

        // 2. ПЕРЕВІРКА ПАУЗИ: Якщо після 1-го або 2-го удару минуло забагато часу — комбо збивається!
        if (currentComboIndex > 0)
        {
            float allowedPause = comboSteps[currentComboIndex - 1].maxPauseToNextCombo;
            if (allowedPause > 0f && Time.time - lastAttackTime > allowedPause)
            {
                ResetCombo();
            }
        }

        // 3. Б'ємо тільки тоді, коли минув Cooldown і був клік
        if (Time.time - lastClickTime <= inputBufferWindow && Time.time >= nextAttackTime)
        {
            lastClickTime = -10f; 
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        ComboStep step = comboSteps[currentComboIndex];

        float finalDamage = baseAttackDamage * step.damageMultiplier;
        float finalRadius = baseAttackRadius * step.radiusMultiplier;

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; 

        Vector2 dir = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;

        Vector2 attackDir = Vector2.right;
        float zRotation = 0f;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0) { attackDir = Vector2.right; zRotation = 0f; }
            else           { attackDir = Vector2.left;  zRotation = 180f; }
        }
        else
        {
            if (dir.y > 0) { attackDir = Vector2.up;    zRotation = 90f; }
            else           { attackDir = Vector2.down;  zRotation = -90f; }
        }

        Vector2 attackPoint = (Vector2)transform.position + attackDir * attackOffset;

        OnPlayerAttacked?.Invoke(transform.position, attackPoint, finalRadius, finalDamage);

        lastAttackTime = Time.time;
        nextAttackTime = Time.time + step.cooldown; 

        if (attackVisual != null)
        {
            attackVisual.transform.position = attackPoint;
            attackVisual.transform.rotation = Quaternion.Euler(0, 0, zRotation);
            attackVisual.transform.localScale = Vector3.one * step.radiusMultiplier;

            if (visualSpriteRenderer != null)
            {
                visualSpriteRenderer.color = step.visualColor;
            }

            StartCoroutine(ShowAttackVisual());
        }

        currentComboIndex++;
        if (currentComboIndex >= comboSteps.Length)
        {
            ResetCombo();
        }
    }

    private void ResetCombo()
    {
        currentComboIndex = 0;
    }

    private IEnumerator ShowAttackVisual()
    {
        attackVisual.SetActive(true); 
        yield return new WaitForSeconds(visualDuration); 
        attackVisual.SetActive(false); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, baseAttackRadius);
    }
}