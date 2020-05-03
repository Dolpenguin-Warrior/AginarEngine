#define _LIGHT_DEBUG

using OpenToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Aginar.VoxelEngine.Lighting
{
    public static class LightStorage
    {
        private static Queue<LightNode> lightNodes = new Queue<LightNode>();
        private static Queue<LightRemovalNode> lightRemovalNodes = new Queue<LightRemovalNode>();

        public static void PropogateLight(int localIndex, Chunk blockChunk)
        {

#if _LIGHT_DEBUG
            Stopwatch timer = Stopwatch.StartNew();
#endif
            lightNodes.Enqueue(new LightNode() {
                chunk = blockChunk,
                index = localIndex
                });

            while (lightNodes.Count > 0)
            {
                LightNode node = lightNodes.Dequeue();

                int index = node.index;
                Chunk chunk = node.chunk;

                uint lightLevel = chunk.lights.GetLightUInt(index);

                if (lightLevel > 0)
                {
                    int shift = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (((index >> shift) & World.CHUNK_MASK) != 0)
                        {
                            if (chunk[index - (1 << shift)] == 0)
                            {
                                uint light = chunk.lights.GetLightUInt(index - (1 << shift));
                                bool enqueue = false;

                                if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                                {
                                    enqueue = true;
                                    chunk.lights.SetLight(index - (1 << shift), SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                                }

                                if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                                {
                                    chunk.lights.SetLight(index - (1 << shift), SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                                    enqueue = true;
                                }

                                if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                                {
                                    enqueue = true;
                                    chunk.lights.SetLight(index - (1 << shift), SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                                }


                                if (enqueue)
                                    lightNodes.Enqueue(new LightNode()
                                    {
                                        chunk = chunk,
                                        index = index - (1 << shift)
                                    });
                            }
                        }
                        if (((index >> shift) & World.CHUNK_MASK) != World.CHUNK_MASK)
                        {
                            if (chunk[index + (1 << shift)] == 0)
                            {

                                uint light = chunk.lights.GetLightUInt(index + (1 << shift));
                                bool enqueue = false;

                                if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                                {
                                    enqueue = true;
                                    chunk.lights.SetLight(index + (1 << shift), SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                                }

                                if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                                {
                                    chunk.lights.SetLight(index + (1 << shift), SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                                    enqueue = true;
                                }

                                if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                                {
                                    enqueue = true;
                                    chunk.lights.SetLight(index + (1 << shift), SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                                }


                                if (enqueue)
                                    lightNodes.Enqueue(new LightNode()
                                    {
                                        chunk = chunk,
                                        index = index + (1 << shift)
                                    });
                            }
                        }
                        shift += World.LOG_CHUNK_SIZE;
                    }


                }
            }

#if _LIGHT_DEBUG
            timer.Stop();
            Debug.Log($"Lighting Speed: {timer.Elapsed.TotalMilliseconds}");
#endif
        }

        public static void DestroyLight(int localIndex, Chunk blockChunk)
        {
            
            lightRemovalNodes.Enqueue(new LightRemovalNode()
            {
                chunk = blockChunk,
                index = localIndex,
                value = 0
            });
            blockChunk.lights.SetLight(localIndex, 0);

            while (lightRemovalNodes.Count > 0)
            {
                LightRemovalNode node = lightRemovalNodes.Dequeue();

                int index = node.index;
                Chunk chunk = node.chunk;

                uint lightLevel = node.value;

                if (lightLevel > 0)
                {
                    int shift = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (((index >> shift) & World.CHUNK_MASK) != 0)
                        {
                            if (chunk[index - (1 << shift)] == 0)
                            {
                                uint light = chunk.lights.GetLightUInt(index - (1 << shift));
                                bool enqueue = false;
                                bool enqueueRemove = false;

                                if (GetRedLight(light) != 0 && GetRedLight(light) < GetRedLight(lightLevel))
                                {
                                    chunk.lights.SetLight(index - (1 << shift), 0);
                                    enqueueRemove = true;
                                }
                                else if (GetRedLight(light) >= GetRedLight(lightLevel))
                                {
                                    enqueue = true;
                                }

                                if (GetBlueLight(light) != 0 && GetBlueLight(light) < GetBlueLight(lightLevel))
                                {
                                    chunk.lights.SetLight(index - (1 << shift), SetBlueLight(ref light, 0));
                                    enqueueRemove = true;
                                }
                                else if (GetBlueLight(light) >= GetBlueLight(lightLevel))
                                {
                                    enqueue = true;
                                }

                                if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                                {
                                    enqueue = true;
                                }


                                if (enqueue)
                                    lightNodes.Enqueue(new LightNode()
                                    {
                                        chunk = chunk,
                                        index = index - (1 << shift)
                                    });
                            }
                        }
                        if (((index >> shift) & World.CHUNK_MASK) != World.CHUNK_MASK)
                        {
                            if (chunk[index + (1 << shift)] == 0)
                            {

                                uint light = chunk.lights.GetLightUInt(index + (1 << shift));
                                bool enqueue = false;

                                if (GetRedLight(light) + 2u <= GetRedLight(lightLevel))
                                {
                                    enqueue = true;
                                    chunk.lights.SetLight(index + (1 << shift), SetRedLight(ref light, GetRedLight(lightLevel) - 1u));
                                }

                                if (GetBlueLight(light) + 2u <= GetBlueLight(lightLevel))
                                {
                                    chunk.lights.SetLight(index + (1 << shift), SetBlueLight(ref light, GetBlueLight(lightLevel) - 1u));
                                    enqueue = true;
                                }

                                if (GetGreenLight(light) + 2u <= GetGreenLight(lightLevel))
                                {
                                    enqueue = true;
                                    chunk.lights.SetLight(index + (1 << shift), SetGreenLight(ref light, GetGreenLight(lightLevel) - 1u));
                                }


                                if (enqueue)
                                    lightNodes.Enqueue(new LightNode()
                                    {
                                        chunk = chunk,
                                        index = index + (1 << shift)
                                    });
                            }
                        }
                        shift += World.LOG_CHUNK_SIZE;
                    }


                }
            }
        }

        private static uint GetRedLight(uint light)
            => light & 0xFu;
        private static uint SetRedLight(ref uint light, uint value)
        {
            light &= 0xFF0;
            return light |= value & 0xFu;
        }
            

        private static uint GetGreenLight(uint light)
            => (light >> 4) & 0xFu;
        private static uint SetGreenLight(ref uint light, uint value)
        {
            light &= 0xF0Fu;
            return light |= (value << 4) & 0xF0u;
        }
            

        private static uint GetBlueLight(uint light)
            => (light >> 8) & 0xFu;
        private static uint SetBlueLight(ref uint light, uint value)
        {
            light &= 0xFFu;
            return light |= (value << 8) & 0xF00u;
        }
             
    }
}
