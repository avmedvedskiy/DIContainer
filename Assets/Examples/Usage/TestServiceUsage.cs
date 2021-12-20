using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Examples
{
    public class TestServiceUsage : MonoBehaviour
    {
        private readonly TestService _testService = Inject.Service<TestService>();
        void Start()
        {
            _testService.Run();
        }

    }
}
