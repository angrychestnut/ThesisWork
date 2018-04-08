using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace myApp
{
    [RequireComponent(typeof(Networking))]
    public class Movements : MonoBehaviour
    {
        public Vector3 initPos;
        public GameObject myCube;
        public Vector3 speed;
        private Networking networking;

        public void Awake()
        {
            networking = GetComponent<Networking>();
        }

        public void Start()
        {
            myCube.transform.position = this.initPos;
           
        }

        public void Update()
        {
            if (!networking.isServer)
            {
                if (Input.GetKeyDown("up"))
                {
                    speed = new Vector3(1.03F, 0, 0);

                }
                else if (Input.GetKeyDown("down"))
                {
                    speed = new Vector3(-1.22F, 0, 0);
                }
                else
                {
                    speed = new Vector3(0, 0, 0);
                }
                myCube.transform.position = myCube.transform.position + speed;
            }

        }


    }
}

