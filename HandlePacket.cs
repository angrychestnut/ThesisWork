﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace myApp
{
    public class HandlePacket : MonoBehaviour
    {

        public char[] myname = new char[32];
        public UInt32[] spare = new UInt32[3];
        public static HandlePacket hp = new HandlePacket();

        /** ------ header of a complete message ------ */
        public class header_c
        {
            public UInt16 magicNo;      /**< must be RDB_MAGIC_NO (35712)                                               @unit @link GENERAL_DEFINITIONS @endlink   @version 0x0100 */
            public UInt16 version;      /**< upper byte = major, lower byte = minor                                     @unit _                                    @version 0x0100 */
            public UInt32 headerSize;   /**< size of this header structure when transmitted                             @unit byte                                 @version 0x0100 */
            public UInt32 dataSize;     /**< size of data following the header                                          @unit byte                                 @version 0x0100 */
            public UInt32 frameNo;      /**< number of the simulation frame                                             @unit _                                    @version 0x0100 */
            public double simTime;      /**< simulation time                                                            @unit s                                    @version 0x0100 */

            public header_c(UInt32 dataSize, UInt32 frameNo)
            {
                this.magicNo = 35712;
                this.version = 1000;   // no idea
                this.headerSize = 24;
                this.dataSize = dataSize;
                this.frameNo = frameNo;
                this.simTime = DateTime.Now.ToOADate();
            }

            public header_c(byte[] DataStream)
            {
                this.magicNo = BitConverter.ToUInt16(DataStream, 0);
                this.version = BitConverter.ToUInt16(DataStream, 2);
                this.headerSize = BitConverter.ToUInt32(DataStream, 4);
                this.dataSize = BitConverter.ToUInt32(DataStream, 8);
                this.frameNo = BitConverter.ToUInt32(DataStream, 12);
                this.simTime = BitConverter.ToDouble(DataStream, 16);
            }
        }

        /** ------ header of a package vector within a message ------ */
        public class header_m
        {
            public UInt32 headerSize;   /**< size of this header structure when transmitted                              @unit byte                     @version 0x0100 */
            public UInt32 dataSize;     /**< size of data following the header                                           @unit byte                     @version 0x0100 */
            public UInt32 elementSize;  /**< if data following the header contains an array of elements of equal size:
							size of one element in this data
							(elementSize is equivalent to dataSize if only one element is transmitted)  @unit byte                         @version 0x0100 */
            public UInt16 pkgId;        /**< package identifier                                                          @unit _                            @version 0x0100 */
            public UInt16 flags;        /**< various flags concerning the package's contents (e.g. extension)            @unit @link RDB_PKG_FLAG @endlink  @version 0x0100 */

            public header_m(UInt32 dataSize, UInt32 elementSize, UInt16 pkgId, UInt16 flags)
            {
                this.headerSize = 16;
                this.dataSize = dataSize;
                this.elementSize = elementSize;
                this.pkgId = pkgId;
                this.flags = flags;
            }

            public header_m(byte[] DataStream)
            {
                this.headerSize = BitConverter.ToUInt32(DataStream, 0);
                this.dataSize = BitConverter.ToUInt32(DataStream, 4);
                this.elementSize = BitConverter.ToUInt32(DataStream, 8);
                this.pkgId = BitConverter.ToUInt16(DataStream, 12);
                this.flags = BitConverter.ToUInt16(DataStream, 14);
            }
        }

        /** ------ geometry information for an object --- */
        public class geo
        {
            public float dimX;        /**< x dimension in object co-ordinates (length)                                               @unit m                                  @version 0x0100 */
            public float dimY;        /**< y dimension in object co-ordinates (width)                                                @unit m                                  @version 0x0100 */
            public float dimZ;        /**< z dimension in object co-ordinates (height)                                               @unit m                                  @version 0x0100 */
            public float offX;        /**< x distance from ref. point to center of geometry, object co-ordinate system               @unit m                                  @version 0x0100 */
            public float offY;        /**< y distance from ref. point to center of geometry, object co-ordinate system               @unit m                                  @version 0x0100 */
            public float offZ;        /**< z distance from ref. point to center of geometry, object co-ordinate system               @unit m                                  @version 0x0100 */

            public geo(float dimX, float dimY, float dimZ, float offX, float offY, float offZ)
            {
                this.dimX = dimX;
                this.dimY = dimY;
                this.dimZ = dimZ;
                this.offX = offX;
                this.offY = offY;
                this.offZ = offZ;
            }

            public geo(byte[] DataStream)
            {
                this.dimX = BitConverter.ToSingle(DataStream, 0);
                this.dimY = BitConverter.ToSingle(DataStream, 4);
                this.dimZ = BitConverter.ToSingle(DataStream, 8);
                this.offX = BitConverter.ToSingle(DataStream, 12);
                this.offY = BitConverter.ToSingle(DataStream, 16);
                this.offZ = BitConverter.ToSingle(DataStream, 20);
            }

            public geo ReturnGeo(geo mygeo)
            {
                return mygeo;
            }
        }

        /** ------ generic co-ordinate structure --- */
        public class coord
        {
            public double x;       /**< x position                                                @unit m                                @version 0x0100 */
            public double y;       /**< y position                                                @unit m                                @version 0x0100 */
            public double z;       /**< z position                                                @unit m                                @version 0x0100 */
            public float h;       /**< heading angle                                             @unit rad                              @version 0x0100 */
            public float p;       /**< pitch angle                                               @unit rad                              @version 0x0100 */
            public float r;       /**< roll angle                                                @unit rad                              @version 0x0100 */
            public byte flags;   /**< co-ordinate flags                                         @unit @link RDB_COORD_FLAG @endlink    @version 0x0100 */
            public byte type;    /**< co-ordinate system type identifier                        @unit @link RDB_COORD_TYPE @endlink    @version 0x0100 */
            public UInt16 system;  /**< unique ID of the corresponding (user) co-ordinate system  @unit _                                @version 0x0100 */

            public coord(double x, double y, double z, float h, float p, float r, byte flags, byte type, UInt16 system)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.h = h;
                this.p = p;
                this.r = r;
                this.flags = flags;
                this.type = type;
                this.system = system;
            }

            public coord(byte[] DataStream)
            {
                this.x = BitConverter.ToDouble(DataStream, 0);
                this.y = BitConverter.ToDouble(DataStream, 8);
                this.z = BitConverter.ToDouble(DataStream, 16);
                this.h = BitConverter.ToSingle(DataStream, 24);
                this.p = BitConverter.ToSingle(DataStream, 28);
                this.r = BitConverter.ToSingle(DataStream, 32);
                this.flags = DataStream[36];
                this.type = DataStream[37];
                this.system = BitConverter.ToUInt16(DataStream, 38);
            }

            public coord ReturnCoord(coord mycoord)
            {
                return mycoord;
            }
        }

        /** ------ state of an object (may be extended by the next structure) ------- */
        public class state_o
        {
            public UInt32 id;                         /**< unique object ID                                              @unit _                                  @version 0x0100 */
            public byte category;                   /**< object category                                               @unit @link RDB_OBJECT_CATEGORY @endlink @version 0x0100 */
            public byte type;                       /**< object type                                                   @unit @link RDB_OBJECT_TYPE     @endlink @version 0x0100 */
            public UInt16 visMask;                    /**< visibility mask                                               @unit @link RDB_OBJECT_VIS_FLAG @endlink @version 0x0100 */
            public char[] name = new char[32];                       /**< symbolic name                                                 @unit _                                  @version 0x0100 */
            public geo geo;                        /**< info about object's geometry                                  @unit m,m,m,m,m,m                        @version 0x0100 */
            public coord pos;                        /**< position and orientation of object's reference point          @unit m,m,m,rad,rad,rad                  @version 0x0100 */
            public UInt32 parent;                     /**< unique ID of parent object                                    @unit _                                  @version 0x0100 */
            public UInt16 cfgFlags;                   /**< configuration flags                                           @unit @link RDB_OBJECT_CFG_FLAG @endlink @version 0x0100 */
            public UInt16 cfgModelId;                 /**< visual model ID (configuration parameter)                     @unit _                                  @version 0x0100 */
            public byte[] geo_b = new byte[24];
            public byte[] coord_b = new byte[40];
            public byte[] name_b = new byte[64];

            public state_o(UInt32 id, byte category, byte type, UInt16 visMask, char[] name, geo geo, coord pos, UInt32 parent, UInt16 cfgFlags, UInt16 cfgModelId)
            {
                this.id = id;
                this.category = category;
                this.type = type;
                this.visMask = visMask;
                this.name = name;
                this.geo = geo;
                this.pos = pos;
                this.parent = parent;
                this.cfgFlags = cfgFlags;
                this.cfgModelId = cfgModelId;

            }

            public state_o(byte[] DataStream)
            {
                this.id = BitConverter.ToUInt32(DataStream, 0);
                this.category = DataStream[4];
                this.type = DataStream[5];
                this.visMask = BitConverter.ToUInt16(DataStream, 6);
                Buffer.BlockCopy(DataStream, 8, name_b, 0, 32);
                Buffer.BlockCopy(DataStream, 40, geo_b, 0, 24);
                Buffer.BlockCopy(DataStream, 64, coord_b, 0, 40);
                this.name = Encoding.ASCII.GetString(name_b).ToCharArray();
                this.geo = new geo(geo_b);
                this.pos = new coord(coord_b);
                this.parent = BitConverter.ToUInt32(DataStream, 104);
                this.cfgFlags = BitConverter.ToUInt16(DataStream, 108);
                this.cfgModelId = BitConverter.ToUInt16(DataStream, 110);
            }
        }

        /** ------ extended object data (e.g. for dynamic objects) ------- */
        public class state_e
        {
            public coord speed;                      /**< speed and rates                                               @unit m/s,m/s,m/s,rad/s,rad/s,rad/s          @version 0x0100 */
            public coord accel;                      /**< acceleration                                                  @unit m/s2,m/s2,m/s2,rad/s2,rad/s2/rad/s2    @version 0x0100 */
            public float traveledDist;               /**< traveled distance                                             @unit m                                      @version 0x011a */
            public UInt32[] spare = new UInt32[3];                   /**< reserved for future use                                       @unit _                                      @version 0x0100 */
            public byte[] speed_b = new byte[40];
            public byte[] accel_b = new byte[40];

            public state_e(coord speed, coord accel, float traveledDist, UInt32[] spare)
            {
                this.speed = speed;
                this.accel = accel;
                this.traveledDist = traveledDist;
                this.spare = spare;
            }

            public state_e(byte[] DataStream)
            {
                Buffer.BlockCopy(DataStream, 0, speed_b, 0, 40);
                Buffer.BlockCopy(DataStream, 40, accel_b, 0, 40);
                this.speed = new coord(speed_b);
                this.accel = new coord(accel_b);
                this.traveledDist = BitConverter.ToSingle(DataStream, 80);
                this.spare[0] = BitConverter.ToUInt32(DataStream, 84);
                this.spare[1] = BitConverter.ToUInt32(DataStream, 88);
                this.spare[2] = BitConverter.ToUInt32(DataStream, 92);
            }
        }

        /** ------ complete object data (basic and extended info) ------- */
        public class state
        {
            public state_o state_base;           /**< state of an object     @unit RDB_OBJECT_STATE_BASE_t   @version 0x0100 */
            public state_e state_ext;            /**< extended object data   @unit RDB_OBJECT_STATE_EXT_t    @version 0x0100 */
            public byte[] state_base_b = new byte[112];
            public byte[] state_ext_b = new byte[96];

            public state(state_o state_base, state_e state_ext)
            {
                this.state_base = state_base;
                this.state_ext = state_ext;
            }

            public state(byte[] DataStream)
            {
                Buffer.BlockCopy(DataStream, 0, state_base_b, 0, 112);
                Buffer.BlockCopy(DataStream, 112, state_ext_b, 0, 96);
                this.state_base = new state_o(state_base_b);
                this.state_ext = new state_e(state_ext_b);
            }
        }

        public class Serialization
        {
            public byte[] Serialize(header_c header_C)
            {
                List<byte> DataStream = new List<byte>();
                DataStream.AddRange(BitConverter.GetBytes(header_C.magicNo));
                DataStream.AddRange(BitConverter.GetBytes(header_C.version));
                DataStream.AddRange(BitConverter.GetBytes(header_C.headerSize));
                DataStream.AddRange(BitConverter.GetBytes(header_C.dataSize));
                DataStream.AddRange(BitConverter.GetBytes(header_C.frameNo));
                DataStream.AddRange(BitConverter.GetBytes(header_C.simTime));
                return DataStream.ToArray();
            }

            public byte[] Serialize(header_m header_M)
            {
                List<byte> DataStream = new List<byte>();
                DataStream.AddRange(BitConverter.GetBytes(header_M.headerSize));
                DataStream.AddRange(BitConverter.GetBytes(header_M.dataSize));
                DataStream.AddRange(BitConverter.GetBytes(header_M.elementSize));
                DataStream.AddRange(BitConverter.GetBytes(header_M.pkgId));
                DataStream.AddRange(BitConverter.GetBytes(header_M.flags));
                return DataStream.ToArray();
            }

            public byte[] Serialize(geo geo)
            {
                List<byte> DataStream = new List<byte>();
                DataStream.AddRange(BitConverter.GetBytes(geo.dimX));
                DataStream.AddRange(BitConverter.GetBytes(geo.dimY));
                DataStream.AddRange(BitConverter.GetBytes(geo.dimZ));
                DataStream.AddRange(BitConverter.GetBytes(geo.offX));
                DataStream.AddRange(BitConverter.GetBytes(geo.offY));
                DataStream.AddRange(BitConverter.GetBytes(geo.offZ));
                return DataStream.ToArray();
            }

            public byte[] Serialize(coord coord)
            {
                List<byte> DataStream = new List<byte>();
                DataStream.AddRange(BitConverter.GetBytes(coord.x));
                DataStream.AddRange(BitConverter.GetBytes(coord.y));
                DataStream.AddRange(BitConverter.GetBytes(coord.z));
                DataStream.AddRange(BitConverter.GetBytes(coord.h));
                DataStream.AddRange(BitConverter.GetBytes(coord.p));
                DataStream.AddRange(BitConverter.GetBytes(coord.r));
                DataStream.AddRange(BitConverter.GetBytes(coord.flags));
                DataStream.AddRange(BitConverter.GetBytes(coord.type));
                DataStream.AddRange(BitConverter.GetBytes(coord.system));
                DataStream.RemoveAt(37);
                DataStream.RemoveAt(38);
                return DataStream.ToArray();
            }

            public byte[] Serialize(state_o state_O)
            {
                List<byte> DataStream = new List<byte>();
                DataStream.AddRange(BitConverter.GetBytes(state_O.id));
                DataStream.AddRange(BitConverter.GetBytes(state_O.category));
                DataStream.AddRange(BitConverter.GetBytes(state_O.type));
                DataStream.AddRange(BitConverter.GetBytes(state_O.visMask));
                byte[] realname = Encoding.ASCII.GetBytes(state_O.name);
                DataStream.AddRange(realname);
                byte[] realgeo = Serialize(state_O.geo);
                DataStream.AddRange(realgeo);
                byte[] realpos = Serialize(state_O.pos);
                DataStream.AddRange(realpos);
                DataStream.AddRange(BitConverter.GetBytes(state_O.parent));
                DataStream.AddRange(BitConverter.GetBytes(state_O.cfgFlags));
                DataStream.AddRange(BitConverter.GetBytes(state_O.cfgModelId));
                DataStream.RemoveAt(5);
                DataStream.RemoveAt(6);
                return DataStream.ToArray();
            }

            public byte[] Serialize(state_e state_E)
            {
                List<byte> DataStream = new List<byte>();
                byte[] realspeed = Serialize(state_E.speed);
                DataStream.AddRange(realspeed);
                byte[] realaccel = Serialize(state_E.accel);
                DataStream.AddRange(realaccel);
                DataStream.AddRange(BitConverter.GetBytes(state_E.traveledDist));
                DataStream.AddRange(BitConverter.GetBytes(state_E.spare[0]));
                DataStream.AddRange(BitConverter.GetBytes(state_E.spare[1]));
                DataStream.AddRange(BitConverter.GetBytes(state_E.spare[2]));
                return DataStream.ToArray();
            }

            public byte[] Serialize(state state)
            {
                List<byte> DataStream = new List<byte>();
                byte[] realstate_base = Serialize(state.state_base);
                DataStream.AddRange(realstate_base);
                byte[] realstate_ext = Serialize(state.state_ext);
                DataStream.AddRange(realstate_ext);
                return DataStream.ToArray();
            }

        }

        public class Packet
        {
            public byte[] myArray = new byte[1024];
            public byte[][] b = new byte[1024][];
            public Serialization serialization = new Serialization();
            public header_c C;
            public header_m[] _M = new header_m[3];
            public state State;

            public Packet(header_c C, header_m M0, header_m M1, state State, header_m M2)
            {
                this.C = C;
                this._M[0] = M0;
                this._M[1] = M1;
                this._M[2] = M2;
                this.State = State;
            }

            public Packet()
            {

            }

            public byte[] Combine(params byte[][] arrays)
            {
                int offset = 0;
                foreach (byte[] array in arrays)
                {
                    Buffer.BlockCopy(array, 0, myArray, offset, array.Length);
                    offset = offset + array.Length;
                }
                return myArray;
            }

            public byte[][] Split(byte[] DataStream, int[] lens)
            {
                int offset = 0;
                for (int i = 0; i < lens.Length; i++)
                {
                    b[i] = new byte[lens[i]];
                    Buffer.BlockCopy(DataStream, offset, b[i], 0, lens[i]);
                    offset = offset + lens[i];
                }
                return b;
            }

            public byte[] FormPacketArray(header_c header_C, header_m header_M0, header_m header_M1, state state, header_m header_M2)
            {
                byte[] a1 = serialization.Serialize(header_C);
                byte[] a2 = serialization.Serialize(header_M0);
                byte[] a3 = serialization.Serialize(header_M1);
                byte[] a4 = serialization.Serialize(state);
                byte[] a5 = serialization.Serialize(header_M2);
                myArray = Combine(a1, a2, a3, a4, a5);
                return myArray;
            }

            public Packet Parse(byte[] DataStream, int[] lens)
            {
                b = Split(DataStream, lens);
                header_c _header_C = new header_c(b[0]);
                header_m _header_M0 = new header_m(b[1]);
                header_m _header_M1 = new header_m(b[2]);
                state _state = new state(b[3]);
                header_m _header_M2 = new header_m(b[4]);
                Packet pkt = new Packet(_header_C, _header_M0, _header_M1, _state, _header_M2);
                return pkt;
            }

            //public void DisplayPacket(header_c C, state state)
            //{
            //    Debug.Log("packet "+C.frameNo+ ": x position is"+ state.state_base.pos.x);
            //}

            public void DisplayPacket(header_c C)
            {
                Console.WriteLine(C.magicNo);
                Console.WriteLine(C.version);
                Console.WriteLine(C.headerSize);
                Console.WriteLine(C.dataSize);
                Console.WriteLine(C.frameNo);
                Console.WriteLine(DateTime.FromOADate(C.simTime));
                Console.WriteLine(Environment.NewLine);
            }

            public void DisplayPacket(header_m M)
            {
                Console.WriteLine(M.headerSize);
                Console.WriteLine(M.dataSize);
                Console.WriteLine(M.elementSize);
                Console.WriteLine(M.pkgId);
                Console.WriteLine(M.flags);
                Console.WriteLine(Environment.NewLine);
            }

            public void DisplayPacket(state_o state_O)
            {
                //Console.WriteLine(state_O.geo.dimX);
                //Console.WriteLine(state_O.geo.dimY);
                //Console.WriteLine(state_O.geo.dimZ);
                //Console.WriteLine(state_O.geo.offX);
                //Console.WriteLine(state_O.geo.offY);
                //Console.WriteLine(state_O.geo.offZ);
                //Console.WriteLine(state_O.name);
                //Console.WriteLine(state_O.cfgModelId);
                //Console.WriteLine(Environment.NewLine);
            }

            public void DisplayPacket(state state)
            {
                DisplayPacket(state.state_base);
                DisplayPacket(state.state_ext);
            }

            public void DisplayPacket(state_e state_E)
            {
                Console.WriteLine(state_E.speed.x);
                Console.WriteLine(state_E.accel.p);
                Console.WriteLine(state_E.spare[2]);
                Console.WriteLine(Environment.NewLine);
            }

        }

        public class Catch
        {
            public static coord CatchCoord(Vector3 coord)
            {
                coord mycoord = new coord(coord.x, coord.y, coord.z, 0, 0, 0, 0, 0, 0);
                return mycoord;
            }

            public static geo CatchGeo(Vector3 geo)
            {
                geo mygeo = new geo(geo.x, geo.y, geo.z, 0, 0, 0);
                return mygeo;
            }

            public static byte[] CatchPacket(geo geo, coord pos, coord speed, coord accel, UInt32 counter)
            {
                Packet pkt = new Packet();
                header_m[] header_M = new header_m[3];


                state_o state_O = new state_o(3, 255, 255, 3, hp.myname, geo, pos, 3, 3, 8);
                state_e state_E = new state_e(speed, accel, 498.55f, hp.spare);
                state state = new state(state_O, state_E);
                header_M[2] = new header_m(0, 0, 2, 99);
                header_M[1] = new header_m(208 + header_M[2].headerSize, 208 + header_M[2].headerSize, 9, 99);
                header_M[0] = new header_m(header_M[1].headerSize + header_M[1].dataSize, header_M[1].headerSize + header_M[1].dataSize, 1, 99);
                header_c header_C = new header_c(header_M[0].headerSize + header_M[0].dataSize, counter);
                int[] lens = { (int)header_C.headerSize, (int)header_M[0].headerSize, (int)header_M[1].headerSize, (int)header_M[1].dataSize - (int)header_M[2].headerSize, (int)header_M[2].headerSize };
                byte[] stream = pkt.FormPacketArray(header_C, header_M[0], header_M[1], state, header_M[2]);
                return stream;
            }


        }


        //    //public static void PrintByteArray(byte[] bytes)
        //    //{
        //    //    var sb = new StringBuilder("new byte[] { ");
        //    //    foreach (var b in bytes)
        //    //    {
        //    //        sb.Append(b + ", ");
        //    //    }
        //    //    sb.Append("}");
        //    //    Console.WriteLine(sb.ToString());
        //    //}


        //    //    


    }
}

