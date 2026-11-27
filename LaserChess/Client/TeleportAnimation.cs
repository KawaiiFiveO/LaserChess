using System;
using System.Drawing;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	public class TeleportAnimation : Animation
	{
		public TeleportAnimation(Game game, SkinningClass skin)
		{
			this.game = game;
			foreach (Piece piece in game.PieceList)
			{
				piece.PieceLogic.TeleportEvent += this.OnTeleport;
			}
		}

		private void OnTeleport(object sender, TeleportEventArgs e)
		{
			this.fromTile = e.Tile1;
			this.padTile = e.Tile2;
			this.destinationTile = e.Tile3;
			this.movingPiece = null;
			this.teleportingPiece = null;
			this.destroyedPiece = null;
			if (e.Piece1 != null)
			{
				this.movingPiece = this.game.FindPiece(e.Piece1);
			}
			if (e.Piece2 != null)
			{
				this.teleportingPiece = this.game.FindPiece(e.Piece2);
			}
			if (e.Piece3 != null)
			{
				this.destroyedPiece = this.game.FindPiece(e.Piece3);
			}
			this.Start();
		}

		public override bool IsIdle()
		{
			return this.state == TeleportAnimation.State.Idle;
		}

		public override void Update(float elapsed)
		{
			this.animations.Update(elapsed);
			switch (this.state)
			{
			case TeleportAnimation.State.Moving:
				this.UpdateMoving();
				return;
			case TeleportAnimation.State.FadingOut:
				this.UpdateFadingOut();
				return;
			case TeleportAnimation.State.FadingIn:
				this.UpdateFadingIn();
				return;
			default:
				this.Stop();
				return;
			}
		}

		private void Start()
		{
			this.animations.Clear();
			PointF tileCenter = this.game.Board.GetTileCenter(this.fromTile);
			PointF tileCenter2 = this.game.Board.GetTileCenter(this.padTile);
			this.animations.Add(new TranslationAnimation(this.movingPiece, tileCenter, tileCenter2, 0.25f));
			this.game.SoundSystem.PlaySound(SoundSystem.Sound.MovePiece);
			this.state = TeleportAnimation.State.Moving;
		}

		private void UpdateMoving()
		{
			if (this.animations.IsIdle())
			{
				this.OnMoveFinished();
			}
		}

		private void OnMoveFinished()
		{
			this.StartFadeOut();
		}

		private void StartFadeOut()
		{
			this.animations.Clear();
			this.animations.Add(new FadeOutAnimation(this.teleportingPiece, 1.2f));
			this.game.SoundSystem.PlaySound(SoundSystem.Sound.TeleportOut);
			this.state = TeleportAnimation.State.FadingOut;
		}

		private void UpdateFadingOut()
		{
			if (this.animations.IsIdle())
			{
				this.OnFadeOutFinished();
			}
		}

		private void OnFadeOutFinished()
		{
			if (this.teleportingPiece != this.destroyedPiece)
			{
				this.StartFadeIn();
				return;
			}
			this.Stop();
		}

		private void StartFadeIn()
		{
			this.animations.Clear();
			PointF tileCenter = this.game.Board.GetTileCenter(this.destinationTile);
			this.teleportingPiece.Translation = tileCenter;
			this.animations.Add(new FadeInAnimation(this.teleportingPiece, 0.6f));
			if (this.destroyedPiece != null)
			{
				if (this.destroyedPiece != this.teleportingPiece)
				{
					this.animations.Add(new FadeOutAnimation(this.destroyedPiece, 1.2f));
				}
			}
			this.game.SoundSystem.PlaySound(SoundSystem.Sound.TeleportIn);
			this.state = TeleportAnimation.State.FadingIn;
		}

		private void UpdateFadingIn()
		{
			if (this.animations.IsIdle())
			{
				this.OnFadeInFinished();
			}
		}

		private void OnFadeInFinished()
		{
			this.Stop();
		}

		private void Stop()
		{
			this.animations.Clear();
			this.state = TeleportAnimation.State.Idle;
		}

		private const float MoveDuration = 0.25f;

		private const float FadeOutDuration = 1.2f;

		private const float FadeInDuration = 0.6f;

		private Game game;

		private Tile fromTile;

		private Tile padTile;

		private Tile destinationTile;

		private Piece movingPiece;

		private Piece teleportingPiece;

		private Piece destroyedPiece;

		private TeleportAnimation.State state;

		private AnimationList animations = new AnimationList();

		private enum State
		{
			Idle,
			Moving,
			FadingOut,
			FadingIn
		}
	}
}
