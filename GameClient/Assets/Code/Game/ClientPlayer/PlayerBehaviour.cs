﻿using Assets.Code.AssetHandling.Sprites.Animations;
using Assets.Code.Game;
using Client.Net;
using Common.Networking.Packets;
using CommonCode.EntityShared;
using CommonCode.Networking.Packets;
using MapHandler;
using System;
using System.Linq;
using UnityEngine;

public class PlayerBehaviour : MovingEntityBehaviour
{
    public static long NOW_MILLIS = 0;

    public override void OnBeforeUpdate()
    {
        NOW_MILLIS = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        TryPerformAttack();
    }

    private void TryPerformAttack()
    {
        if (UnityClient.Player.Target != null)
        {
            if (UnityClient.Player.NextAttackAt <= NOW_MILLIS)
            {
                var targetPosition = UnityClient.Player.Target.Position;
                var distanceToTarget = UnityClient.Player.Position.GetDistance(targetPosition);
                if (distanceToTarget <= 1)
                {
                    UnityClient.TcpClient.Send(new EntityAttackPacket()
                    {
                        EntityUID = UnityClient.Player.Target.UID
                    });
                    var atkSpeedDelay = Formulas.GetTimeBetweenAttacks(UnityClient.Player.AtkSpeed);
                    var nextAttack = NOW_MILLIS + atkSpeedDelay;
                    UnityClient.Player.NextAttackAt = nextAttack;

                    SpriteSheets.ForEach(e => e.SetDirection(UnityClient.Player.Position.GetDirection(targetPosition)));

                    UnityClient.Player.Movement.SpriteSheets.ForEach(e => {
                        e.SetAnimation(SpriteAnimations.ATTACKING, atkSpeedDelay);
                    });

                }
            }
        }
    }

    public override void OnFinishRoute()
    {
        // Hide the green square on the ground when i finish moving
        if (this.Route.Count == 0)
            Selectors.HideSelector();
    }

    public override void OnBeforeMoveTile(Position movingTo)
    {
        
        // Inform the server i moved
        UnityClient.TcpClient.Send(new EntityMovePacket()
        {
            UID = UnityClient.Player.UID,
            To = movingTo
        });

        var playerPos = UnityClient.Player.Position;

        var currentChunk = UnityClient.Map.GetChunkByTilePosition(playerPos.X, playerPos.Y);
        var newChunk = UnityClient.Map.GetChunkByTilePosition(movingTo.X, movingTo.Y);
        
        if (currentChunk.x != newChunk.x || currentChunk.y != newChunk.y)
        {
            var currentChunkX = playerPos.X >> 4;
            var currentChunkY = playerPos.Y >> 4;

            var newChunkX = movingTo.X >> 4;
            var newChunkY = movingTo.Y >> 4;

            var currentChunkParents = PositionExtensions.GetSquared3x3Around(new Position(currentChunkX, currentChunkY)).ToList();
            var newChunkParents = PositionExtensions.GetSquared3x3Around(new Position(newChunkX, newChunkY)).ToList();
            
            foreach (var cc in currentChunkParents.Except(newChunkParents))
            {
                var chunk = UnityClient.Map.GetChunkByChunkPosition(cc.X, cc.Y);
                if (chunk != null)
                    chunk.GameObject.SetActive(false);
            }

            foreach (var nc in newChunkParents.Except(currentChunkParents))
            {
                var chunk = UnityClient.Map.GetChunkByChunkPosition(nc.X, nc.Y);
                if(chunk != null)
                    chunk.GameObject.SetActive(true);
            }
        }
    }
}
