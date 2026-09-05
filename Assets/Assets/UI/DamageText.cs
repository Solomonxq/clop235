using UnityEngine;
using TMPro; // Потрібно для роботи з TextMeshPro

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 1.5f;   // Швидкість підйому вгору
    public float disappearTimer = 0.5f; // Час до початку зникнення
    private TextMeshPro textMesh;
    private Color textColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textColor = textMesh.color;
        }
    }

    public void Setup(float damageAmount)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        
        textMesh.text = Mathf.RoundToInt(damageAmount).ToString();
    }

    private void Update()
    {
        // Рух цифри вгору
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // Таймер для плавного зникнення (Fade Out)
        disappearTimer -= Time.deltaTime;
        if (disappearTimer <= 0)
        {
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}