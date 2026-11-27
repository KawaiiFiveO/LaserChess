using System;
using System.Collections.ObjectModel;
using System.Drawing;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	public class FireLaserAnimation : Animation
	{
		public FireLaserAnimation(Game game, SkinningClass skin)
		{
			this.skin = skin;
			this.game = game;
			foreach (Piece piece in game.PieceList)
			{
				piece.PieceLogic.FireLaserEvent += this.OnFireLaser;
			}
		}

		private void OnFireLaser(object sender, FireLaserEventArgs e)
		{
			this.gunGlow.Clear();
			this.beamGlow.Clear();
			this.targetGlow.Clear();
			this.hitPieces.Clear();
			this.animations.Clear();
			string text = ((e.Piece1.Player.PlayerNumber == 0) ? "White" : "Black");
			SkinSprite skinSprite = new SkinSprite(this.skin, text + "LaserGunGlow");
			skinSprite.Rotation = Piece.FacingToAngle(e.Piece1.FrontFacing) - 90f;
			skinSprite.Translation = this.game.Board.GetTileCenter(e.Tile1);
			skinSprite.Display = 0;
			this.gunGlow.Add(skinSprite);
			this.AddBeamSprites(text);
			this.AddTargetSprites(text);
			this.Start();
		}

		private void AddBeamSprites(string color)
		{
			foreach (LaserBeam laserBeam in this.game.GameLogic.LaserBeamList)
			{
				SkinSprite skinSprite = new SkinSprite(this.skin, color + "LaserBeamGlow");
				skinSprite.Scaling = new PointF(this.game.Board.TileSize.X / skinSprite.Width, 1f);
				skinSprite.Center = new PointF(0f, 0.5f);
				skinSprite.Rotation = Piece.FacingToAngle(laserBeam.Direction) - 90f;
				skinSprite.Translation = this.game.Board.GetTileCenter(laserBeam.Tile);
				skinSprite.Display = 0;
				this.beamGlow.Add(skinSprite);
			}
		}

		private void AddTargetSprites(string color)
		{
			foreach (Piece piece in this.game.PieceList)
			{
				if (piece.PieceLogic.LaserHit)
				{
					this.hitPieces.Add(piece);
					Tile tile = piece.PieceLogic.Tile;
					SkinSprite skinSprite = new SkinSprite(this.skin, color + "LaserTargetGlow");
					skinSprite.Translation = this.game.Board.GetTileCenter(tile);
					skinSprite.Display = 0;
					this.targetGlow.Add(skinSprite);
				}
			}
		}

		public override bool IsIdle()
		{
			return this.state == FireLaserAnimation.State.Idle;
		}

		public override void Update(float elapsed)
		{
			this.animations.Update(elapsed);
			switch (this.state)
			{
			case FireLaserAnimation.State.GunGlow:
				this.UpdateGunGlow(elapsed);
				return;
			case FireLaserAnimation.State.Hit:
				this.UpdateHit(elapsed);
				return;
			case FireLaserAnimation.State.FadingOut:
				this.UpdateFadingOut(elapsed);
				return;
			default:
				this.Stop();
				return;
			}
		}

		private void Start()
		{
			this.animations.Clear();
			this.animations.Add(new FadeInAnimation(this.gunGlow, 0.8f));
			this.game.SoundSystem.PlaySound(SoundSystem.Sound.FireLaser);
			this.state = FireLaserAnimation.State.GunGlow;
		}

		private void UpdateGunGlow(float elapsed)
		{
			if (this.animations.IsIdle())
			{
				this.StartHit();
			}
		}

		private void StartHit()
		{
			this.animations.Clear();
			this.animations.Add(new FadeInAnimation(this.beamGlow, 0.25f));
			this.animations.Add(new FadeInAnimation(this.targetGlow, 0.25f));
			foreach (Piece piece in this.hitPieces)
			{
				this.animations.Add(new FadeOutAnimation(piece, 0.25f));
				switch (this.random.Next(4))
				{
				case 0:
					this.game.SoundSystem.PlaySound(SoundSystem.Sound.PieceHit1);
					break;
				case 1:
					this.game.SoundSystem.PlaySound(SoundSystem.Sound.PieceHit2);
					break;
				case 2:
					this.game.SoundSystem.PlaySound(SoundSystem.Sound.PieceHit3);
					break;
				case 3:
					this.game.SoundSystem.PlaySound(SoundSystem.Sound.PieceHit4);
					break;
				}
			}
			this.state = FireLaserAnimation.State.Hit;
		}

		private void UpdateHit(float elapsed)
		{
			if (this.animations.IsIdle())
			{
				this.StartFadeOut();
			}
		}

		private void StartFadeOut()
		{
			this.animations.Clear();
			this.animations.Add(new FadeOutAnimation(this.gunGlow, 1f));
			this.animations.Add(new FadeOutAnimation(this.beamGlow, 1f));
			this.animations.Add(new FadeOutAnimation(this.targetGlow, 1f));
			this.state = FireLaserAnimation.State.FadingOut;
		}

		private void UpdateFadingOut(float elapsed)
		{
			if (this.animations.IsIdle())
			{
				this.Stop();
			}
		}

		private void Stop()
		{
			this.animations.Clear();
			this.state = FireLaserAnimation.State.Idle;
		}

		private const float GunGlowDuration = 0.8f;

		private const float HitDuration = 0.25f;

		private const float FadeOutDuration = 1f;

		private SkinningClass skin;

		private Game game;

		private FireLaserAnimation.State state;

		private SpriteList gunGlow = new SpriteList();

		private SpriteList beamGlow = new SpriteList();

		private SpriteList targetGlow = new SpriteList();

		private Collection<Piece> hitPieces = new Collection<Piece>();

		private AnimationList animations = new AnimationList();

		private Random random = new Random();

		private enum State
		{
			Idle,
			GunGlow,
			Hit,
			FadingOut
		}
	}
}
