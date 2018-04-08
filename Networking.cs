using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using myApp;


namespace myApp
{
    [RequireComponent(typeof(HandlePacket))]
    public class Networking : MonoBehaviour
    {
        #region Parameters

        public bool isServer = true;
        public bool isHeidi;
        public bool isYanni;
        public static string serverAddress = "10.246.140.81";
        public List<IPEndPoint> ClientList = new List<IPEndPoint>();

        public int s_SendPort = 48199;
        public int s_ReceivePort = 600;
        public int c_SendPort = 9000;
        public int c_ReceivePort = 5000;

        public int ReceivePort;
        public int SendPort;
        public IPEndPoint IPEP;

        public UdpClient Sender;
        public IPEndPoint Objective;
        public UdpClient Receiver;
        public IPEndPoint RefPoint;

        public byte[] buffer = new byte[1024];
        public UInt32 counter = 0;
        public int[] lens = { 24, 16, 16, 208, 16 };
        public GameObject theGO;
        public GameObject heidi;
        public GameObject yanni;
        public RingBuffer MsgQueue = new RingBuffer(100);
        private HandlePacket hp;


        #endregion

        #region DetailFunctions

        public void StartToReceive()
        {
            this.Receiver.BeginReceive(OnReceive, null);
        }

        public byte[] ClientWritePacket(GameObject go, UInt32 count)
        {
            Vector3 pos = go.transform.position;
            Vector3 speed = new Vector3(0, 0, 0);
            Vector3 accel = new Vector3(0, 0, 0);
            Vector3 geo = new Vector3(0, 0, 0);
            HandlePacket.coord mypos = HandlePacket.Catch.CatchCoord(pos);
            HandlePacket.coord myspeed = HandlePacket.Catch.CatchCoord(speed);
            HandlePacket.coord myaccel = HandlePacket.Catch.CatchCoord(accel);
            HandlePacket.geo mygeo = HandlePacket.Catch.CatchGeo(geo);
            buffer = HandlePacket.Catch.CatchPacket(mygeo, mypos, myspeed, myaccel, count);
            return buffer;
        }

        public HandlePacket.Packet ParsePacket(byte[] stream, int[] lens)
        {
            HandlePacket.Packet pkt = new HandlePacket.Packet();
            HandlePacket.Packet pkt_n = pkt.Parse(stream, lens);
            return pkt_n;
        }

        public IPEndPoint Conversion(IPEndPoint ipep)
        {
            if (isServer)
            {
                IPEP = new IPEndPoint(ipep.Address, c_ReceivePort);
            }
            else
            {
                IPEP = new IPEndPoint(ipep.Address, s_ReceivePort);
            }
            return (IPEP);
        }

        public void AddClient(IPEndPoint IPEP)
        {
            if (ClientList.Contains(IPEP) == false)
            {
                ClientList.Add(IPEP);
                Debug.Log("Newly registered" + IPEP.ToString());
            }
        }

        public void RemoveClient(IPEndPoint ipep)
        {
            ClientList.Remove(ipep);
        }

        public void BroadcastMessage(byte[] msg, List<IPEndPoint> list)
        {
            foreach (var ip in ClientList)
            {
                Sender.Client.BeginSendTo(msg, 0, msg.Length, SocketFlags.None, ip, OnSend, null);
                Debug.Log("To " + ip.ToString());
            }
        }

        public char[] GetName()
        {
            char[] thename = new char[32];
            thename[1] = 'h';
            thename[2] = 'e';
            thename[3] = 'i';
            thename[4] = 'd';
            thename[5] = 'i';
            return thename;
        }

        #endregion

        #region ImportantFunctions

