using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services.Examples
{
    public class TestService : IService, IInitialize
    {
        void IInitialize.Initialize()
        {
            Debug.Log("TestService Initialized");
            SceneManager.LoadScene(1);
        }

        public void Run()
        {
            Debug.Log("Run");
        }
    }
}
