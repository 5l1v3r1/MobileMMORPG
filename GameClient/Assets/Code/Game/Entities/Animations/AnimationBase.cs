﻿
using Assets.Code.Game.Entities;
using MapHandler;

namespace Assets.Code.AssetHandling.Sprites.Animations
{
    public class AnimationBase
    {
        public bool IsOver = false;
        public SpriteSheet SpriteSheet;
        public int CurrentFrame = 0;
        public int OffsetX = 0;
        public int OffsetY = 0;
        public float AnimationTimeInSeconds = 0.1f;

        public void Reset()
        {
            CurrentFrame = 0;
            OffsetX = 0;
            OffsetY = 0;
            IsOver = false;
            OnReset();
        }

        public virtual AnimationResult Loop(Direction dir) => null;

        public virtual void OnReset() { }

        public AnimationBase(SpriteSheet sheet)
        {
            this.SpriteSheet = sheet;
        }
    }
}
