﻿using Assets.Code.Game;
using Assets.Code.Game.ClientPlayer;
using Assets.Code.Game.Factories;
using Client.Net;
using Common.Networking.Packets;
using CommonCode.EventBus;
using CommonCode.Networking.Packets;
using MapHandler;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Net.PacketListeners
{
    public class PlayerListener : IEventListener
    {
        [EventMethod]
        // When something goes wrong, we sync (force) the player to a state
        public void OnPlayerSync(SyncPacket packet)
        {
            Position destination = null;
            if (UnityClient.Player.FollowingPath != null && UnityClient.Player.FollowingPath.Count > 0)
            {
                destination = UnityClient.Player.FollowingPath.Last();
            }
            UnityClient.Player.PlayerObject.GetComponent<PlayerBehaviour>().StopMovement();
            UnityClient.Player.FollowingPath = null;
            UnityClient.Player.TeleportToTile(packet.Position.X, packet.Position.Y);
            Debug.Log("SYNC TO " + packet.Position.X + " - " + packet.Position.Y);

            // recalculating route to destination
            if (destination != null)
            {
                var path = WorldMap<Chunk>.FindPath(UnityClient.Player.Position, destination, UnityClient.Map.Chunks);
                if (path != null)
                {
                    UnityClient.Player.FollowingPath = path;
                }
            }
        }

        [EventMethod]
        public void OnPlayerMove(EntityMovePacket packet)
        {
            var entityObj = GameObject.Find(packet.UID);
            if (entityObj != null)
            {
                var movingEntity = entityObj.GetComponent<MovingEntityBehaviour>();
                if(movingEntity != null)
                {
                    movingEntity.Route.Add(packet.To);
                }
            }
        }

        [EventMethod]
        public void OnPlayerAppears(PlayerPacket packet)
        {
            // instantiate the player if needed
            PlayerFactory.BuildAndInstantiate(new PlayerFactoryOptions()
            {
                HeadSpriteIndex = packet.HeadSpriteIndex,
                BodySpriteIndex = packet.BodySpriteIndex,
                LegsSpriteIndex = packet.LegSpriteIndex,
                ChestSpriteIndex = packet.ChestSpriteIndex,
                UserId = packet.UserId,
                Speed = packet.Speed,
                tileX = packet.X,
                tileY = packet.Y,
                IsMainPlayer = packet.UserId == UnityClient.Player.UserId
            });
            TouchHandler.GameTouchEnabled = true;
        }

        public static void PlayerSetTarget(GameObject target)
        {
            UnityClient.TcpClient.Send(new TargetPacket()
            {
                TargetUuid = target.name,
                WhoUuid = UnityClient.Player.UserId
            });
            UnityClient.Player.Target = target;
        }
    }
}
