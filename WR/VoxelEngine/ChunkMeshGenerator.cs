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
        private static readonly uint3[] Face_Data = new uint3[]
        {
        // North
                new uint3(1, 0, 1),
                new uint3(1, 1, 1),
                new uint3(1, 1, 0),
                new uint3(1, 0, 0),
        // South
                new uint3(0, 0, 1),
                new uint3(0, 1, 1),
                new uint3(0, 1, 0),
                new uint3(0, 0, 0),

        // East
                new uint3(1, 0, 1),
                new uint3(1, 1, 1),
                new uint3(0, 1, 1),
                new uint3(0, 0, 1),

        // West
                new uint3(1, 0, 0),
                new uint3(1, 1, 0),
                new uint3(0, 1, 0),
                new uint3(0, 0, 0),

        // Top
                new uint3(0, 1, 0),
                new uint3(0, 1, 1),
                new uint3(1, 1, 1),
                new uint3(1, 1, 0),

        // Bottom
                new uint3(1, 0, 0),
                new uint3(1, 0, 1),
                new uint3(0, 0, 1),
                new uint3(0, 0, 0)
        };

        public static ArrayPool<ChunkVertex> vertexPool = new ArrayPool<ChunkVertex>(4, World.CHUNK_SIZE_CUBED * 6 * 4);
        public static ArrayPool<uint> indexPool = new ArrayPool<uint>(4, World.CHUNK_SIZE_CUBED * 6 * 6);

        private static List<Chunk> _chunksToGenerate = new List<Chunk>(4);
        private static List<ChunkMesh> _meshesToGenerate = new List<ChunkMesh>(4);

        public static void Update()
        {
            for (int i = 0; i < _chunksToGenerate.Count && i < _meshesToGenerate.Count && i < 4; i++)
            {
                GenerateMesh(_meshesToGenerate[0], _chunksToGenerate[0]);
                _meshesToGenerate.RemoveAt(0);
                _chunksToGenerate.RemoveAt(0);
            }
        }

        public static bool GenerateMesh(ChunkMesh mesh, Chunk chunk)
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
                uint x, y, z = 0;
                int current = 0;
                for (int i = 0; i < World.CHUNK_SIZE_CUBED; i++)
                {
                    current = blocks.GetBlock(i);
                    
                    if (current != 0)
                    {
                        x = (uint)(i & World.CHUNK_MASK);
                        y = (uint)(i >> World.LOG_CHUNK_SIZE) & World.CHUNK_MASK;
                        z = (uint)(i >> (World.LOG_CHUNK_SIZE * 2)) & World.CHUNK_MASK;

                        Vector3i lights;
                        //
                        if (x == World.CHUNK_MASK || blocks.GetBlock(i + 1) == 0)
                        {
                            if (x == World.CHUNK_MASK)
                                lights = new Vector3i();
                            else
                                lights = chunk.lights.GetLight(i + 1);
                            // BlockType (16 bits), Normal (3 bits), Rotation (4 bits), UV ()
                            vertices[vertexCount + 0].BlockType = (uint)current | (0u << 16) | (0u << 19) | (0u << 23);
                            vertices[vertexCount + 0].BlockData =
                                ((Face_Data[0].x + x) | ((Face_Data[0].y + y) << 6) | ((Face_Data[0].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            

                            vertices[vertexCount + 1].BlockType = (uint)current | (0u << 16) | (0u << 19) | (3u << 23);
                            vertices[vertexCount + 1].BlockData = 
                                ((Face_Data[1].x + x) | ((Face_Data[1].y + y) << 6) | ((Face_Data[1].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 2].BlockType = (uint)current | (0u << 16) | (0u << 19) | (2u << 23);
                            vertices[vertexCount + 2].BlockData = 
                                ((Face_Data[2].x + x) | ((Face_Data[2].y + y) << 6) | ((Face_Data[2].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 3].BlockType = (uint)current | (0u << 16) | (0u << 19) | (1u << 23);
                            vertices[vertexCount + 3].BlockData = 
                                ((Face_Data[3].x + x) | ((Face_Data[3].y + y) << 6) | ((Face_Data[3].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            indices[indexCount++] = (uint)vertexCount + 3;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            indices[indexCount++] = (uint)vertexCount + 0;

                            indices[indexCount++] = (uint)vertexCount + 2;
                            indices[indexCount++] = (uint)vertexCount + 1;
                            indices[indexCount++] = (uint)vertexCount + 0;
                            vertexCount += 4;
                        } // North

                        if (x == 0 || blocks.GetBlock(i - 1) == 0)
                        {
                            if (x == 0)
                                lights = new Vector3i();
                            else
                                lights = chunk.lights.GetLight(i - 1);
                            vertices[vertexCount + 0].BlockType = (uint)current | (1u << 16) | (0u << 19) | (1u << 23);
                            vertices[vertexCount + 0].BlockData =
                                ((Face_Data[4].x + x) | ((Face_Data[4].y + y) << 6) | ((Face_Data[4].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            vertices[vertexCount + 1].BlockType = (uint)current | (1u << 16) | (0u << 19) | (2u << 23);
                            vertices[vertexCount + 1].BlockData =
                                ((Face_Data[5].x + x) | ((Face_Data[5].y + y) << 6) | ((Face_Data[5].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 2].BlockType = (uint)current | (1u << 16) | (0u << 19) | (3u << 23);
                            vertices[vertexCount + 2].BlockData =
                                ((Face_Data[6].x + x) | ((Face_Data[6].y + y) << 6) | ((Face_Data[6].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 3].BlockType = (uint)current | (1u << 16) | (0u << 19) | (0u << 23);
                            vertices[vertexCount + 3].BlockData =
                                ((Face_Data[7].x + x) | ((Face_Data[7].y + y) << 6) | ((Face_Data[7].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            indices[indexCount++] = (uint)vertexCount + 0;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            indices[indexCount++] = (uint)vertexCount + 3;

                            indices[indexCount++] = (uint)vertexCount + 0;
                            indices[indexCount++] = (uint)vertexCount + 1;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            vertexCount += 4;
                        } // South

                        if (z == World.CHUNK_MASK || blocks.GetBlock(i + World.CHUNK_SIZE_SQUARE) == 0)
                        {
                            if (z == World.CHUNK_MASK)
                                lights = new Vector3i();
                            else
                                lights = chunk.lights.GetLight(i + World.CHUNK_SIZE_SQUARE);

                            vertices[vertexCount + 0].BlockType = (uint)current | (2u << 16) | (0u << 19) | (1u << 23);
                            vertices[vertexCount + 0].BlockData =
                                ((Face_Data[8].x + x) | ((Face_Data[8].y + y) << 6) | ((Face_Data[8].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            vertices[vertexCount + 1].BlockType = (uint)current | (2u << 16) | (0u << 19) | (2u << 23);
                            vertices[vertexCount + 1].BlockData =
                                ((Face_Data[9].x + x) | ((Face_Data[9].y + y) << 6) | ((Face_Data[9].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 2].BlockType = (uint)current | (2u << 16) | (0u << 19) | (3u << 23);
                            vertices[vertexCount + 2].BlockData =
                                ((Face_Data[10].x + x) | ((Face_Data[10].y + y) << 6) | ((Face_Data[10].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 3].BlockType = (uint)current | (2u << 16) | (0u << 19) | (0u << 23);
                            vertices[vertexCount + 3].BlockData =
                                ((Face_Data[11].x + x) | ((Face_Data[11].y + y) << 6) | ((Face_Data[11].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );



                            indices[indexCount++] = (uint)vertexCount + 0;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            indices[indexCount++] = (uint)vertexCount + 3;

                            indices[indexCount++] = (uint)vertexCount + 0;
                            indices[indexCount++] = (uint)vertexCount + 1;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            vertexCount += 4;
                        } // East

                        if (z == 0 || blocks.GetBlock(i - World.CHUNK_SIZE_SQUARE) == 0) // West
                        {
                            if (z == 0)
                                lights = new Vector3i();
                            else
                                lights = chunk.lights.GetLight(i - World.CHUNK_SIZE_SQUARE);

                            vertices[vertexCount + 0].BlockType = (uint)current | (3u << 16) | (0u << 19) | (0u << 23);
                            vertices[vertexCount + 0].BlockData =
                                ((Face_Data[12].x + x) | ((Face_Data[12].y + y) << 6) | ((Face_Data[12].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            vertices[vertexCount + 1].BlockType = (uint)current | (3u << 16) | (0u << 19) | (3u << 23);
                            vertices[vertexCount + 1].BlockData =
                                ((Face_Data[13].x + x) | ((Face_Data[13].y + y) << 6) | ((Face_Data[13].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 2].BlockType = (uint)current | (3u << 16) | (0u << 19) | (2u << 23);
                            vertices[vertexCount + 2].BlockData =
                                ((Face_Data[14].x + x) | ((Face_Data[14].y + y) << 6) | ((Face_Data[14].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 3].BlockType = (uint)current | (3u << 16) | (0u << 19) | (1u << 23);
                            vertices[vertexCount + 3].BlockData =
                                ((Face_Data[15].x + x) | ((Face_Data[15].y + y) << 6) | ((Face_Data[15].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            indices[indexCount++] = (uint)vertexCount + 3;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            indices[indexCount++] = (uint)vertexCount + 0;

                            indices[indexCount++] = (uint)vertexCount + 2;
                            indices[indexCount++] = (uint)vertexCount + 1;
                            indices[indexCount++] = (uint)vertexCount + 0;
                            vertexCount += 4;
                        }

                        if (y == World.CHUNK_MASK || blocks.GetBlock(i + World.CHUNK_SIZE) == 0)
                        {
                            if (y == World.CHUNK_MASK)
                                lights = new Vector3i();
                            else
                                lights = chunk.lights.GetLight(i + World.CHUNK_SIZE);
                            vertices[vertexCount + 0].BlockType = (uint)current | (4u << 16) | (0u << 19) | (0u << 23);
                            vertices[vertexCount + 0].BlockData =
                                ((Face_Data[16].x + x) | ((Face_Data[16].y + y) << 6) | ((Face_Data[16].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            vertices[vertexCount + 1].BlockType = (uint)current | (4u << 16) | (0u << 19) | (1u << 23);
                            vertices[vertexCount + 1].BlockData =
                                ((Face_Data[17].x + x) | ((Face_Data[17].y + y) << 6) | ((Face_Data[17].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 2].BlockType = (uint)current | (4u << 16) | (0u << 19) | (2u << 23);
                            vertices[vertexCount + 2].BlockData =
                                ((Face_Data[18].x + x) | ((Face_Data[18].y + y) << 6) | ((Face_Data[18].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 3].BlockType = (uint)current | (4u << 16) | (0u << 19) | (3u << 23);
                            vertices[vertexCount + 3].BlockData =
                                ((Face_Data[19].x + x) | ((Face_Data[19].y + y) << 6) | ((Face_Data[19].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            indices[indexCount++] = (uint)vertexCount + 0;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            indices[indexCount++] = (uint)vertexCount + 3;

                            indices[indexCount++] = (uint)vertexCount + 0;
                            indices[indexCount++] = (uint)vertexCount + 1;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            vertexCount += 4;
                        } // Up

                        if (y == 0 || blocks.GetBlock(i - World.CHUNK_SIZE) == 0)
                        {
                            if (y == 0)
                                lights = new Vector3i();
                            else
                                lights = chunk.lights.GetLight(i - World.CHUNK_SIZE);

                            vertices[vertexCount + 0].BlockType = (uint)current | (5u << 16) | (0u << 19) | (0u << 23);
                            vertices[vertexCount + 0].BlockData =
                                ((Face_Data[20].x + x) | ((Face_Data[20].y + y) << 6) | ((Face_Data[20].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            vertices[vertexCount + 1].BlockType = (uint)current | (5u << 16) | (0u << 19) | (1u << 23);
                            vertices[vertexCount + 1].BlockData =
                                ((Face_Data[21].x + x) | ((Face_Data[21].y + y) << 6) | ((Face_Data[21].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 2].BlockType = (uint)current | (5u << 16) | (0u << 19) | (2u << 23);
                            vertices[vertexCount + 2].BlockData =
                                ((Face_Data[22].x + x) | ((Face_Data[22].y + y) << 6) | ((Face_Data[22].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );
                            vertices[vertexCount + 3].BlockType = (uint)current | (5u << 16) | (0u << 19) | (3u << 23);
                            vertices[vertexCount + 3].BlockData =
                                ((Face_Data[23].x + x) | ((Face_Data[23].y + y) << 6) | ((Face_Data[23].z + z) << 12) // Position
                                | ((uint)lights.X << 18) | ((uint)lights.Y << 22) | ((uint)lights.Z << 26)
                                );

                            indices[indexCount++] = (uint)vertexCount + 0;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            indices[indexCount++] = (uint)vertexCount + 3;

                            indices[indexCount++] = (uint)vertexCount + 0;
                            indices[indexCount++] = (uint)vertexCount + 1;
                            indices[indexCount++] = (uint)vertexCount + 2;
                            vertexCount += 4;
                        } // Down

                    }
                }

#if _DEBUG_CHUNKS
                generationTimer.Stop();
                Debug.Log(generationTimer.Elapsed.TotalMilliseconds.ToString());
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