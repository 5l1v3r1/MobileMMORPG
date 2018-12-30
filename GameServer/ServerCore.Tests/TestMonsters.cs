using NUnit.Framework;
using MapHandler;
using Storage;
using Storage.TestDataBuilder;
using ServerCore;
using ServerCore.Tests.TestUtilities;
using Common.Networking.Packets;
using Storage.Players;
using ServerCore.Game.Monsters;
using System.Linq;
using ServerCore.GameServer.Entities;
using ServerCore.Game.Entities;
using static ServerCore.Server;
using System.Threading;
using System.Collections.Generic;

namespace GameTests
{
    [TestFixture]
    public class TestMonsters
    {
        private StoredPlayer _player;
        private Server _server;

        [SetUp]
        public void Start()
        {
            Redis redis = new Redis();
            redis.Start();
            TestDb.Create();
            _server = new Server(new ServerStartConfig() { Port = 123 });
            _server.StartGameThread();
            _player = new StoredPlayer()
            {
                UserId = "wololo",
                Login = "login",
                Password = "password",
                MoveSpeed = 1,
                X = 0,
                Y = 0
            };
        }

        [TearDown]
        public void TearDown()
        {
            _server.Stop();
        }


        [Test]
        public void TestMonsterMovingUpdatingClients()
        {
            var client = ServerMocker.GetClient();
            var player = client.FullLoginSequence(_player);

            var skeleton = new Skeleton()
            {
                Position = new Position(0, 1)
            };
            var chunk = player.GetChunk();

            chunk.EntitiesInChunk[EntityType.MONSTER].Add(skeleton);

            client.RecievedPackets.Clear();

            skeleton.MovementTick();
            skeleton.MovementTick();
            skeleton.MovementTick();

            var monsterMovePackets = client.RecievedPackets
                .Where(p => p.GetType() == typeof(EntityMovePacket))
                .Select(p => (EntityMovePacket)p)
                .Where(p => p.UID == skeleton.UID)
                .ToList();

            Assert.That(monsterMovePackets.Count > 0,
                "Player did not recieve updates from monster moving");
        }

        [Test]
        public void TestPlayerTooFarAwayToRecieveUpdates()
        {
            var client = ServerMocker.GetClient();
            var player = client.FullLoginSequence(_player);

            var skeleton = new Skeleton()
            {
                Position = new Position(33, 0)
            };

            var monsterChunk = Server.Map.GetChunkByChunkPosition(2,0);
            var playerChunki = Server.Map.GetChunkByChunkPosition(0, 0);

            var chunks = Server.Map.Chunks;

            monsterChunk.EntitiesInChunk[EntityType.MONSTER].Add(skeleton);

            client.RecievedPackets.Clear();

            skeleton.MovementTick();
            skeleton.MovementTick();
            skeleton.MovementTick();

            var monsterMovePackets = client.RecievedPackets
                .Where(p => p.GetType() == typeof(EntityMovePacket))
                .Select(p => (EntityMovePacket)p)
                .Where(p => p.UID == skeleton.UID)
                .ToList();

            Assert.That(monsterMovePackets.Count == 0,
                "Player Recieved Updates from a too far away monster");
        }

        [Test]
        public void TestPlayerMovingOnDifferentChunk()
        {
            var client = ServerMocker.GetClient();
            var player = client.FullLoginSequence(_player);

            var skeleton = new Skeleton()
            {
                Position = new Position(0, 22)
            };

            var monsterChunk = Server.Map.GetChunkByChunkPosition(0, 1);
            var playerChunki = Server.Map.GetChunkByChunkPosition(0, 0);

            monsterChunk.EntitiesInChunk[EntityType.MONSTER].Add(skeleton);

            client.RecievedPackets.Clear();

            skeleton.MovementTick();
            skeleton.MovementTick();
            skeleton.MovementTick();

            var monsterMovePackets = client.RecievedPackets
                .Where(p => p.GetType() == typeof(EntityMovePacket))
                .Select(p => (EntityMovePacket)p)
                .Where(p => p.UID == skeleton.UID)
                .ToList();

            Assert.That(monsterMovePackets.Count > 0,
                "Player did not recieve updates from monster moving");
        }
        
       [Test]
        public void TestPlayerMovingOnVariousDiferentChunk()
        {
            var client = ServerMocker.GetClient();
            var player = client.FullLoginSequence(_player);

            var chunkPackets = new List<BasePacket>();

            client.RecievedPackets.Clear();

            player.MoveSpeed = int.MaxValue;

            for (int i = 1; i <= 30; i++)
            {
                var newPosition = new Position(player.Position.X + 1, player.Position.Y);

                client.SendToServer(new EntityMovePacket
                {
                    ClientId = client.ConnectionId,
                    UID = player.UID,
                    To = newPosition
                });
                
                chunkPackets = client.RecievedPackets
                    .Where(p => p.GetType() == typeof(ChunkPacket))
                    .ToList();
            }

            Assert.That(chunkPackets.Count == 3,
                "Player did not recieve updates from chunk map");
        }
    }
}

