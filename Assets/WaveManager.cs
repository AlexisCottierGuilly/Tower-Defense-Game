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
    TrollThatTrolls
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

    [Header("Parents")]
    public GameObject monsterParent;

    [Header("Others")]
    public TextMeshProUGUI roundText;
    public GameObject roundTextTitle;
    public UnityEvent gameFinished;
    public GameObject camera;

    [Space]
    public bool isSpawningMonsters = false;
    
    [HideInInspector] int currentPathIndex = 0;
    [HideInInspector] public List<GameObject> monsters = new List<GameObject>();
    
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
    
    public IEnumerator LoadNextRound()
    {
         waveFinished = false;
         isSpawningMonsters = true;
        
        if (wave >= waves.Count)
        {
            gameFinished.Invoke();
            gameGenerator.PauseGame();
            
            yield return new WaitForSeconds(0f);
        }
        else
        {
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
        GameObject monster = Instantiate(gameGenerator.GetMonsterPrefab(type));
        MonsterBehaviour behaviour = monster.GetComponent<MonsterBehaviour>();
        monster.transform.localScale = behaviour.finalScale;

        if (position != Vector3.zero)
        {
            monster.transform.position = position;
        }
        else
        {
            currentPathIndex++;
            if (currentPathIndex >= gameGenerator.pathGenerator.paths.Count)
                currentPathIndex = 0;

            int lastItemIndex = gameGenerator.pathGenerator.paths[currentPathIndex].Count - 1;
            
            Vector2 tilePosition = gameGenerator.pathGenerator.paths[currentPathIndex][lastItemIndex];
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
        behaviour.healthScript.camera = camera;
        behaviour.waveManager = this;
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
