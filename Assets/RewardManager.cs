using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class TowerWeight
{
    public TowerType tower;
    public int weight = 1;
}


public class RewardManager : MonoBehaviour
{
    public GameObject screen;
    public GameObject chest;
    public GameObject title;
    [Space]
    public GameObject rewardPlaceholder;
    public GameObject rewardParent;
    [Space]
    public float crystalPercentage = 0.75f;
    public Sprite crystalIcon;
    [Space]
    public List<TowerWeight> towerWeights;

    private bool didLoadReward = false;
    private bool showingReward = false;
    private GameObject rewardInstance = null;

    void Start()
    {
        InititalizeWeights();
    }

    void Update()
    {
        if (GameManager.instance.player.rewardCount > 0)
            screen.SetActive(true);
        else if (!showingReward)
        {
            screen.SetActive(false);
            didLoadReward = false;
            showingReward = false;
        }
        

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            if (GameManager.instance.player.rewardCount > 0 && !didLoadReward && !showingReward)
            {
                ShowReward();
                showingReward = true;
            }

            else if (didLoadReward)
            {
                if (GameManager.instance.player.rewardCount == 0)
                    screen.SetActive(false);
                else
                {
                    Animator chestAnim = chest.GetComponent<Animator>();
                    chestAnim.Rebind();
                    chestAnim.Update(0f);

                    Animator titleAnim = title.GetComponent<Animator>();
                    titleAnim.Rebind();
                    titleAnim.Update(0f);
                }
                
                if (rewardInstance != null)
                    Destroy(rewardInstance);
                
                didLoadReward = false;
                showingReward = false;
            }
        }
    }

    void ShowReward()
    {
        GameManager.instance.player.rewardCount -= 1;
        
        chest.GetComponent<Animator>().SetTrigger("Clear");
        title.GetComponent<Animator>().SetTrigger("Clear");

        float delay = chest.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;

        StartCoroutine(LoadTimedReward(delay));
    }

    IEnumerator LoadTimedReward(float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);
        LoadReward();
    }

    void LoadReward()
    {
        if (rewardInstance != null)
            Destroy(rewardInstance);
        
        GameObject reward = Instantiate(rewardPlaceholder, rewardParent.transform);
        rewardInstance = reward;
        reward.SetActive(true);

        StartCoroutine(UpdateDidLoadReward(reward.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length));

        reward.transform.localPosition = Vector3.zero;
        reward.transform.localScale = Vector3.one;

        List<TowerPrefab> notUnlocked = GetNotUnlockedTowers();

        bool isCrystal = Random.Range(0f, 1f) < crystalPercentage;

        if (isCrystal || notUnlocked.Count == 0)
            GiveCrystals(reward.GetComponent<RewardPreviewManager>());
        else
            GiveTower(reward.GetComponent<RewardPreviewManager>(), notUnlocked);
        
        GameManager.instance.player.achievementStats.openedBoxes += 1;
    }

    IEnumerator UpdateDidLoadReward(float delay)
    {
        yield return new WaitForSeconds(delay);
        didLoadReward = true;
    }

    void GiveCrystals(RewardPreviewManager preview)
    {
        int crystals = Random.Range(1, 3);

        string suffix = crystals > 1 ? "cristaux" : "cristal";
        string tempTitle = $"{crystals} {suffix} !";

        preview.title.text = tempTitle;
        preview.icon.sprite = crystalIcon;

        GameManager.instance.player.crystals += crystals;
    }

    void GiveTower(RewardPreviewManager preview, List<TowerPrefab> notUnlocked)
    {
        TowerPrefab prefab = GetRandomTower(notUnlocked);

        preview.title.text = prefab.data.name;
        preview.icon.sprite = Sprite.Create(prefab.icon, new Rect(0, 0, prefab.icon.width, prefab.icon.height), new Vector2(0.5f, 0.5f));

        GameManager.instance.player.unlockedTowers.Add(prefab.tower);
    }

    List<TowerPrefab> GetNotUnlockedTowers()
    {
        List<TowerPrefab> notUnlocked = new List<TowerPrefab>();

        foreach (TowerPrefab prefab in GameManager.instance.towerPrefabs)
        {
            if (!GameManager.instance.player.unlockedTowers.Contains(prefab.tower))
                notUnlocked.Add(prefab);
        }

        return notUnlocked;
    }

    TowerPrefab GetRandomTower(List<TowerPrefab> notUnlocked)
    {
        List<TowerType> usedTypes = new List<TowerType>();
        foreach (TowerPrefab prefab in notUnlocked)
            usedTypes.Add(prefab.tower);
        
        List<TowerWeight> usedWeights = new List<TowerWeight>();
        
        foreach (TowerWeight weight in towerWeights)
        {
            if (usedTypes.Contains(weight.tower))
                usedWeights.Add(weight);
        }

        int totalWeight = 0;
        foreach (TowerWeight weight in usedWeights)
            totalWeight += weight.weight;
        
        int random = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (TowerWeight weight in usedWeights)
        {
            currentWeight += weight.weight;
            if (random < currentWeight)
            {
                foreach (TowerPrefab prefab in notUnlocked)
                {
                    if (prefab.tower == weight.tower)
                        return prefab;
                }
            }
        }

        return null;
    }

    void InititalizeWeights()
    {
        foreach (TowerPrefab prefab in GameManager.instance.towerPrefabs)
        {
            bool found = false;
            foreach (TowerWeight weight in towerWeights)
            {
                if (weight.tower == prefab.tower)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                TowerWeight newWeight = new TowerWeight();
                newWeight.tower = prefab.tower;
                towerWeights.Add(newWeight);
            }
        }
    }
}
