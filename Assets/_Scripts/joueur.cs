﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class joueur : MonoBehaviour {

    public float pointDeVie;
    public float pointDeVieMax;
    public float forceSaut = 0.005f;
    public float multiplicateurGravite;
    public float multiplicateurSautMin;
    public float vitesseDeplacement;
    public float distanceRaycast;
    public float distanceRaycastCote;
    public float raycastDecalement;
    public AudioClip SonSaut;
    public Transform arme;
    public List<string> inventaireObjet = new List<string>();
    public List<int> inventaireObjetQte = new List<int>();

    private Rigidbody _RB;
    private Animator _animator;
    private AudioSource _AS;
    private bool _estTouchable = true;
    private bool _estEnLair = false;

    //debugger
    public bool debugVelocite;
    public bool debugX;
    public bool debugY;
    public bool debugZ;
    public bool debugRaycast;

    // Use this for initialization
    void Start() {
        _RB = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _AS = Camera.main.gameObject.GetComponent<AudioSource>();
        arme = gameObject.transform.GetChild(3);
        pointDeVie = pointDeVieMax;
    }

    // Update is called once per frame
    void Update() {
        /******************************************************déplacements************************************************************/
        float deplacementHorizontale = Input.GetAxisRaw("Horizontal");
        //float deplacementVerticale = Input.GetAxisRaw("Vertical");
        Vector3 deplacement = _RB.velocity;
        deplacement.x = deplacementHorizontale * vitesseDeplacement;

        /******************************************************saut**********************************************************************/

        //détection du saut
        if (Input.GetButtonDown("Jump") && _estEnLair == false) {
            _animator.SetBool("saut", true);
            deplacement.y = forceSaut;
            _AS.PlayOneShot(SonSaut, 0.5f);
        }

        // Gestion du saut
        if (_RB.velocity.y < 0) {
            deplacement.y += Physics.gravity.y * (multiplicateurGravite - 1) * Time.deltaTime;
        }
        else if (_RB.velocity.y > 0 && !Input.GetButton("Jump")) {
            deplacement.y += Physics.gravity.y * (multiplicateurSautMin - 1) * Time.deltaTime;
        }

        /******************************************************Orientation*****************************************************************/
        if (Input.GetAxis("Horizontal") < 0) {
            gameObject.transform.eulerAngles = new Vector3(0, -90, 0);
        }
        else if (Input.GetAxis("Horizontal") > 0) {
            gameObject.transform.eulerAngles = new Vector3(0, 90, 0);
        }

        /************************************************Gestion de la détection avec le sol************************************************/
        RaycastHit raycast_0;
        RaycastHit raycast_1;
        RaycastHit raycast_2;

        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.down), out raycast_0, distanceRaycast)) {
            if (debugRaycast) Debug.DrawRay(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.down) * raycast_0.distance, Color.yellow);
            _estEnLair = false;
            _animator.SetBool("saut", false);
            _animator.SetBool("chute", false);
        }
        else if (Physics.Raycast(gameObject.transform.position - (Vector3.right * raycastDecalement), gameObject.transform.TransformDirection(Vector3.down), out raycast_1, distanceRaycastCote)) {
            if (debugRaycast) Debug.DrawRay(gameObject.transform.position - (Vector3.right * raycastDecalement), gameObject.transform.TransformDirection(Vector3.down) * raycast_1.distance, Color.green);
            _estEnLair = false;
            _animator.SetBool("chute", false);
        }
        else if (Physics.Raycast(gameObject.transform.position + (Vector3.right * raycastDecalement), gameObject.transform.TransformDirection(Vector3.down), out raycast_2, distanceRaycastCote)) {
            if (debugRaycast) Debug.DrawRay(gameObject.transform.position + (Vector3.right * raycastDecalement), gameObject.transform.TransformDirection(Vector3.down) * raycast_2.distance, Color.blue);
            _estEnLair = false;
            _animator.SetBool("chute", false);
        }
        else {
            if (debugRaycast) {
                Debug.DrawRay(gameObject.transform.position - (Vector3.right * raycastDecalement), gameObject.transform.TransformDirection(Vector3.down) * distanceRaycastCote, Color.red);
                Debug.DrawRay(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.down) * distanceRaycast, Color.red);
                Debug.DrawRay(gameObject.transform.position + (Vector3.right * raycastDecalement), gameObject.transform.TransformDirection(Vector3.down) * distanceRaycastCote, Color.red);
            };
            _animator.SetBool("chute", true);
            _estEnLair = true;
        }

        /**************************************************combat*******************************************************************************/
        if (Input.GetButton("Gauche") || Input.GetButton("Droite")) {
            arme.gameObject.SetActive(true);
        }

        /**********************************************************débug************************************************************************/
        if (debugVelocite) debug(debugX, debugY, debugZ);

        /**********************************************************déplacements*****************************************************************/
        if (deplacement.x != 0) {
            _animator.SetBool("deplacement", true);
        }
        else {
            _animator.SetBool("deplacement", false);
        }
        _RB.velocity = deplacement;
    }


    /**************************************************Détection des collisions*****************************************************************/
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == "recompense") {
            int resultat = EstDansLinventaire(collision.gameObject.name);
            if (resultat != -1) {
                inventaireObjetQte[resultat]++;
            }
            else {
                inventaireObjet.Insert(inventaireObjet.Count, collision.name);
                inventaireObjetQte.Insert(inventaireObjetQte.Count, 1);
            }
            Destroy(collision.gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "ennemi" && _estTouchable == true) {
            pointDeVie -= collision.gameObject.GetComponent<ennemi>().degats;
            StartCoroutine("DevienIntouchable", 1f);
            VerificationMortTeddy();
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag == "ennemi" && _estTouchable == true) {
            pointDeVie -= collision.gameObject.GetComponent<ennemi>().degats;
            StartCoroutine("DevienIntouchable", 1f);
            VerificationMortTeddy();
        }
    }



    /**************************************************************Fonctions********************************************************************/
    void VerificationMortTeddy(){
        if (pointDeVie <= 0) {
            SceneManager.LoadScene("MainMenu");
        }
    }

    int EstDansLinventaire(string cible) {
        var taille = inventaireObjet.Count;
        for (int i = 0; i < taille; i++) {
            if (inventaireObjet[i] == cible) {
                return i;
            }
        }
        return -1;
    }

    //IENUMERATORS
    private IEnumerator DevienIntouchable(float temps) {
        _estTouchable = false;
        yield return new WaitForSeconds(temps);
        _estTouchable = true;
        StopCoroutine("DevienIntouchable");
    }

    //fonction de débug
    public void debug(bool x, bool y, bool z) {
        if (x) Debug.Log("x:" + _RB.velocity.x);
        if (y) Debug.Log("y:" + _RB.velocity.y);
        if (z) Debug.Log("z:" + _RB.velocity.z);
    }
}