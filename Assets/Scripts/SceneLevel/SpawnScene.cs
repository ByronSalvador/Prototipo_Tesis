using UnityEngine;

namespace SceneLevel
{
    public class SpawnScene : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            int sceneIndex = PlayerPrefs.GetInt("IndexScene");
            Instantiate(GameManager.Instance.scenesList[sceneIndex].scenePrefab, transform.position, Quaternion.identity);

        }


    }
}
