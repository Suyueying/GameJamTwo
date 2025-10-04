using UnityEngine;

public class SceseTrans : MonoBehaviour
{
    public SceneLoader sceneLoader;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (sceneLoader != null)
            {
                sceneLoader.LoadTargetScene(true);
            }
        }
    }
}