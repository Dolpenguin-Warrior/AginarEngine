using System.Collections.Generic;

namespace Aginar.VoxelEngine
{
    using Core;
    using Core.Generic;

    public class ChunkMeshGenerator
    {
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

                int[] blocks = chunk.GetBlocks();
                int vertexCount = 0;
                int indexCount = 0;
                for (int localZ = 0; localZ < World.CHUNK_SIZE; localZ++)
                {
                    for (int localY = 0; localY < World.CHUNK_SIZE; localY++)
                    {
                        for (int localX = 0; localX < World.CHUNK_SIZE; localX++)
                        {
                            int index = World.Vector3IntToIndex(localX, localY, localZ);
                            if (blocks[index] == 0)
                            { // check to place face for others
                                // Check foward (x +)
                                if (localX + 1 < World.CHUNK_SIZE)
                                {
                                    if (blocks[index + 1] != 0)
                                        AddQuad(Direction.South, vertices, indices, ref indexCount, ref vertexCount, localX + 1, localY, localZ, blocks[index + 1]);
                                }
                                else
                                {
                                    if (chunk.World[localX + 1 + chunk.position.X, localY + chunk.position.Y, localZ + chunk.position.Z] != 0)
                                        AddQuad(Direction.South, vertices, indices, ref indexCount, ref vertexCount, localX + 1, localY, localZ, chunk.World[localX + 1 + chunk.position.X, localY + chunk.position.Y, localZ + chunk.position.Z]);
                                }
                                // Check up (y +)
                                if (localY + 1 < World.CHUNK_SIZE)
                                {
                                    if (blocks[index + World.CHUNK_SIZE] != 0)
                                        AddQuad(Direction.Down, vertices, indices, ref indexCount, ref vertexCount, localX, localY + 1, localZ, blocks[index + World.CHUNK_SIZE]);
                                }
                                else
                                {
                                    if (chunk.World[localX + chunk.position.X, localY + 1 + chunk.position.Y, localZ + chunk.position.Z] != 0)
                                        AddQuad(Direction.Down, vertices, indices, ref indexCount, ref vertexCount, localX, localY + 1, localZ, chunk.World[localX + chunk.position.X, localY + 1 + chunk.position.Y, localZ + chunk.position.Z]);
                                }
                                // Check right (z +)
                                if (localZ + 1 < World.CHUNK_SIZE)
                                {
                                    if (blocks[index + World.CHUNK_SIZE_SQUARE] != 0)
                                        AddQuad(Direction.West, vertices, indices, ref indexCount, ref vertexCount, localX, localY, localZ + 1, blocks[index + World.CHUNK_SIZE_SQUARE]);
                                }
                                else
                                {
                                    if (chunk.World[localX + chunk.position.X, localY + chunk.position.Y, localZ + 1 + chunk.position.Z] != 0)
                                        AddQuad(Direction.West, vertices, indices, ref indexCount, ref vertexCount, localX, localY, localZ + 1, chunk.World[localX + chunk.position.X, localY + chunk.position.Y, localZ + 1 + chunk.position.Z]);
                                }
                            }
                            else
                            { // check to place face for self
                                // Check foward (x +)
                                if (localX + 1 < World.CHUNK_SIZE)
                                {
                                    if (blocks[index + 1] == 0)
                                        AddQuad(Direction.North, vertices, indices, ref indexCount, ref vertexCount, localX, localY, localZ, blocks[index]);
                                }
                                else
                                {
                                    if (chunk.World[localX + 1 + chunk.position.X, localY + chunk.position.Y, localZ + chunk.position.Z] == 0)
                                        AddQuad(Direction.North, vertices, indices, ref indexCount, ref vertexCount, localX, localY, localZ, blocks[index]);
                                }
                                // Check up (y +)
                                if (localY + 1 < World.CHUNK_SIZE)
                                {
                                    if (blocks[index + World.CHUNK_SIZE] == 0)
                                        AddQuad(Direction.Up, vertices, indices, ref indexCount, ref vertexCount, localX, localY, localZ, blocks[index]);
                                }
                                else
                                {
                                    if (chunk.World[localX + chunk.position.X, localY + 1 + chunk.position.Y, localZ + chunk.position.Z] == 0)
                                        AddQuad(Direction.Up, vertices, indices, ref indexCount, ref vertexCount, localX, localY, localZ, blocks[index]);
                                }
                                // Check right (z +)
                                // Check right (z +)
                                if (localZ + 1 < World.CHUNK_SIZE)
                                {
                                    if (blocks[index + World.CHUNK_SIZE_SQUARE] == 0)
                                        AddQuad(Direction.East, vertices, indices, ref indexCount, ref vertexCount, localX, localY, localZ, blocks[index]);
                                }
                                else
                                {
                                    if (chunk.World[localX + chunk.position.X, localY + chunk.position.Y, localZ + 1 + chunk.position.Z] == 0)
                                        AddQuad(Direction.East, vertices, indices, ref indexCount, ref vertexCount, localX, localY, localZ, blocks[index]);
                                }
                            }
                        }
                    }
                }

