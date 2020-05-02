using OpenToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Aginar.VoxelEngine.Lighting
{
    public static class LightStorage
    {
        private static Queue<LightNode> lightNodes = new Queue<LightNode>();
        private static Queue<LightRemovalNode> lightRemovalNodes = new Queue<LightRemovalNode>();

        public static void SetLight(int localIndex, Chunk blockChunk)
        {
            lightNodes.Enqueue(new LightNode() {
                chunk = blockChunk,
                index = localIndex
                });

            while (lightNodes.Count > 0)
            {
                LightNode node = lightNodes.Dequeue();

                int index = node.index;
                Chunk chunk = node.chunk;

                Vector3i pos = World.IndexToLocalPos(index);

                int lightLevel = chunk.lights.GetLightInt(index);

                if (lightLevel > 0)
                {
                    int shift = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (((index >> shift) & World.CHUNK_MASK) != 0)
                        {
                            int light = chunk.lights.GetLightInt(index - (1 << shift));
                            bool enqueue = false;

                            if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                            {
                                enqueue = true;
                                chunk.lights.SetLight(index - (1 << shift), SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                            }

                            //if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                            //{
                            //    chunk.lights.SetLight(index - (1 << shift), SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                            //    enqueue = true;
                            //}

                            //if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                            //{
                            //    enqueue = true;
                            //    chunk.lights.SetLight(index - (1 << shift), SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                            //}


                            if (enqueue && chunk[index - (1 << shift)] == 0)
                                lightNodes.Enqueue(new LightNode()
                                {
                                    chunk = chunk,
                                    index = index - (1 << shift)
                                });
                        }
                        if (((index >> shift) & World.CHUNK_MASK) != World.CHUNK_MASK)
                        {
                            int light = chunk.lights.GetLightInt(index + (1 << shift));
                            bool enqueue = false;

                            if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                            {
                                enqueue = true;
                                chunk.lights.SetLight(index + (1 << shift), SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                            }

                            //if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                            //{
                            //    chunk.lights.SetLight(index + (1 << shift), SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                            //    enqueue = true;
                            //}

                            //if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                            //{
                            //    enqueue = true;
                            //    chunk.lights.SetLight(index + (1 << shift), SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                            //}


                            if (enqueue && chunk[index + (1 << shift)] == 0)
                                lightNodes.Enqueue(new LightNode()
                                {
                                    chunk = chunk,
                                    index = index + (1 << shift)
                                });
                        }
                        shift += World.LOG_CHUNK_SIZE;
                    }

                    
                }
            }
        }

        public static void RemoveLight(int localIndex, Chunk blockChunk)
        {
            lightRemovalNodes.Enqueue(new LightRemovalNode()
            {
                chunk = blockChunk,
                index = localIndex,
                value = blockChunk.lights.GetLightInt(localIndex)
            });

            while (lightRemovalNodes.Count > 0)
            {
                LightRemovalNode node = lightRemovalNodes.Dequeue();

                int index = node.index;
                Chunk chunk = node.chunk;
                int value = node.value;

                int lightLevel = chunk.lights.GetLightInt(index);

                if ((index & World.CHUNK_MASK) != 0)
                {
                    int light = chunk.lights.GetLightInt(index - 1);
                    bool enqueue = false;

                    if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index - 1, SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                    }

                    if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                    {
                        chunk.lights.SetLight(index - 1, SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                        enqueue = true;
                    }

                    if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index - 1, SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                    }


                    if (enqueue)
                        if (chunk[index - 1] == 0)
                            lightNodes.Enqueue(new LightNode()
                            {
                                chunk = chunk,
                                index = index - 1
                            });
                }
                if ((index & World.CHUNK_MASK) != World.CHUNK_MASK)
                {
                    int light = chunk.lights.GetLightInt(index + 1);
                    bool enqueue = false;

                    if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index + 1, SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                    }

                    if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                    {
                        chunk.lights.SetLight(index + 1, SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                        enqueue = true;
                    }

                    if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index + 1, SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                    }


                    if (enqueue)
                        if (chunk[index + 1] == 0)
                            lightNodes.Enqueue(new LightNode()
                            {
                                chunk = chunk,
                                index = index + 1
                            });
                }

                if (((index >> World.LOG_CHUNK_SIZE) & World.CHUNK_MASK) != 0)
                {
                    int light = chunk.lights.GetLightInt(index - World.CHUNK_SIZE);
                    bool enqueue = false;

                    if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index - World.CHUNK_SIZE, SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                    }

                    if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                    {
                        chunk.lights.SetLight(index - World.CHUNK_SIZE, SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                        enqueue = true;
                    }

                    if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index - World.CHUNK_SIZE, SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                    }


                    if (enqueue)
                        if (chunk[index - World.CHUNK_SIZE] == 0)
                            lightNodes.Enqueue(new LightNode()
                            {
                                chunk = chunk,
                                index = index - World.CHUNK_SIZE
                            });
                }
                if (((index >> World.LOG_CHUNK_SIZE) & World.CHUNK_MASK) != World.CHUNK_MASK)
                {
                    int light = chunk.lights.GetLightInt(index + World.CHUNK_SIZE);
                    bool enqueue = false;

                    if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index + World.CHUNK_SIZE, SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                    }

                    if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                    {
                        chunk.lights.SetLight(index + World.CHUNK_SIZE, SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                        enqueue = true;
                    }

                    if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index + World.CHUNK_SIZE, SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                    }


                    if (enqueue)
                        if (chunk[index + World.CHUNK_SIZE] == 0)
                            lightNodes.Enqueue(new LightNode()
                            {
                                chunk = chunk,
                                index = index + World.CHUNK_SIZE
                            });
                }

                if (((index >> (World.LOG_CHUNK_SIZE * 2)) & World.CHUNK_MASK) != 0)
                {
                    int light = chunk.lights.GetLightInt(index - World.CHUNK_SIZE_SQUARE);
                    bool enqueue = false;

                    if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index - World.CHUNK_SIZE_SQUARE, SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                    }

                    if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                    {
                        chunk.lights.SetLight(index - World.CHUNK_SIZE_SQUARE, SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                        enqueue = true;
                    }

                    if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index - World.CHUNK_SIZE_SQUARE, SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                    }


                    if (enqueue)
                        if (chunk[index - World.CHUNK_SIZE_SQUARE] == 0)
                            lightNodes.Enqueue(new LightNode()
                            {
                                chunk = chunk,
                                index = index - World.CHUNK_SIZE_SQUARE
                            });
                }
                if (((index >> (World.LOG_CHUNK_SIZE * 2)) & World.CHUNK_MASK) != World.CHUNK_MASK)
                {
                    int light = chunk.lights.GetLightInt(index + World.CHUNK_SIZE_SQUARE);
                    bool enqueue = false;

                    if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index + World.CHUNK_SIZE_SQUARE, SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                    }

                    if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                    {
                        chunk.lights.SetLight(index + World.CHUNK_SIZE_SQUARE, SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                        enqueue = true;
                    }

                    if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                    {
                        enqueue = true;
                        chunk.lights.SetLight(index + World.CHUNK_SIZE_SQUARE, SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                    }


                    if (enqueue)
                        if (chunk[index + World.CHUNK_SIZE_SQUARE] == 0)
                            lightNodes.Enqueue(new LightNode()
                            {
                                chunk = chunk,
                                index = index + World.CHUNK_SIZE_SQUARE
                            });
                }
            }
        }

        private static uint GetRedLight(int light)
            => (uint)light & 0xFu;
        private static uint SetRedLight(ref int light, uint value)
            => (uint)(light = (int)((uint)light | (value)));

        private static uint GetGreenLight(int light)
            => ((uint)light >> 4) & 0xFu;
        private static uint SetGreenLight(ref int light, uint value)
            => (uint)(light =(int)((uint)light | (value << 4)));

        private static uint GetBlueLight(int light)
            => ((uint)light >> 8) & 0xFu;
        private static uint SetBlueLight(ref int light, uint value)
             => (uint)(light = (int)((uint)light | (value << 8)));
    }
}
