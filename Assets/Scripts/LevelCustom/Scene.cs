using UnityEngine;

namespace LevelCustom
{
    [CreateAssetMenu(fileName = "NewScene", menuName = "Scene")]
    public class Scene : ScriptableObject
    {
        public GameObject scenePrefab;
        public Sprite imageScene;
        public string nameScene;
    }
}
