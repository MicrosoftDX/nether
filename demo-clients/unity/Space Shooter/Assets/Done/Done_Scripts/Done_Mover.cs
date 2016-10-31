// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using UnityEngine;
using System.Collections;

public class Done_Mover : MonoBehaviour
{
    public float speed;

    private void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}

