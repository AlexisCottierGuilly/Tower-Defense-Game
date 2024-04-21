using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using TMPro;
using UnityEngine.Events;

public enum Monster
{
    Goblin,
    Troll,
    TrollThatTrolls,
    SlidingSlime,
    MediumSlime,
    Slime
}

public class WaveManager : MonoBehaviour
{
    public GameGenerator gameGenerator;

    [Header("Prefabs")]
    public List<NavMeshSurface> surfaces = new List<NavMeshSurface>();

    [Header("Settings")]
    public int wave = 0;
    public List<WaveData> waves = new List<WaveData>();
    public bool waveFinished = true;
    public bool autoStart = false;

    [Header("Paths")]
    public int initialUsedPaths = 1;

    // AddPathReccurence controls the number of rounds before adding a new path
    public int addPathReccurence = 2;

    [Header("Parents")]
    public GameObject monsterParent;

    [Header("Others")]
    public TextMeshProUGUI roundText;
    public GameObject roundTextTitle;
    public GameObject bossHealthBar;
    public UnityEvent gameFinished;
    public GameObject camera;

    [Space]
    public bool isSpawningMonsters = false;
    
    [HideInInspector] int currentPathIndex = 0;
    [HideInInspector] public List<GameObject> monsters = new List<GameObject>();
    [HideInInspector] public List<List<Vector2>> usedPaths = new List<List<Vector2>>();
    
    void Start()
    {
        gameFinished = new UnityEvent();
        wave = 1;
        LoadFogColor();
        roundText.text = $"Round {wave}/{waves.Count}";
        wave = 0;
    }
    
    public void InitializeSurfaces()
    {
        foreach (NavMeshSurface surface in surfaces)
        {
            surface.BuildNavMesh();
        }
    }

    void UpdateUsedPaths()
    {
        int numberOfPaths = 0;
        
        if (wave == 0)
        {
            numberOfPaths = initialUsedPaths;
        }
        else
        {
            numberOfPaths = (wave - 1) / addPathReccurence + initialUsedPaths;
            numberOfPaths = Mathf.Min(numberOfPaths, gameGenerator.pathGenerator.paths.Count);
        }

        usedPaths.Clear();
        for (int i = 0; i < numberOfPaths; i++)
        {
            usedPaths.Add(gameGenerator.pathGenerator.paths[i]);
        }
    }
    
    public IEnumerator LoadNextRound()
    {
        if (wave >= waves.Count)
        {
            gameFinished.Invoke();
            gameGenerator.PauseGame();
            
            yield return new WaitForSeconds(0f);
        }
        else
        {
            waveFinished = false;
            isSpawningMonsters = true;
            
            wave++;
            RoundDidStart();

            WaveData currentWaveData = waves[wave - 1];
            foreach (WavePart part in currentWaveData.waveParts)
            {
                StartCoroutine(SpawnMonsters(part.monster, part.amount, part.interval));
                yield return new WaitForSeconds((float)part.amount * part.interval);
            }
        }

        isSpawningMonsters = false;
    }

    IEnumerator SpawnMonsters(Monster type, int repetition, float interval)
    {
        for (int i = 0; i < repetition; i++)
        {
            SpawnMonster(type);
            yield return new WaitForSeconds(interval);
        }
    }

