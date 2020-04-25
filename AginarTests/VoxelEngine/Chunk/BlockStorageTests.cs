using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AginarTests
{
    using Aginar.VoxelEngine.ChunkData;
    using Aginar.VoxelEngine;
    [TestClass]
    public class BlockStorageTests
    {
        [TestMethod]
        public void TestWrite()
        {
            BlockStorage blockStorage = new BlockStorage(5);

            blockStorage.setBlock(2, default(Grass));

            Assert.AreEqual(blockStorage.getBlock(2), default(Grass));
        }

        [TestMethod]
        public void TestRead()
        {
        }
    }
}
