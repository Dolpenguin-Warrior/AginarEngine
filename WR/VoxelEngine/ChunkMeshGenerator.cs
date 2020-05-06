//#if DEBUG
#define _DEBUG_CHUNKS

//#endif
using System.Collections.Generic;
using System.Diagnostics;

namespace Aginar.VoxelEngine
{
    using ChunkData;
    using Core;
    using Core.Generic;
    using OpenToolkit.Mathematics;
    using System.Globalization;
    using System.Net;

    

    public class ChunkMeshGenerator
    {
        
        private static uint GetFaceData(uint x, uint y, uint z) => x | y << 6 | z << 12;

        private static readonly uint[] Block_Pos_Shifts = new uint[]
        {
            GetFaceData(1, 0, 0),
            GetFaceData(0, 1, 0),
            GetFaceData(0, 0, 1),
        };

        private static readonly uint[] Face_Data = new uint[]
        {
        // North
                GetFaceData(1, 0, 1),
                GetFaceData(1, 1, 1),
                GetFaceData(1, 1, 0),
                GetFaceData(1, 0, 0),

        // Top
                GetFaceData(1, 1, 0),
                GetFaceData(1, 1, 1),
                GetFaceData(0, 1, 1),
                GetFaceData(0, 1, 0),

        // East
                GetFaceData(0, 0, 1),
                GetFaceData(0, 1, 1),
                GetFaceData(1, 1, 1),
                GetFaceData(1, 0, 1),





        // South
                GetFaceData(0, 0, 0),
                GetFaceData(0, 1, 0),
                GetFaceData(0, 1, 1),
                GetFaceData(0, 0, 1),

        // Bottom
                GetFaceData(0, 0, 0),
                GetFaceData(0, 0, 1),
                GetFaceData(1, 0, 1),
                GetFaceData(1, 0, 0),

        // West
                GetFaceData(1, 0, 0),
                GetFaceData(1, 1, 0),
                GetFaceData(0, 1, 0),
                GetFaceData(0, 0, 0),


        };

        public static ArrayPool<ChunkVertex> vertexPool = new ArrayPool<ChunkVertex>(4, World.CHUNK_SIZE_CUBED * 6 * 4);
        public static ArrayPool<uint> indexPool = new ArrayPool<uint>(4, World.CHUNK_SIZE_CUBED * 6 * 6);

        private static List<Chunk> _chunksToGenerate = new List<Chunk>(4);
        private static List<ChunkMesh> _meshesToGenerate = new List<ChunkMesh>(4);

        public static void Update()
        {
            for (int i = 0; i < _chunksToGenerate.Count && i < _meshesToGenerate.Count && i < 4; i++)
            {
                double timer = 0;
                GenerateMesh(_meshesToGenerate[0], _chunksToGenerate[0], ref timer);
                _meshesToGenerate.RemoveAt(0);
                _chunksToGenerate.RemoveAt(0);
            }
        }

