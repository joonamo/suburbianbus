﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStop : MonoBehaviour
{
  public int idx = 4;
  private GameManager gm;
  public GameObject whatIsPerson;
  public float personSpawnDistance = 2.0f;
  public bool introStop = false;
  public MeshRenderer highlighter;

  public void OnGameInit(int idx, GameManager gm)
  {
    if (!introStop) {
      this.idx = idx;
    }
    this.gm = gm;
    var myMesh = transform.parent.gameObject.GetComponent<MeshRenderer>();
    foreach (var material in myMesh.materials)
    {
      material.SetColor("_Color", gm.stopColors[idx]);
    }
    Color highColor = gm.stopColors[this.idx];
    highColor.a = 1.0f;
    highlighter.material.SetColor("_Color", highColor);

    if (!introStop) {
      spawnWaitingPerson();
      spawnWaitingPerson();
      spawnWaitingPerson();
    }
  }

  private PersonAi spawnPerson(Vector3 where, int targetStop, bool wander) {
    var position = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
    var dist = wander ? personSpawnDistance : 1.0f;
    position = where + Vector3.up * 0.5f + position.normalized * dist;
    var spawnedPerson = Instantiate(whatIsPerson, position, Quaternion.identity);
    var personScript = spawnedPerson.GetComponent<PersonAi>();
    personScript.OnInitialize(wander, targetStop, gm, this.gameObject);

    return personScript;
  }

  public void spawnWaitingPerson()
  {
    var targetStop = Random.Range(0, gm.stopColors.Count);
    while (targetStop == idx)
    {
      targetStop = Random.Range(0, gm.stopColors.Count);
    }

    spawnPerson(transform.position, targetStop, true);
  }

  void Start()
  {
    highlighter.material.SetColor("_Color", Color.clear);

    if (introStop) {
      this.gm = GameObject.FindObjectOfType<GameManager>();
      spawnWaitingPerson();
      spawnWaitingPerson();
      spawnWaitingPerson();
    }
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      int n = gm.stopReached(idx);
      for (int i = 0; i < n; ++i) {
        spawnPerson(other.transform.position - other.transform.forward * 3.0f, idx, false);
      }
    }
  }
}