        void Awake()
        {
            hp = GetComponent<HandlePacket>();
            if (isServer)
            {
                this.ReceivePort = s_ReceivePort;
                this.SendPort = s_SendPort;
                Debug.Log("Server started, servicing on port {0}:" + this.ReceivePort);
                this.Objective = new IPEndPoint(IPAddress.Any, c_ReceivePort);
                this.RefPoint = new IPEndPoint(IPAddress.Any, c_SendPort);
            }
            else
            {
                this.ReceivePort = c_ReceivePort;
                this.SendPort = c_SendPort;
                Debug.Log("This is a client");
                this.Objective = new IPEndPoint(IPAddress.Parse(serverAddress), s_ReceivePort);
                this.RefPoint = new IPEndPoint(IPAddress.Parse(serverAddress), s_SendPort);
                this.ClientList.Add(this.Objective);
            }
            this.Sender = new UdpClient(this.SendPort);
            this.Receiver = new UdpClient(this.ReceivePort);
            // Receiving Message
            this.StartToReceive();
        }

        //void Init()
        //{
        //    //DontDestroyOnLoad(gameObject);
        //}

        void Update()
        {
            while (MsgQueue.queue.Count != 0)
            {
                HandlePacket.Packet reader = MsgQueue.Read();
                if (reader != null)
                {
                    MySyncVar(reader);
                }
            }

            if (!isServer) 
            {
                counter++;
                buffer = ClientWritePacket(theGO, counter);
                BroadcastMessage(buffer, ClientList);
            }
        }

        public void OnApplicationQuit()
        {
            Sender.Close();
            Receiver.Close();
        }

        public void OnDisable()
        {
            Sender.Close();
            Receiver.Close();
        }

        public void DisplayPacket(HandlePacket.header_c C, HandlePacket.state state)
        {
            Debug.Log("packet " + C.frameNo + ": x position is" + state.state_base.pos.x);
        }

        public void OnReceive(IAsyncResult ar)
        {
            try
            {
                buffer = Receiver.EndReceive(ar, ref RefPoint);
                Debug.Log("End Receiving");
                // Add clients
                IPEP = Conversion(RefPoint);
                AddClient(IPEP);
                if (isServer)
                {
                    BroadcastMessage(buffer, ClientList);                   
                }

                HandlePacket.Packet pkt_n = ParsePacket(buffer, lens);
                MsgQueue.Add(pkt_n);

                // Start to receive again
                StartToReceive();
            }
            catch (ArgumentException)
            {

            }
        }

        public void OnSend(IAsyncResult ar)
        {
            try
            {
                Sender.EndSend(ar);
                Debug.Log("End Sending");
            }
            catch (ArgumentException)
            {

            }
        }

        public void MySyncVar(HandlePacket.Packet pkt)
        {
            HandlePacket.coord kk = pkt.State.state_base.pos;
            if (!isServer)
            {
                if (pkt.State.state_base.name.ToString() != hp.myname.ToString())
                {
                    if(isYanni)
                    {
                        heidi.transform.position = new Vector3(Convert.ToSingle(kk.x), Convert.ToSingle(kk.y), Convert.ToSingle(kk.z));
                    }
                    if(isHeidi)
                    {
                        yanni.transform.position = new Vector3(Convert.ToSingle(kk.x), Convert.ToSingle(kk.y), Convert.ToSingle(kk.z));
                    }
                }
            }
            else
            {
                //DisplayPacket(pkt.C, pkt.State);
                if (pkt.State.state_base.name.ToString() == hp.myname.ToString())
                {
                    yanni.transform.position = new Vector3(Convert.ToSingle(kk.x), Convert.ToSingle(kk.y), Convert.ToSingle(kk.z));
                }
            }
        }

        #endregion

    }

    public class RingBuffer
    {
        public Queue<HandlePacket.Packet> queue;
        public int size;

        #region Constructor
        public RingBuffer(int size)
        {
            this.queue = new Queue<HandlePacket.Packet>(size);
            this.size = size;
        }
        #endregion

        #region API

        public void Add(HandlePacket.Packet newItem)
        {
            if (queue.Count == this.size)
            {
                queue.Dequeue();
            }
            queue.Enqueue(newItem);
        }

        public HandlePacket.Packet Read()
        {
            if (queue.Count != 0)
            {
                return queue.Dequeue();
            }
            else
            {
                return null;
            }
        }

        public HandlePacket.Packet Peek()
        {
            return queue.Peek();
        }

        #endregion
    }
}

