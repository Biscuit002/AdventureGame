using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    private Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);
    private float speed = 5f;

    public int grassAmount = 0;
    public int dirtAmount = 0;
    public int stoneAmount = 0;
    public int woodAmount = 0;
    public int leafAmount = 0;
    public int copperAmount = 0;
    public int diamondAmount = 0;
    public int goldAmount = 0;
    public int ironAmount = 0;
    public int platinumAmount = 0;
    public int silverAmount = 0;
    public int titaniumAmount = 0;

    public int currentBlockAmount;

    public TextMeshProUGUI grassText;
    public TextMeshProUGUI dirtText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI leafText;
    public TextMeshProUGUI copperText;
    public TextMeshProUGUI diamondText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI ironText;
    public TextMeshProUGUI platinumText;
    public TextMeshProUGUI silverText;
    public TextMeshProUGUI titaniumText;

    public GameObject grassUI;
    public GameObject dirtUI;
    public GameObject stoneUI;
    public GameObject woodUI;
    public GameObject leafUI;
    public GameObject copperUI;
    public GameObject diamondUI;
    public GameObject goldUI;
    public GameObject ironUI;
    public GameObject platinumUI;
    public GameObject silverUI;
    public GameObject titaniumUI;

    public BlockDestruction blockDestruction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blockDestruction = FindObjectOfType<BlockDestruction>();
    }

    // Update is called once per frame
    void Update()
    {
        grassText.text = "" + grassAmount;
        dirtText.text = "" + dirtAmount;
        stoneText.text = "" + stoneAmount;
        woodText.text = "" + woodAmount;
        leafText.text = "" + leafAmount;
        copperText.text = "" + copperAmount;
        diamondText.text = "" + diamondAmount;
        goldText.text = "" + goldAmount;
        ironText.text = "" + ironAmount;
        platinumText.text = "" + platinumAmount;
        silverText.text = "" + silverAmount;
        titaniumText.text = "" + titaniumAmount;

        if (blockDestruction.blockIndex == 0)
        {
            currentBlockAmount = grassAmount;
            grassUI.transform.localScale = Vector3.Lerp(grassUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            grassUI.transform.localScale = Vector3.Lerp(grassUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 1)
        {
            currentBlockAmount = dirtAmount;
            dirtUI.transform.localScale = Vector3.Lerp(dirtUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            dirtUI.transform.localScale = Vector3.Lerp(dirtUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 2)
        {
            currentBlockAmount = stoneAmount;
            stoneUI.transform.localScale = Vector3.Lerp(stoneUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            stoneUI.transform.localScale = Vector3.Lerp(stoneUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 3)
        {
            currentBlockAmount = woodAmount;
            woodUI.transform.localScale = Vector3.Lerp(woodUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            woodUI.transform.localScale = Vector3.Lerp(woodUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 4)
        {
            currentBlockAmount = leafAmount;
            leafUI.transform.localScale = Vector3.Lerp(leafUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            leafUI.transform.localScale = Vector3.Lerp(leafUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == speed)
        {
            currentBlockAmount = copperAmount;
            copperUI.transform.localScale = Vector3.Lerp(copperUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            copperUI.transform.localScale = Vector3.Lerp(copperUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 6)
        {
            currentBlockAmount = diamondAmount;
            diamondUI.transform.localScale = Vector3.Lerp(diamondUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            diamondUI.transform.localScale = Vector3.Lerp(diamondUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 7)
        {
            currentBlockAmount = goldAmount;
            goldUI.transform.localScale = Vector3.Lerp(goldUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            goldUI.transform.localScale = Vector3.Lerp(goldUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 8)
        {
            currentBlockAmount = ironAmount;
            ironUI.transform.localScale = Vector3.Lerp(ironUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            ironUI.transform.localScale = Vector3.Lerp(ironUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 9)
        {
            currentBlockAmount = platinumAmount;
            platinumUI.transform.localScale = Vector3.Lerp(platinumUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            platinumUI.transform.localScale = Vector3.Lerp(platinumUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 10)
        {
            currentBlockAmount = silverAmount; 
            silverUI.transform.localScale = Vector3.Lerp(silverUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            silverUI.transform.localScale = Vector3.Lerp(silverUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (blockDestruction.blockIndex == 11)
        {
            currentBlockAmount = titaniumAmount;
            titaniumUI.transform.localScale = Vector3.Lerp(titaniumUI.transform.localScale, targetScale, Time.deltaTime * speed);
        }
        else
        {
            titaniumUI.transform.localScale = Vector3.Lerp(titaniumUI.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * speed);
        }

        if (grassAmount > 0)
        {
            grassUI.SetActive(true);
        }
        else
        {
            grassUI.SetActive(false);
        }

        if (dirtAmount > 0)
        {
            dirtUI.SetActive(true);
        }
        else
        {
            dirtUI.SetActive(false);
        }

        if (stoneAmount > 0)
        {
            stoneUI.SetActive(true);
        }
        else
        {
            stoneUI.SetActive(false);
        }

        if (woodAmount > 0)
        {
            woodUI.SetActive(true);
        }
        else
        {
            woodUI.SetActive(false);
        }

        if (leafAmount > 0)
        {
            leafUI.SetActive(true);
        }
        else
        {
            leafUI.SetActive(false);
        }

        if (copperAmount > 0)
        {
            copperUI.SetActive(true);
        }
        else
        {
            copperUI.SetActive(false);
        }

        if (diamondAmount > 0)
        {
            diamondUI.SetActive(true);
        }
        else
        {
            diamondUI.SetActive(false);
        }

        if (goldAmount > 0)
        {
            goldUI.SetActive(true);
        }
        else
        {
            goldUI.SetActive(false);
        }

        if (ironAmount > 0)
        {
            ironUI.SetActive(true);
        }
        else
        {
            ironUI.SetActive(false);
        }

        if (platinumAmount > 0)
        {
            platinumUI.SetActive(true);
        }
        else
        {
            platinumUI.SetActive(false);
        }

        if (silverAmount > 0)
        {
            silverUI.SetActive(true);
        }
        else
        {
            silverUI.SetActive(false);
        }

        if (titaniumAmount > 0)
        {
            titaniumUI.SetActive(true);
        }
        else
        {
            titaniumUI.SetActive(false);
        }
    }
}