    public void SpawnMonster(Monster type, Vector3 position = new Vector3())
    {
        GameObject monsterPrefab = gameGenerator.GetMonsterPrefab(type);
        if (monsterPrefab == null)
        {
            Debug.LogError($"You Need to Add the {type} prefab to the GameGenerator's monsterPrefabs list.");
            return;
        }
        
        GameObject monster = Instantiate(monsterPrefab);
        MonsterBehaviour behaviour = monster.GetComponent<MonsterBehaviour>();
        monster.transform.localScale = behaviour.finalScale;

        if (behaviour.data.spawnMessage != "")
        {
            gameGenerator.notificationManager.ShowNotification(behaviour.data.spawnMessage);
        }

        if (position != Vector3.zero)
        {
            monster.transform.position = position;
        }
        else
        {
            currentPathIndex++;
            if (currentPathIndex >= usedPaths.Count)
                currentPathIndex = 0;

            int lastItemIndex = usedPaths[currentPathIndex].Count - 1;
            
            Vector2 tilePosition = usedPaths[currentPathIndex][lastItemIndex];
            GameObject placement = gameGenerator.tiles[(int)tilePosition.x][(int)tilePosition.y].GetComponent<TileBehaviour>().placement;
            
            monster.transform.position = new Vector3(
                placement.transform.position.x,
                placement.transform.position.y + monster.transform.localScale.y / 2f,
                placement.transform.position.z
            );
        };

        monster.transform.parent = monsterParent.transform;
        behaviour.gameGenerator = gameGenerator;
        behaviour.agent.destination = gameGenerator.villageGenerator.mainVillage.transform.position;
        behaviour.agent.Warp(monster.transform.position);
        behaviour.healthScript.camera = camera;
        behaviour.waveManager = this;

        if (behaviour.data.isBoss)
            SetBossBar(behaviour);

        // Destroy(monster.GetComponent<NavMeshAgent>());
        
        monsters.Add(monster);
    }

    void CleanMonsterList()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject monster in monsters)
        {
            if (monster == null)
                toRemove.Add(monster);
        }

        foreach (GameObject monster in toRemove)
        {
            monsters.Remove(monster);
        }
    }

    public void SetBossBar(MonsterBehaviour boss)
    {
        bossHealthBar.GetComponent<RectTransform>().localScale = new Vector3(0f, 0f, 1f);
        
        bossHealthBar.SetActive(true);
        bossHealthBar.GetComponent<BossHealthBar>().boss = boss;
        bossHealthBar.GetComponent<Animator>().SetTrigger("ShowAnimation");

        /*
        Bug : the bossHealthBar shows up at full scale, and then it begins it's animation (0 scale to full scale)
        the problem is that the bossHealthBas's initial scale is 1,1,1, and it should be 0, 1, 1

        How to fix this ?

        1. Set the initial scale of the bossHealthBar to 0, 1, 1
        2. Remove the line  "bossHealthBar.GetComponent<RectTransform>().localScale = new Vector3(0f, 1f, 1f);"
        3. Done
        */
    }

    public void UnsetBossBar()
    {
        bossHealthBar.SetActive(false);
        bossHealthBar.GetComponent<BossHealthBar>().boss = null;
    }

    public bool WaveIsFinished()
    {
        if (monsters.Count == 0 && !isSpawningMonsters)
            return true;
        return false;
    }

    public void RoundDidStart()
    {
        roundText.text = $"Round {wave}";
        roundTextTitle.GetComponent<TextMeshProUGUI>().text = roundText.text;
        roundText.text += $"/{waves.Count}";

        roundTextTitle.GetComponent<Animator>().SetTrigger("ShowAnimation");

        LoadFogColor();
        UpdateUsedPaths();
    }

    public void LoadFogColor()
    {
        Color startingColor = Color.green;
        Color endingColor = Color.red;
        
        float percentage = ((float)wave - 1f) / Mathf.Max((float)waves.Count - 1f, 1f);
        
        Color waveColor = Color.Lerp(startingColor, endingColor, percentage);
        float luminosityDivisor = 3f;

        waveColor = new Color(
            waveColor.r / luminosityDivisor,
            waveColor.g / luminosityDivisor,
            waveColor.b / luminosityDivisor,
            
            waveColor.a
        );

        RenderSettings.fogColor = waveColor;
    }

    public void LoadRound()
    {
        if (waveFinished)
        {
            StartCoroutine(LoadNextRound());
        }
    }
    
    void Update()
    {
        CleanMonsterList();
        waveFinished = WaveIsFinished();
        
        if (autoStart)
            LoadRound();
    }
}
