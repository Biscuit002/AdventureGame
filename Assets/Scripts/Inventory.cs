using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
    }
}