                mesh.SetData(vertices, indices);
                vertexPool.ReturnArray(vertexToken);
                indexPool.ReturnArray(indexToken);
            }
            // Wait for free cycles
            _chunksToGenerate.Add(chunk);
            _meshesToGenerate.Add(mesh);
            return false;
        }

        private static void AddQuad(Direction direction, Vertex[] verticies, uint[] indicies, ref int indexCount, ref int vertexCount, int _x, int _y, int _z, int blockType)
        {
            switch (direction)
            {
                case Direction.North:
                    verticies[vertexCount + 0] = new Vertex(_x + 1f, _y + 1f, _z,
                        0,
                        (UVValues.BlockUVHeight * (blockType - 1))
                        ); // Top left
                    verticies[vertexCount + 1] = new Vertex(_x + 1f, _y + 1f, _z + 1f,
                        UVValues.BlockUVWidth,
                        ((UVValues.BlockUVHeight) * (blockType - 1))
                        ); // Top right
                    verticies[vertexCount + 2] = new Vertex(_x + 1f, _y, _z,
                        0,
                        (UVValues.BlockUVHeight * (blockType))
                        ); // bottom left
                    verticies[vertexCount + 3] = new Vertex(_x + 1f, _y, _z + 1f,
                        UVValues.BlockUVWidth,
                        (UVValues.BlockUVHeight * (blockType))
                        ); // bottom right
                    break;

                case Direction.South:
                    verticies[vertexCount + 0] = new Vertex(_x, _y + 1f, _z + 1f,
                        UVValues.BlockUVWidth * 1,
                        UVValues.BlockUVHeight * (blockType - 1)
                        ); // Top left
                    verticies[vertexCount + 1] = new Vertex(_x, _y + 1f, _z,
                        UVValues.BlockUVWidth * 2,
                        UVValues.BlockUVHeight * (blockType - 1)
                        ); // Top right
                    verticies[vertexCount + 2] = new Vertex(_x, _y, _z + 1f,
                        UVValues.BlockUVWidth * 1,
                        UVValues.BlockUVHeight * (blockType)); // bottom left
                    verticies[vertexCount + 3] = new Vertex(_x, _y, _z,
                        UVValues.BlockUVWidth * 2,
                        UVValues.BlockUVHeight * (blockType)); // bottom right
                    break;

                case Direction.East:
                    verticies[vertexCount + 0] = new Vertex(_x + 1f, _y + 1f, _z + 1f,
                        UVValues.BlockUVWidth * 2,
                        UVValues.BlockUVHeight * (blockType - 1)
                        ); // Top left
                    verticies[vertexCount + 1] = new Vertex(_x, _y + 1f, _z + 1f,
                        UVValues.BlockUVWidth * 3,
                        UVValues.BlockUVHeight * (blockType - 1)
                        ); // Top right
                    verticies[vertexCount + 2] = new Vertex(_x + 1f, _y, _z + 1f,
                        UVValues.BlockUVWidth * 2,
                        UVValues.BlockUVHeight * blockType
                        ); // bottom left
                    verticies[vertexCount + 3] = new Vertex(_x, _y, _z + 1f,
                        UVValues.BlockUVWidth * 3,
                        UVValues.BlockUVHeight * blockType
                        ); // bottom right
                    break;

                case Direction.West:
                    verticies[vertexCount + 0] = new Vertex(_x, _y + 1f, _z,
                        UVValues.BlockUVWidth * 3,
                        UVValues.BlockUVHeight * (blockType - 1)); // Top left
                    verticies[vertexCount + 1] = new Vertex(_x + 1f, _y + 1f, _z,
                        UVValues.BlockUVWidth * 4,
                        UVValues.BlockUVHeight * (blockType - 1)); // Top right
                    verticies[vertexCount + 2] = new Vertex(_x, _y, _z,
                        UVValues.BlockUVWidth * 3,
                        UVValues.BlockUVHeight * (blockType)); // bottom left
                    verticies[vertexCount + 3] = new Vertex(_x + 1f, _y, _z,
                        UVValues.BlockUVWidth * 4,
                        UVValues.BlockUVHeight * (blockType)); // bottom right
                    break;

                case Direction.Up:
                    verticies[vertexCount + 0] = new Vertex(_x, _y + 1f, _z + 1f,
                        UVValues.BlockUVWidth * 4,
                        UVValues.BlockUVHeight * (blockType - 1)); // Top left
                    verticies[vertexCount + 1] = new Vertex(_x + 1f, _y + 1f, _z + 1f,
                        UVValues.BlockUVWidth * 5,
                        UVValues.BlockUVHeight * (blockType - 1)); // Top right
                    verticies[vertexCount + 2] = new Vertex(_x, _y + 1f, _z,
                        UVValues.BlockUVWidth * 4,
                        UVValues.BlockUVHeight * blockType); // bottom left
                    verticies[vertexCount + 3] = new Vertex(_x + 1f, _y + 1f, _z,
                        UVValues.BlockUVWidth * 5,
                        UVValues.BlockUVHeight * blockType); // bottom right
                    break;

                case Direction.Down:
                    verticies[vertexCount + 0] = new Vertex(_x + 1f, _y, _z + 1f,
                        UVValues.BlockUVWidth * 5,
                        UVValues.BlockUVHeight * (blockType - 1)); // Top left
                    verticies[vertexCount + 1] = new Vertex(_x, _y, _z + 1f,
                        UVValues.BlockUVWidth * 6,
                        UVValues.BlockUVHeight * (blockType - 1)); // Top right
                    verticies[vertexCount + 2] = new Vertex(_x + 1f, _y, _z,
                        UVValues.BlockUVWidth * 5,
                        UVValues.BlockUVHeight * blockType); // bottom left
                    verticies[vertexCount + 3] = new Vertex(_x, _y, _z,
                        UVValues.BlockUVWidth * 6,
                        UVValues.BlockUVHeight * blockType); // bottom right
                    break;
            }

            indicies[indexCount] = (uint)vertexCount + 0;
            indicies[indexCount + 1] = (uint)vertexCount + 1;
            indicies[indexCount + 2] = (uint)vertexCount + 2;
            indicies[indexCount + 3] = (uint)vertexCount + 2;
            indicies[indexCount + 4] = (uint)vertexCount + 1;
            indicies[indexCount + 5] = (uint)vertexCount + 3;

            vertexCount += 4;
            indexCount += 6;
        }
    }
}