        public static bool GenerateMesh(ChunkMesh mesh, Chunk chunk, ref double timer)
        {
            if (vertexPool.ArrayAvailble && indexPool.ArrayAvailble)
            {
                
                // Generate the mesh
                int vertexToken = 0;
                int indexToken = 0;

                ChunkVertex[] vertices = vertexPool.GetArray(ref vertexToken);
                uint[] indices = indexPool.GetArray(ref indexToken);

                BlockStorage blocks = chunk.GetBlocks();
                int vertexCount = 0;
                int indexCount = 0;

#if _DEBUG_CHUNKS
                Stopwatch generationTimer = Stopwatch.StartNew();
#endif // _DEBUG_CHUNKS

                int index = 0;
                uint posIndex = 0;
                uint i;
                int shift; 
                uint A, B;
                uint lights;
                uint[] pos = new uint[3];

                for (pos[2] = 0; pos[2] < World.CHUNK_SIZE; pos[2]++)
                {
                    for (pos[1] = 0; pos[1] < World.CHUNK_SIZE; pos[1]++)
                    {
                        for (pos[0] = 0; pos[0] < World.CHUNK_SIZE; pos[0]++)
                        {
                            shift = 0;
                            A = (uint)blocks.GetBlock(index);

                            for (i = 0; i < 3; i++)
                            {
                                if (pos[i] == World.CHUNK_MASK)
                                    B = 0;
                                else
                                    B = (uint)blocks.GetBlock(index + (1 << shift));

                                shift += World.LOG_CHUNK_SIZE;
                                if (A > 0 == B > 0) continue;

                                posIndex = pos[0] | pos[1] << 6 | pos[2] << 12;

                                if (B == 0)
                                {
                                    if (pos[i] == World.CHUNK_MASK)
                                        lights = 0;
                                    else
                                        lights = chunk.lights.GetLightUInt(index);

                                    //Current face
                                    vertices[vertexCount + 0].BlockType = A | (i << 16) | (0u << 23);
                                    vertices[vertexCount + 0].BlockData =
                                        (posIndex + Face_Data[(i * 4) + 0] // Position
                                        | lights << 18
                                        );


                                    vertices[vertexCount + 1].BlockType = A | (i << 16) | (3u << 23);
                                    vertices[vertexCount + 1].BlockData =
                                        (posIndex + Face_Data[(i * 4) + 1] // Position
                                        | lights << 18
                                        );
                                    vertices[vertexCount + 2].BlockType = A | (i << 16) | (2u << 23);
                                    vertices[vertexCount + 2].BlockData =
                                        (posIndex + Face_Data[(i * 4) + 2] // Position
                                        | lights << 18
                                        );
                                    vertices[vertexCount + 3].BlockType = A | (i << 16) | (1u << 23);
                                    vertices[vertexCount + 3].BlockData =
                                        (posIndex + Face_Data[(i * 4) + 3] // Position
                                        | lights << 18
                                        );

                                    indices[indexCount++] = (uint)vertexCount + 3;
                                    indices[indexCount++] = (uint)vertexCount + 2;
                                    indices[indexCount++] = (uint)vertexCount + 0;

                                    indices[indexCount++] = (uint)vertexCount + 2;
                                    indices[indexCount++] = (uint)vertexCount + 1;
                                    indices[indexCount++] = (uint)vertexCount + 0;
                                    vertexCount += 4;
                                }
                                else
                                {
                                    //OtherFace
                                    lights = chunk.lights.GetLightUInt(index);
                                    //Current face
                                    vertices[vertexCount + 0].BlockType = A | (i << 16) | (0u << 23);
                                    vertices[vertexCount + 0].BlockData =
                                        (posIndex + Face_Data[(i * 4) + 0 + 12] + Block_Pos_Shifts[i] // Position
                                        | lights << 18
                                        );


                                    vertices[vertexCount + 1].BlockType = A | (i << 16) | (3u << 23);
                                    vertices[vertexCount + 1].BlockData =
                                        (posIndex + Face_Data[(i * 4) + 1 + 12] + Block_Pos_Shifts[i] // Position
                                        | lights << 18
                                        );
                                    vertices[vertexCount + 2].BlockType = A | (i << 16) | (2u << 23);
                                    vertices[vertexCount + 2].BlockData =
                                        (posIndex + Face_Data[(i * 4) + 2 + 12] + Block_Pos_Shifts[i] // Position
                                        | lights << 18
                                        );
                                    vertices[vertexCount + 3].BlockType = A | (i << 16) | (1u << 23);
                                    vertices[vertexCount + 3].BlockData =
                                        (posIndex + Face_Data[(i * 4) + 3 + 12] + Block_Pos_Shifts[i] // Position
                                        | lights << 18
                                        );

                                    indices[indexCount++] = (uint)vertexCount + 3;
                                    indices[indexCount++] = (uint)vertexCount + 2;
                                    indices[indexCount++] = (uint)vertexCount + 0;

                                    indices[indexCount++] = (uint)vertexCount + 2;
                                    indices[indexCount++] = (uint)vertexCount + 1;
                                    indices[indexCount++] = (uint)vertexCount + 0;
                                    vertexCount += 4;
                                }
                            }

                            index++;
                        }
                    }
                }

#if _DEBUG_CHUNKS
                generationTimer.Stop();
                timer = generationTimer.ElapsedTicks / 10000.0;
#endif // _DEBUG_CHUNKS

                mesh.SetData(vertices, indices);
                vertexPool.ReturnArray(vertexToken);
                indexPool.ReturnArray(indexToken);

                return true;

            }
            // Wait for free cycles
            _chunksToGenerate.Add(chunk);
            _meshesToGenerate.Add(mesh);
            return false;
        }
    }
}