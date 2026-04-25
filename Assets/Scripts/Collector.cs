using UnityEngine;
using UnityEngine.Events;


public class Collector : MonoBehaviour
{
    [Header("UI Text Settings")]
    [SerializeField] private string collectedText;

    [Header("Level Settings")]
    [Tooltip("The total number of collectables required to end the level")]
    [SerializeField] private int totalCollectablesRequired;

    [Header("Events")]
    public UnityEvent<string> OnCollected;
    // New event to trigger when the goal is reached
    public UnityEvent OnAllCollected;

    private int collected;

    void Awake()
    {
        PlayerDetectAndCollect.onCollected += Collect;

        totalCollectablesRequired = Object.FindObjectsByType<PlayerDetectAndCollect>(FindObjectsSortMode.None).Length;    

        OnCollected.Invoke($"{collectedText} 0 / {totalCollectablesRequired}");
    }

    private void OnDestroy(){
        PlayerDetectAndCollect.onCollected -= Collect;
    }

    public void Collect()
    {
        collected++;
        OnCollected.Invoke($"{collectedText} {collected} / {totalCollectablesRequired}");

        if (collected >= totalCollectablesRequired){
            EndLevel();
        }
    }

    private void EndLevel(){
        OnAllCollected.Invoke();
    }
}
