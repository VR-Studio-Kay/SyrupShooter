using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    
    public GameObject imgIndiceIn;
    public GameObject imgIndiceTalk;
    public GameObject imgIndiceOut;

    public GameObject imgIndiceTalkText;
    /*
    public GameObject videoIndiceIn;
    public GameObject videoIndiceTalk;
    public GameObject videoIndiceOut;
    public GameObject canvaIndiceIn;
    public GameObject canvaIndiceTalk;
    public GameObject canvaIndiceOut;
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "IndicesTest")
        {
            Debug.Log("enter collider");

            imgIndiceIn.SetActive(true);
            //canvaIndiceIn.SetActive(true);
            

            StartCoroutine(timerUnSecEnter());

            
            IEnumerator timerUnSecEnter()
            {
                yield return new WaitForSeconds(1f);
                imgIndiceIn.SetActive(false);
                //canvaIndiceIn.SetActive(false);

                imgIndiceTalk.SetActive(true);
                //canvaIndiceTalk.SetActive(true);

                imgIndiceTalkText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "IndicesTest")
        {
            Debug.Log("exit collider");

            imgIndiceTalk.SetActive(false);
            //canvaIndiceTalk.SetActive(false);

            imgIndiceTalkText.SetActive(false);

            imgIndiceOut.SetActive(true);
            //canvaIndiceOut.SetActive(true);

            StartCoroutine(timerUnSecExit());

            
            IEnumerator timerUnSecExit()
            {
                yield return new WaitForSeconds(1f);
                imgIndiceOut.SetActive(false);
                //canvaIndiceOut.SetActive(false);
            }
        }
        
    }

    

}
