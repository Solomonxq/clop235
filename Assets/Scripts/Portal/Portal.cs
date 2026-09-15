using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    [Tooltip("Назва сцени, на яку треба перейти")]
    [SerializeField] private string sceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        
        SceneManager.LoadScene("Arena");
        

    }
}
