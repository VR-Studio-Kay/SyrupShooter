using UnityEngine;
using System.Collections;

public class intro_jeu : MonoBehaviour
{
    public Animator WagonAnimator;
    public Animator TunnelAnimator;
    public GameObject start;
    public GameObject title;
    public GameObject porte;
    public GameObject introTrigger;
    public GameObject candycorn;
    //public Bool trigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WagonAnimator.SetFloat("speed", 0);
	    TunnelAnimator.GetComponent<Animator>().Play("tunnel_intro");
    }

    /*private void OnTriggerEnter(Collider other){

    if(other.CompareTag("introTrigger")){
        WagonAnimator.SetFloat("speed", 1);
        TunnelAnimator.GetComponent<Animator>().enabled = false;
        introTrigger.SetActive(false);
        porte.SetActive(false);
        start.SetActive(false);
        title.SetActive(true);
            }

}*/

    public void allo(){
        WagonAnimator.SetFloat("speed", 1);
        TunnelAnimator.GetComponent<Animator>().enabled = false;
        introTrigger.SetActive(false);
        porte.SetActive(false);
        start.SetActive(false);
        title.SetActive(true);
        Debug.Log("yo");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
