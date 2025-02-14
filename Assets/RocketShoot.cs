using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShoot : MonoBehaviour
{
   /* [SerializeField]
        public GameObject projectile;
        [Header("Missile spawns at attached game object")]
        public Transform spawnPosition;
        public float speed = 500;
        RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f)) //Finds the point where you click with the mouse
        {
                        GameObject projectileObject = Instantiate(projectile, spawnPosition.position, Quaternion.identity) as GameObject; //Spawns the selected projectile
                        projectileObject.transform.LookAt(hit.point); //Sets the projectiles rotation to look at the point clicked
                        projectileObject.GetComponent<Rigidbody>().AddForce(projectileObject.transform.forward * speed); //Set the speed of the projectile by applying force to the rigidbody
        }
    }*/
}
