using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AginarTests
{
    using Aginar.VoxelEngine.ChunkData;
    using Aginar.VoxelEngine;
    using System;

    [TestClass]
    public class ChunkStorageTests
    {
        [TestMethod]
        public void BlockStorageTests()
        {
            BlockStorage blockStorage = new BlockStorage(5);

            blockStorage.SetBlock(2, 1);
            blockStorage.SetBlock(4, 4);

            Assert.AreEqual(1, blockStorage.GetBlock(2));
            Assert.AreEqual(4, blockStorage.GetBlock(4));
        }

        [TestMethod]
        public void BitBufferTests()
        {
            BitBuffer bitBuffer = new BitBuffer(10);

            bitBuffer.SetBits(3, 3, new BitArray32(5));

            Assert.AreEqual((int)bitBuffer.GetBits(3, 3).Data, 5);
        }

    }
}
