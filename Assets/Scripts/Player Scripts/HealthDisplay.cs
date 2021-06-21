using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public int healthPoints{get; set;} = 5;
    public int maxHealth{get; set;} = 10;

    [Header("Health Display Options")]
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Vector2 healthPosition;
    [SerializeField] private GameObject healthTemplate;
    [SerializeField] private Sprite[] healthDisplay;
    [SerializeField] private GameObject[] baseExtensions;
    private GameObject[] health;
    public void Initialize() 
    {
        health = new GameObject[maxHealth];

        for(int i = 0; i < maxHealth; i++) {
            if(i < healthPoints) health[i] = Initialize(health[i], healthDisplay[0], i, true);
            else health[i] = Initialize(health[i], healthDisplay[0], i, false);
        }

        if(healthPoints > 6) IncreaseBase(0);
    }
    
    //Initializes a health point to the correct position relative to the other positions
    private GameObject Initialize(GameObject health, Sprite healthImage, int num, bool display)
    {
        health = GameObject.Instantiate(healthTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        health.name = "Health " + num; //Object's name
        health.GetComponent<RectTransform>().SetParent(this.GetComponent<RectTransform>(), false);
        //anchoredPosition is the position relative to its parent
        health.GetComponent<RectTransform>().anchoredPosition = new Vector2(healthPosition.x + ((float)(num * 27)), healthPosition.y);
        health.tag = "Health";
        health.GetComponent<Image>().sprite = healthImage;
        if(!display) health.GetComponent<Image>().enabled = false;
        return health;
    }

    public void Add(int healthIncrease)
    {
        healthPoints += healthIncrease;
        for(int i = 0; i < healthPoints; i++) 
            if(!health[i].GetComponent<Image>().IsActive()) health[i].GetComponent<Image>().enabled = true;

        if(healthPoints > 6) IncreaseBase(0);
    }

    public void Remove(int healthDecrease)
    {
        healthPoints -= healthDecrease;
        for(int i = 0; i < maxHealth; i++) 
            if(i >= healthPoints && health[i].GetComponent<Image>().IsActive()) health[i].GetComponent<Image>().enabled = false;
    }

    private void IncreaseBase(int index) {baseExtensions[index].GetComponent<Image>().enabled = true;}
}
