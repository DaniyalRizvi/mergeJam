using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupHandler : Singelton<PowerupHandler>
{
    public GameObject Fan;
    public GameObject Jump;
    public GameObject Rocket;
    // Start is called before the first frame update
    public void SetPanel()
    {
        Fan.gameObject.SetActive(false);
        Jump.gameObject.SetActive(false);
        Rocket.gameObject.SetActive(false);
            
         Debug.Log("Current Level T: "+PlayerPrefs.GetInt("CurrentLevel"));
            
        if(PlayerPrefs.GetInt("CurrentLevel")>=35)
            Fan.gameObject.SetActive(true);
        if(PlayerPrefs.GetInt("CurrentLevel")>=13)
            Jump.gameObject.SetActive(true);
        if(PlayerPrefs.GetInt("CurrentLevel")>=27)
            Rocket.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
