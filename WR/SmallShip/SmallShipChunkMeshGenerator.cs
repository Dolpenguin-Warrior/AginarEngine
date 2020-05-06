#if DEBUG
#define _DEBUG_CHUNKS
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Aginar.Core;
using Aginar.Core.Generic;
using Aginar.VoxelEngine;
using Common.VoxelEngine.Spaceship.SmallShip;

namespace Aginar.SmallShip
{
    public class SmallShipChunkMeshGenerator
    {
        private readonly static uint3[] Face_Data = new uint3[]
        {
            new uint3(1, 0, 1),
            new uint3(1, 1, 1),
            new uint3(1, 1, 0),
            new uint3(1, 0, 0),

            new uint3(1, 0, 1),
            new uint3(1, 1, 1),
            new uint3(0, 1, 1),
            new uint3(0, 0, 1),

            new uint3(0, 1, 0),
            new uint3(0, 1, 1),
            new uint3(1, 1, 1),
            new uint3(1, 1, 0),
        };

        public static void GenerateMesh(Common.VoxelEngine.Spaceship.SmallShip.Chunk chunk, ChunkMesh chunkMesh)
        {
            ChunkVertex[] vertices = new ChunkVertex[SmallShipComponent.CHUNK_SIZE_CUBE * 6 * 4];
            uint[] indices = new uint[SmallShipComponent.CHUNK_SIZE_CUBE * 6 * 6];

            uint vertexCount = 0;
            int indexCount = 0;

#if _DEBUG_CHUNKS
            Stopwatch generationTimer = Stopwatch.StartNew();
#endif // _DEBUG_CHUNKS

            int shift = 0;
            int i = 0;
            uint[] pos = new uint[3];
            int index = 0;
            int A, B;
            for (pos[2] = 0; pos[2] < SmallShipComponent.CHUNK_SIZE; pos[2]++)
            {
                for (pos[1] = 0; pos[1] < SmallShipComponent.CHUNK_SIZE; pos[1]++)
                {
                    for (pos[0] = 0; pos[0] < SmallShipComponent.CHUNK_SIZE; pos[0]++)
                    {
                        shift = 0;
                        A = chunk.blocks[index];
                        for (i = 0; i < 3; i++)
                        {
                            if (pos[i] == SmallShipComponent.CHUNK_SIZE_MASK)
                                B = 0;
                            else
                                B = chunk.blocks[index + (1 << shift)];

                            if (A > 0 == B > 0) continue;
                            // There is a face, render it
                            if (B == 0)
                            {
                                // Render this blocks face

                                
                            }
                            else
                            {
                                // Render the adjacent block's face

                                
                            }

                            shift += SmallShipComponent.LOG_CHUNK_SIZE;
                        }
                        index++;
                    }
                }
            }

#if _DEBUG_CHUNKS
                generationTimer.Stop();
                Debug.Log($"ShipChunk Meshing Speed: {generationTimer.Elapsed.TotalMilliseconds}");
#endif // _DEBUG_CHUNKS

            chunkMesh.SetData(vertices, indices);
        }
    }
}
