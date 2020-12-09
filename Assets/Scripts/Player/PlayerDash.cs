﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerDash : MonoBehaviour
{
    [Header("Player rigidbody")]
    [SerializeField] private Rigidbody2D sf_playerRB;

    [Header("Raycast2D origin")]
    [SerializeField] private Transform firePoint;
    
    [SerializeField] private int maxDistance;
    [SerializeField] private int maxDistanceDashInput;

    [Header("Values to change which affect the Dash")]
    [SerializeField] private float sf_dashSpeed;
    [SerializeField] public int sf_dashCooldown;
    
    [Header("Particules")]
    [SerializeField] private ParticleSystem  DeadParticules;
    [SerializeField] private ParticleSystem  DashParticules;
    [SerializeField] private float distanceMinToBeDestroyed;
  
    [Header("UI Text")]
    [SerializeField] private GameObject  sf_DashText;

    [HideInInspector] public bool canJump;
    private int dashCoolDownBuffer;
    private bool dashUI;

    private Vector2 buffVelocity ;

    /// <summary>
    /// Se lance une fois au début
    /// </summary>
    void Start()
    {
        canJump = false;
        dashUI = false;
        dashCoolDownBuffer = sf_dashCooldown;
    }

    /// <summary>
    /// Se lance tout les 0.02 secondes (Basé sur le deltaTime)
    /// </summary>
    void FixedUpdate()
    { 
        dash();
        buffVelocity = sf_playerRB.velocity;

    }

    /// <summary>
    /// Procédure qui regarde si un dash est possible, et si ce dernier est possible, fait dash le player
    /// <remark>
    /// Un dash est une accélaration
    /// </remark>
    /// </summary>
    void dash(){

        if(sf_playerRB != null && firePoint != null ){
            // Permet de récupérer la position de la souris dans la scène
            var worldMousePosition = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.transform.position.z));
            
            dashCoolDownBuffer--;

            //Si le dash devrait être disponible, on le remet à true.
            if(dashCoolDownBuffer < 0 && !canJump){
                dashCoolDownBuffer = sf_dashCooldown;
                canJump = true;
            }

            //Si il appuie sur l'axe "Jump" et qu'il peut sauter
            if(Input.GetAxis("Jump") != 0 && canJump)
            {
                // Direction du dash calculée en fonction de la position de la souris et du personnage
                var direction = worldMousePosition - this.transform.position;
                direction.Normalize();
                
                //On fait un raycast 
                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, sf_playerRB.velocity, maxDistance);
                //Si il touche quelque chose qui peut être détruit
                if(hit && hit.transform.gameObject.tag == "canBeDestroyed" && Vector2.Distance(hit.point, firePoint.position) <= maxDistance ){
                    //On met la cible à not active
                  hit.transform.gameObject.SetActive(false);
                }
                //Le dash n'est plus possible
                canJump = false;
                //On applique une accélération
                sf_playerRB.velocity += sf_dashSpeed * (Vector2)direction;
                //On met les effets et tout le tintouin
                DashParticules.Play();
                playDashSound();

                //On remet le buffer à sa valeur initiale
                dashCoolDownBuffer = sf_dashCooldown;
            }

            printDash();

        }
    
    }
    
    /// <summary>
    /// Affiche si le dash est possible ou non
    /// </summary>
    private void printDash()
    {   
        //On fait un raycast
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, sf_playerRB.velocity, maxDistanceDashInput);
        //Si il touche alors on affiche commme quoi le dash est accecible et qu'il faut penser à l'utiliser
        bool dashUi = hit && hit.transform.gameObject.tag == "canBeDestroyed" ? true : false;            
        sf_DashText.gameObject.SetActive(dashUI);

    }

    /// <summary>
    /// Joue le son du dash
    /// </summary>
    private void playDashSound(){
        this.gameObject.GetComponent<AudioSource>().Play();
    }
  
}
