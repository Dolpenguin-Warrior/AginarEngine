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
    using System.Globalization;
    using System.Net;

    public class ChunkMeshGenerator
    {
            private static readonly uint3[] Face_Data = new uint3[]
            {
            // North
                    new uint3(0, 0, 1),
                    new uint3(0, 1, 1),
                    new uint3(0, 0, 1),
                    new uint3(0, 0, 0),
            // South
                    new uint3(1, 1, 0),
                    new uint3(1, 1, 1),
                    new uint3(1, 0, 1),
                    new uint3(1, 0, 0),

            // East
                    new uint3(1, 0, 0),
                    new uint3(1, 1, 0),
                    new uint3(1, 1, 1),
                    new uint3(1, 0, 1),

            // West
                    new uint3(0, 0, 1),
                    new uint3(0, 1, 1),
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


        public static ArrayPool<Vertex> vertexPool = new ArrayPool<Vertex>(4, World.CHUNK_SIZE_CUBED * 6 * 4);
        public static ArrayPool<uint> indexPool = new ArrayPool<uint>(4, World.CHUNK_SIZE_CUBED * 6 * 6);

        private static List<Chunk> _chunksToGenerate = new List<Chunk>(4);
        private static List<Mesh> _meshesToGenerate = new List<Mesh>(4);

        public static void Update()
        {
            for (int i = 0; i < _chunksToGenerate.Count && i < _meshesToGenerate.Count && i < 4; i++)
            {
                GenerateMesh(_meshesToGenerate[0], _chunksToGenerate[0]);
                _meshesToGenerate.RemoveAt(0);
                _chunksToGenerate.RemoveAt(0);
            }
        }

        public static bool GenerateMesh(Mesh mesh, Chunk chunk)
        {
            if (vertexPool.ArrayAvailble && indexPool.ArrayAvailble)
            {
                
                // Generate the mesh
                int vertexToken = 0;
                int indexToken = 0;

                Vertex[] vertices = vertexPool.GetArray(ref vertexToken);
                uint[] indices = indexPool.GetArray(ref indexToken);

                BlockStorage blocks = chunk.GetBlocks();
                int vertexCount = 0;
                int indexCount = 0;

#if _DEBUG_CHUNKS
                Stopwatch generationTimer = Stopwatch.StartNew();
#endif // _DEBUG_CHUNKS
                int x, y, z = 0;
                int current = 0;
                for (int i = 0; i < World.CHUNK_SIZE_CUBED; i++)
                {
                    current = blocks.GetBlock(i);
                    
                    if (current != 0)
                    {
                        x = (i & World.CHUNK_MASK);
                        y = (i >> World.LOG_CHUNK_SIZE) & World.CHUNK_MASK;
                        z = (i >> (World.LOG_CHUNK_SIZE * 2)) & World.CHUNK_MASK;

                    North:
                        if ((x == 32 || blocks.GetBlock(i + 1) == 0) == false) goto South;

                        //vertices[vertexCount + 0] = new Vertex(Face_Data[0].x + x, Face_Data[0].y + y, Face_Data[0].z + z, 0, 0); // BR
                        //vertices[vertexCount + 1] = new Vertex(Face_Data[1].x + x, Face_Data[1].y + y, Face_Data[1].z + z, 1, 0); // TR
                        //vertices[vertexCount + 2] = new Vertex(Face_Data[2].x + x, Face_Data[2].y + y, Face_Data[2].z + z, 1, 1); // TL
                        //vertices[vertexCount + 3] = new Vertex(Face_Data[3].x + x, Face_Data[3].y + y, Face_Data[3].z + z, 0, 1); // BL

                        //indices[indexCount++] = (uint)vertexCount + 0;
                        //indices[indexCount++] = (uint)vertexCount + 2;
                        //indices[indexCount++] = (uint)vertexCount + 3;

                        //indices[indexCount++] = (uint)vertexCount + 0;
                        //indices[indexCount++] = (uint)vertexCount + 1;
                        //indices[indexCount++] = (uint)vertexCount + 2;
                        //vertexCount += 4;

                    South:
                        if ((x == 0 || blocks.GetBlock(i - 1) == 0) == false) goto East;
                        vertices[vertexCount + 0] = new Vertex(Face_Data[4].x + x, Face_Data[4].y + y, Face_Data[4].z + z, 0, 0);
                        vertices[vertexCount + 1] = new Vertex(Face_Data[5].x + x, Face_Data[5].y + y, Face_Data[5].z + z, 1, 0);
                        vertices[vertexCount + 2] = new Vertex(Face_Data[6].x + x, Face_Data[6].y + y, Face_Data[6].z + z, 1, 1);
                        vertices[vertexCount + 3] = new Vertex(Face_Data[7].x + x, Face_Data[7].y + y, Face_Data[7].z + z, 0, 1);

                        indices[indexCount++] = (uint)vertexCount + 3;
                        indices[indexCount++] = (uint)vertexCount + 2;
                        indices[indexCount++] = (uint)vertexCount + 0;

                        indices[indexCount++] = (uint)vertexCount + 2;
                        indices[indexCount++] = (uint)vertexCount + 1;
                        indices[indexCount++] = (uint)vertexCount + 0;
                        vertexCount += 4;
                    East:
                        if ((i / World.CHUNK_SIZE_SQUARE == 0 || blocks.GetBlock(i - World.CHUNK_SIZE_SQUARE) == 0) == false) goto West;

                    West:
                        if ((i / World.CHUNK_SIZE_SQUARE == 0 || blocks.GetBlock(i - World.CHUNK_SIZE_SQUARE) == 0) == false) goto Up;

                    Up:
                        if ((i / World.CHUNK_SIZE_SQUARE == 0 || blocks.GetBlock(i - World.CHUNK_SIZE_SQUARE) == 0) == false) goto Down;

                    Down:
                        if ((i / World.CHUNK_SIZE_SQUARE == 0 || blocks.GetBlock(i - World.CHUNK_SIZE_SQUARE) == 0) == false) goto End;

                        End:
                        vertexCount = vertexCount;

                        
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