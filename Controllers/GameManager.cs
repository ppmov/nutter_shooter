using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnitsGetter;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> unitContainer;
    [SerializeField]
    private List<Vector2> spawnPositions;
    [SerializeField]
    private Text endGameText;

    public Unit Player { get; private set; }

    private void Awake()
    {
        // set random unit playable
        int playable = Random.Range(0, unitContainer.Count);
        int i = 0;

        List<Vector2> freeSpawnPos = new List<Vector2>();
        freeSpawnPos.AddRange(spawnPositions);

        // spawn units in random positions
        while (Container.Count != unitContainer.Count)
            for (int j = freeSpawnPos.Count - 1; j >= 0; j--)
            {
                if (i == unitContainer.Count) 
                    break;

                Vector3 pos = new Vector3(freeSpawnPos[j].x, 0f, freeSpawnPos[j].y);

                if (Random.Range(0, 2) == 0)
                {
                    GameObject unit = Instantiate(unitContainer[i], pos, Quaternion.identity);
                    Container.Add(unit.GetComponent<Vulnerable>());

                    if (i == playable)
                        Player = unit.GetComponent<Unit>();

                    freeSpawnPos.RemoveAt(j);
                    i++;
                }
            }

        Player.IsPlayer = true;
    }

    void Update()
    {
        // check if game should be finished
        if (endGameText.isActiveAndEnabled) 
            return;

        if (Player == null)
            EndGame(true);

        for (int i = Container.Count - 1; i >= 0; i--)
            if (Container[i] == null)
                Container.RemoveAt(i);

        if (Container.Count <= 1)
            EndGame();
    }

    void EndGame(bool lose = false)
    {
        endGameText.gameObject.SetActive(true);

        if (lose)
            endGameText.text = "You lose";
        else
        if (Container.Count == 1)
            endGameText.text = "The winner is " + Container[0].name.Replace("(Clone)", string.Empty) + "!";
        else
            endGameText.text = "Draw";
    }

    public void OnRestart()
    {
        Container.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
