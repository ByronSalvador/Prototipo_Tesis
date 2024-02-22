using UnityEngine;

namespace LevelCustom
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Character")]
    public class Characters : ScriptableObject
    {
    
        public GameObject characterPrefab;
        public Sprite image;
        public new string name;
    

    }
}
