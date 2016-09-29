// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using UnityEngine;
using System.Collections;

public class Done_DestroyByContact : MonoBehaviour
{
    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;
    private Done_GameController gameController;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<Done_GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        try
        {
            if (other.tag == "Boundary" || other.tag == "Enemy" | other.tag == "EnemyBolt")
            {
                return;
            }

            if (explosion != null)
            {
                Instantiate(explosion, transform.position, transform.rotation);
            }

            if (other.tag == "Player")
            {
                Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                gameController.GameOver();
            }

            gameController.AddScore(scoreValue);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        catch (UnityException ex)
        {
			Debug.LogError("Error in DestroyByContact");
        }
    }
}