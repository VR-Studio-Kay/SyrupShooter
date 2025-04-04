using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100;
    public float maxHealth = 100;

    // Variable used for visual health smoothing
    private float healthperc;


    [Header("Offset for health bar. Increase this to increase distance between object and health bar")]
    public float offset;

    [Header("The object that will represent the health bar. The default one included in this package is located in Nick's Assets/Nick's Stat bar/Healthbar")]
    [SerializeField]
    private GameObject Healthbar;

    [Header("For extra effects")]
    [SerializeField]
    private bool Animation;
    [SerializeField]
    private float animSizeUpDuration = 0.1f;
    [SerializeField]
    private float animSizeDownDuration = 0.3f;
    [SerializeField]
    private float animAmmount = 1.0f;

    // Vector3 to store what scale of the healthbar should be after adding in the animAmmount;
    Vector3 animationAmmountChanged;
    // Vector3 to store original scale of the healthBar
    Vector3 healthBarOriginScale;
    // AudioSource to store healthBar's audioSource component
    AudioSource healthBarAC;

    [Header("Sound to play when Healthbar takes damage, leave empty for no sound")]
    [SerializeField]
    private AudioClip hitSound;

    [Header("How much time it will take for the health bar to change")]
    public float Healthbarsmoothing = 1;

    [Header("How long the damage effect will last")]
    public float DamageDuration = 1;
    // Actual variable that will store our instantiated Healthbar
    private GameObject healthBar;

    // Convienient Variables for accessing UI elements under Healthbar
    private GameObject HealthBarUI;
    private Image HealthBarImage;
    private Image damageEffectMask;
    private Image damageEffectImage;

    // Positioning the visible health bar
    void moveHealthBar(){
        healthBar.transform.position = new Vector3(this.gameObject.transform.position.x,(this.gameObject.transform.position.y + ((this.transform.localScale.y/2) + 0.5f)) + offset, this.gameObject.transform.position.z);
    }



    // Coroutine that will run a little animation effect when TakeDamage() is called
    bool animRunning = false;
    private IEnumerator animCoroutine;
    IEnumerator AnimationCoroutine(){
        animRunning = true;
        // Using lerp correctly

        float elapsedTime = 0;
        
        Vector3 preChangeScale = healthBar.transform.localScale;

        while(elapsedTime < animSizeUpDuration){
            elapsedTime += Time.deltaTime;
            healthBar.transform.localScale = Vector3.Lerp(preChangeScale, new Vector3(Mathf.Clamp(preChangeScale.x + animAmmount, preChangeScale.x, animationAmmountChanged.x), Mathf.Clamp(preChangeScale.y + animAmmount, preChangeScale.y, animationAmmountChanged.y), Mathf.Clamp(preChangeScale.z + animAmmount, preChangeScale.z, animationAmmountChanged.z)), elapsedTime/animSizeUpDuration);
            yield return null;
        }
        
        Vector3 afterChangeScale = healthBar.transform.localScale;
        elapsedTime = 0;

        while(elapsedTime < animSizeDownDuration){
            elapsedTime += Time.deltaTime;
            healthBar.transform.localScale = Vector3.Lerp(afterChangeScale, healthBarOriginScale, elapsedTime/animSizeDownDuration);
            yield return null;
        }

        healthBar.transform.localScale = healthBarOriginScale;
        animRunning = false;
    }
 



 
    // bool that will prevent coroutine from starting more than once
    bool isRunning = false;
    // Coroutine that will lerp the visible health bar where it should be
    private IEnumerator healthbarcoroutine;
    IEnumerator MoveHealthBarCoroutine(){
        isRunning = true;
        // Make sure we are using the Mathf.Lerp method correctly
        float elapsedTime = 0;
        float preChangePercent = HealthBarImage.fillAmount;
        while(elapsedTime < Healthbarsmoothing){
        
            elapsedTime += Time.deltaTime;
            HealthBarImage.fillAmount = Mathf.Lerp(preChangePercent, healthperc, elapsedTime/Healthbarsmoothing);
            yield return null;
        }

        HealthBarImage.fillAmount = healthperc;
        isRunning = false;
    }


    // Bool that will make sure that while coroutine is running alpha of damage effect is not being effected by Update
    bool fadeEffectInProgress = false;
    // Float that will store the ammount of damage last given to this object
    float lastDamage = 0;
    // Coroutine that will do a damage effect where a semi-transparent bar will appear and then slowly fade over time
    private IEnumerator damageBarEffectCoroutine;
    IEnumerator DamageBarEffect(){
        fadeEffectInProgress = true;
        // Making sure the fillAmount of damage effect is correct
        damageEffectImage.fillAmount = (1 - healthperc) + (lastDamage/maxHealth);
        
        // Declaring variables used in Lerp function 
        float elapsedTime = 0;
        damageEffectImage.color = new Color32((byte)damageEffectImage.color.r, (byte)damageEffectImage.color.g, (byte)damageEffectImage.color.b, (byte)0.5f);
        
        while(elapsedTime < DamageDuration){
            elapsedTime += Time.deltaTime;
            damageEffectImage.color = new Color32(255, 255, 255, (byte)Mathf.Lerp(200, 0.0f, elapsedTime/DamageDuration));
            yield return null;
        }
        
        damageEffectImage.color = new Color32((byte)damageEffectImage.color.r, (byte)damageEffectImage.color.g, (byte)damageEffectImage.color.b, (byte)0.0f);
        fadeEffectInProgress = false;
    }



 
    // Starts a coroutine that will lerp the visible health bar to where it should be
    void moveHealthBarFill(){
        if(isRunning == false){
            StartCoroutine(healthbarcoroutine);
        }
        if(fadeEffectInProgress == false){
            StartCoroutine(damageBarEffectCoroutine);
        }
    }




 
    // Used to get how much health we have in terms of percentage
    float calculateHealthBarPercent(){
        return health / maxHealth;
    }




 
    // Self explanatory, takes away health by an ammount (or give more health if you pass a negative argument)
    public void TakeDamage(float Amount){
        health -= Amount;
        lastDamage = Amount;
        moveHealthBarFill();
        if(animRunning == false && Animation == true){
            StartCoroutine(animCoroutine);
        }
        else if(animRunning == true && Animation == true){
            StopCoroutine(animCoroutine);
            StartCoroutine(animCoroutine);
        }
        if(hitSound != null){
            healthBarAC.Stop();
            healthBarAC.Play();
        }
    }




 
    void Start()
    {   
        
        // Instantiating the Healthbar gameobject and making it a child of the gameobject which this script is attached to.
        
        healthBar = Instantiate(Healthbar);
        moveHealthBar();

        // Getting UI elements under Healthbar
        HealthBarUI = healthBar.transform.GetChild(0).gameObject;
        HealthBarImage = HealthBarUI.transform.GetChild(0).transform.GetComponent<Image>();
        damageEffectMask = HealthBarUI.transform.GetChild(HealthBarUI.transform.childCount - 1).transform.GetComponent<Image>();
        damageEffectImage = damageEffectMask.transform.GetChild(0).GetComponent<Image>();

        // Setting up animationAmmountChanged & originscale of healthbar
        animationAmmountChanged = new Vector3(healthBar.transform.localScale.x + animAmmount, healthBar.transform.localScale.y + animAmmount, healthBar.transform.localScale.z + animAmmount);
        healthBarOriginScale = healthBar.transform.localScale;

        // Storing Audio Source component of healthBar & filling in fields inside of it
        healthBarAC = healthBar.GetComponent<AudioSource>();
        if(hitSound != null){
            healthBarAC.clip = hitSound;   
        }
    }
    



 
    void Update()
    {
        // Storing an instance of our coroutines
        healthbarcoroutine = MoveHealthBarCoroutine();
        damageBarEffectCoroutine = DamageBarEffect();
        animCoroutine = AnimationCoroutine();

        // While the damage effect coroutine is not running the damage effect will not be visible
        if(fadeEffectInProgress == false){
            damageEffectImage.color = new Color32((byte)damageEffectImage.color.r, (byte) damageEffectImage.color.g, (byte) damageEffectImage.color.b, (byte) 0);
        }

        // Changing the Mask's fillAmount to hide extra bits of the effect image
        damageEffectMask.fillAmount = HealthBarImage.fillAmount;

        moveHealthBar();
        // Making sure the health doesn't go over max health and below 0
        float clampedHealth = Mathf.Clamp(health, 0, maxHealth);
        health = clampedHealth;

        healthperc = calculateHealthBarPercent();
    }
}
