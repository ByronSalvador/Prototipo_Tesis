using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int playerIndex = PlayerPrefs.GetInt("IndexPlayer");
        Instantiate(GameManager.Instance.charactersList[playerIndex].characterPrefab, transform.position, Quaternion.identity);
    }


}
