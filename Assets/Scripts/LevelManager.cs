using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public GameObject[] toRemove;
    private Stack<GameObject> remaining;

    public delegate void OnLevelComplete();
    public event OnLevelComplete onLevelComplete;

    private int fliesCollected;
    private float timeElapsed;

    void Start() {
        remaining = new Stack<GameObject>(toRemove);
        onLevelComplete += Temp;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Collector c = player.GetComponent<Collector>();

        c.onCollect += OnCollect;
    }
	
	void Update () {
        timeElapsed += Time.deltaTime;

        if(remaining.Count > 0) {
            try {
		        while(remaining.Peek() == null) {
                    remaining.Pop();
                }
            } catch (InvalidOperationException) {
                onLevelComplete();
            }
        }
	}

    private void Temp() {
        Debug.Log("Level Complete! " + (int)timeElapsed + "s, " + 
            "Flies collected: " + fliesCollected + "/" + toRemove.Length);
    }

    private void OnCollect(GameObject toCollect, string tag) {
        switch (tag) {
            case "Fly":
                // TODO change to objective & use list
                fliesCollected += 1;
                toCollect.tag = "X";
                break;
            default:
                break;
        }
    }
}
