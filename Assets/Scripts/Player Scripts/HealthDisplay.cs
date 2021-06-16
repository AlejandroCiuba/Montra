using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour, HealthInterface
{
    [SerializeField] private int healthPoints = 5;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private Vector2 healthPosition;
    [SerializeField] private GameObject healthTemplate;
    [SerializeField] private Sprite[] healthDisplay;
    private GameObject[] health;
    // Start is called before the first frame update
    void Start()
    {
        SetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) AddHealth(1);
        else if(Input.GetKeyDown(KeyCode.G)) RemoveHealth(1);
    }

    public void SetHealth() 
    {
        health = new GameObject[maxHealth];
        if(healthPoints == 1) SetHealth(health[0], healthDisplay[3], 0, true); //Special case for only one health point
        else {
            for(int i = 0; i < maxHealth; i++) {
                if(i == 0) health[i] = SetHealth(health[i], healthDisplay[0], i, true);
                else if(i > 0 && i < healthPoints - 1) health[i] = SetHealth(health[i], healthDisplay[1], i, true);
                else if(i == healthPoints - 1) health[i] = SetHealth(health[i], healthDisplay[2], i, true);
                else health[i] = SetHealth(health[i], healthDisplay[2], i, false);
            }
        }
    }
    
    //Initializes a health point to the correct position relative to the other positions
    private GameObject SetHealth(GameObject health, Sprite healthImage, int num, bool display)
    {
        health = GameObject.Instantiate(healthTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        health.name = "Health " + num; //Object's name
        health.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>(), false);
        //anchoredPosition is the position relative to its parent
        health.GetComponent<RectTransform>().anchoredPosition = new Vector2(healthPosition.x + ((float)(num * 133)), healthPosition.y);
        health.tag = "Health";
        if(!display) health.GetComponent<Image>().enabled = false;
        else health.GetComponent<Image>().sprite = healthImage;
        return health;
    }

    public void AddHealth(int healthIncrease)
    {
        if(healthPoints == maxHealth) return; //Cannot add anymore health
        if(healthPoints + healthIncrease >= maxHealth) healthPoints = maxHealth;
        else healthPoints += healthIncrease;
        for(int i = 0; i < healthPoints; i++) {
            if(!health[i].GetComponent<Image>().IsActive()) health[i].GetComponent<Image>().enabled = true;
            if(i == 0) health[i].GetComponent<Image>().sprite = healthDisplay[0];
            else if(i > 0 && i < healthPoints - 1) health[i].GetComponent<Image>().sprite = healthDisplay[1];
            else health[i].GetComponent<Image>().sprite = healthDisplay[2];
        }
    }

    public void RemoveHealth(int healthDecrease)
    {
        if(healthPoints - healthDecrease <= 0) return; //TODO: Add lose condition
        healthPoints -= healthDecrease;
        for(int i = 0; i < healthPoints + healthDecrease; i++) {
            if(i == 0 && healthPoints != 1) health[i].GetComponent<Image>().sprite = healthDisplay[0];
            else if(i > 0 && i < healthPoints - 1) health[i].GetComponent<Image>().sprite = healthDisplay[1];
            else if(i > 0 && i == healthPoints - 1) health[i].GetComponent<Image>().sprite = healthDisplay[2];
            else health[i].GetComponent<Image>().enabled = false;
        }
        //Special Case
        if(healthPoints == 1) {
            health[0].GetComponent<Image>().sprite = healthDisplay[3];
            health[0].GetComponent<Image>().enabled = true;
        }
    }
}
