 using UnityEngine;
 using System.Collections;
 
 public class PSAutoDestroy : MonoBehaviour 
 {
     private ParticleSystem ps;
 
 
     public void Start() {
         ps = GetComponent<ParticleSystem>();
     }
 
     public void Update() {
         if(ps)
         {
             GameObject.Destroy(gameObject, ps.main.duration + ps.main.startLifetimeMultiplier);
         }
     }
 }