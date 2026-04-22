using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerDetectPit : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) return;
        SceneManager.LoadScene("MainMenu");

    }
}