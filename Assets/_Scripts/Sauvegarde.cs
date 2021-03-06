﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SystemeEventsLib;

/***
 * scriptableObject permettant de stocker les informations importante pour l'exportation en JSON
 * @author Jimmy Tremblay-Bernier
 */

[System.Serializable]
public class Sauvegarde : ScriptableObject {
    /*liste de chose à sauvegarder dont:
     * Position du joueur
     * Points de vie du joueur
     * Inventaire du joueur
     * Arme du joueur
     * Experience du joueur
     * Si le boss est mort
     * Stats
     */
    public Vector3 positionJoueur;
    public int pointVieJoueur;
    public int pointVieMaxJoueur;
    public string armeEquipe;
    public List<string> inventaireArme = new List<string>();
    public int experienceJoueur;
    public int experienceMaxJoueur;
    public int experienceJoueurTotal;
    public int experienceJoueurNextLevel;
    public int[] stats = new int[4];
    public int niveau;
    public bool ventilationEstBriser;
    public bool f1Estmort = false;
    public bool avion1Estmort = false;
    public bool avion2Estmort = false;
    public int rage;
}
