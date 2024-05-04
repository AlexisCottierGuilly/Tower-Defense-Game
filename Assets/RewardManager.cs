using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public GameObject screen;
    public GameObject chest;
    public GameObject title;
    [Space]
    public GameObject rewardPlaceholder;
    public GameObject rewardParent;

    private bool didLoadReward = false;
    private GameObject rewardInstance = null;

    void Start()
    {
        if (GameManager.instance.player.hasReward)
        {
            screen.SetActive(true);
        }
        else
            screen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            if (GameManager.instance.player.hasReward && !didLoadReward)
            {
                ShowReward();
                didLoadReward = true;
            }
            else if (!GameManager.instance.player.hasReward)
            {
                screen.SetActive(false);
                if (rewardInstance != null)
                    Destroy(rewardInstance);
            }
        }
    }

    void ShowReward()
    {
        GameManager.instance.player.hasReward = false;
        
        chest.GetComponent<Animator>().SetTrigger("Clear");
        title.GetComponent<Animator>().SetTrigger("Clear");

        // wait the time the chest and title disappear to LoadReward
        // use IEnumerator to wait

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
        GameObject reward = Instantiate(rewardPlaceholder, rewardParent.transform);
        rewardInstance = reward;
        reward.SetActive(true);
        reward.transform.localPosition = Vector3.zero;
        reward.transform.localScale = Vector3.one;

        int crystals = Random.Range(1, 6);

        string suffix = crystals > 1 ? "cristaux" : "cristal";
        string tempTitle = $"{crystals} {suffix} !";
        RewardPreviewManager preview = reward.GetComponent<RewardPreviewManager>();
        preview.title.text = tempTitle;

        GameManager.instance.player.crystals += crystals;
    }
}
