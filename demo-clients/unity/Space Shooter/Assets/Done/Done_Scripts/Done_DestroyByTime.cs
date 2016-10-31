// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using UnityEngine;
using System.Collections;

public class Done_DestroyByTime : MonoBehaviour
{
    public float lifetime;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}

