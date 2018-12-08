using NUnit.Framework;
using MapHandler;
using ServerCore.Game.GameMap;
using ServerCore.Game.Monsters;
using CommonCode.Networking.Packets;

namespace MapTests
{
    [TestFixture]
    public class AssetLoaderTests
    {
        [Test]
        public void TestSimpleLoading()
        {
            ServerMap map = AssetLoader.LoadMapFromFile();

            Assert.That(map.Tilesets.Count > 0);

            Assert.That(map.Chunks.Count >= 2);
        }

        [Test]
        public void TestLoadingSpawners()
        {
            ServerMap map = AssetLoader.LoadMapFromFile("test");

            Assert.That(map.Spawners.Count > 0);

            var spawner = map.Spawners[0];

            Assert.That(spawner.SpawnerMobs.Count == 1);

            var spawnerMob = spawner.SpawnerMobs[0];

            Assert.AreEqual(typeof(Skeleton), spawnerMob.MonsterClassType);
            Assert.AreEqual(1, spawnerMob.Amount);

        }

        [Test]
        public void TestGeneralAssetLoading()
        {
            AssetLoader.LoadServerAssets();

            Assert.That(AssetLoader.LoadedAssets[AssetType.ANIMATION].Count > 0);
            Assert.That(AssetLoader.LoadedAssets[AssetType.TILESET].Count > 0);
            Assert.That(AssetLoader.LoadedAssets[AssetType.SPRITE].Count > 0);
        }

        [Test]
        public void TestMapLoadTestMap()
        {
            ServerMap map = AssetLoader.LoadMapFromFile("test_map");

            Assert.That(map.Chunks.Count >= 2);

            Chunk chunk = map.GetChunk(0, 0);

            Assert.AreEqual(chunk.GetTile(0, 0), 1);
            Assert.AreEqual(chunk.GetTile(1, 0), 2);
            Assert.AreEqual(chunk.GetTile(2, 0), 3);

            Assert.AreEqual(chunk.GetTile(0, 1), 4);
            Assert.AreEqual(chunk.GetTile(1, 1), 3);
            Assert.AreEqual(chunk.GetTile(2, 1), 2);
            Assert.AreEqual(chunk.GetTile(3, 1), 1);

            chunk = map.GetChunk(0, 0);
        }
    }
}

