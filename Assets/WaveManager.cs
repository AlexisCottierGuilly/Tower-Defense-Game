using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Events;

public enum Monster
{
    Goblin,
    Troll,
    TrollThatTrolls,
    SlidingSlime,
    MediumSlime,
    Slime,
    Hunter,
    Healer
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
    [Space]
    public bool infiniteMode = false;

    [Header("Parents")]
    public GameObject monsterParent;

    [Header("Audio")]
    public AudioSource startWaveSound;
    public AudioSource winWaveSound;
    public AudioSource winGameSound;
    public AudioSource bossSpawnSound;

    [Header("Others")]
    public TextMeshProUGUI waveText;
    public GameObject waveTextTitle;
    public GameObject bossHealthBar;
    public UnityEvent gameFinished;
    public GameObject camera;

    [Space]
    public bool isSpawningMonsters = false;
    
    [HideInInspector] int currentPathIndex = 0;
    [HideInInspector] public List<GameObject> monsters = new List<GameObject>();
    [HideInInspector] public List<List<Vector2>> usedPaths = new List<List<Vector2>>();
    
    private bool playedWinWaveSound = true;
    private bool didCallGameFinished = false;
    
    void Start()
    {
        gameFinished = new UnityEvent();
        wave = 1;
        LoadFogColor();
        waveText.text = $"Vague {wave}/{waves.Count}";
        didCallGameFinished = false;
        playedWinWaveSound = true;
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
        int initialUsedPaths = GameManager.instance.GetDifficultyModifier().initialUsedPaths;
        int addPathReccurence = GameManager.instance.GetDifficultyModifier().addPathReccurence;
        
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
    
    public IEnumerator LoadNextWave()
    {
        if (wave == waves.Count && infiniteMode)
        {
            // add a new wave to the waves
            WaveData newWave = gameGenerator.waveGenerator.GetRandomWave(wave + 1);
            waves.Add(newWave);
        }
        
        if (wave == waves.Count)
        {
            if (!didCallGameFinished)
            {
                winGameSound.Play();
                gameFinished.Invoke();
                gameGenerator.PauseGame();
                didCallGameFinished = true;
            }
            
            yield return new WaitForSeconds(0f);
        }
        else
        {
            waveFinished = false;
            isSpawningMonsters = true;
            playedWinWaveSound = false;
            startWaveSound.Play();
            
            wave++;
            WaveDidStart();

            WaveData currentWaveData = waves[wave - 1];
            foreach (WavePart part in currentWaveData.waveParts)
            {
                StartCoroutine(SpawnMonsters(part.monster, part.amount, part.interval, hideBossBar: currentWaveData.hideBossBar));
                yield return new WaitForSeconds((float)part.amount * part.interval);
            }
        }

        isSpawningMonsters = false;
    }

    IEnumerator SpawnMonsters(Monster type, int repetition, float interval, bool hideBossBar=false)
    {
        for (int i = 0; i < repetition; i++)
        {
            SpawnMonster(type, hideBossBar: hideBossBar);
            yield return new WaitForSeconds(interval);
        }
    }

    public void SpawnMonster(Monster type, Vector3 position = new Vector3(), bool hideBossBar=false)
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

        if (behaviour.data.spawnMessage != "" && !hideBossBar)
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
        behaviour.projectileParent = gameGenerator.projectileParent;

        if (behaviour.data.isBoss && !hideBossBar)
        {
            bossSpawnSound.Play();
            SetBossBar(behaviour);
        }

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

    public void WaveDidStart()
    {
        waveText.text = $"Vague {wave}";
        waveTextTitle.GetComponent<TextMeshProUGUI>().text = waveText.text;

        if (!infiniteMode)
            waveText.text += $"/{waves.Count}";

        waveTextTitle.GetComponent<Animator>().SetTrigger("ShowAnimation");

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

    public void LoadWave()
    {
        if (waveFinished)
        {
            StartCoroutine(LoadNextWave());
        }
    }

    public void SetInfiniteMode()
    {
        infiniteMode = true;
        gameGenerator.ResumeGame();
        gameGenerator.notificationManager.ShowNotification("Mode infini activé");
    }
    
    void Update()
    {
        CleanMonsterList();
        waveFinished = WaveIsFinished();

        if (waveFinished && !playedWinWaveSound)
        {
            if (wave <= waves.Count)
                winWaveSound.Play();
            playedWinWaveSound = true;
        }
        
        if (autoStart)
        {
            LoadWave();
        }
    }
}
