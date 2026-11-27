using System;
using System.Drawing;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	public class CaptureAnimation : Animation
	{
		public CaptureAnimation(Game game, SkinningClass skin)
		{
			this.game = game;
			foreach (Piece piece in game.PieceList)
			{
				piece.PieceLogic.CaptureEvent += this.OnCapture;
			}
		}

		private void OnCapture(object sender, CaptureEventArgs e)
		{
			this.fromTile = e.Tile1;
			this.toTile = e.Tile2;
			this.attacker = this.game.FindPiece(e.Piece1);
			this.captured = this.game.FindPiece(e.Piece2);
			this.Start();
		}

		public override bool IsIdle()
		{
			return this.state == CaptureAnimation.State.Idle;
		}

		public override void Update(float elapsed)
		{
			this.animations.Update(elapsed);
			CaptureAnimation.State @enum = this.state;
			if (@enum != CaptureAnimation.State.Capturing)
			{
				this.Stop();
				return;
			}
			this.UpdateCapturing(elapsed);
		}

		private void Start()
		{
			this.animations.Clear();
			PointF tileCenter = this.game.Board.GetTileCenter(this.fromTile);
			PointF tileCenter2 = this.game.Board.GetTileCenter(this.toTile);
			this.animations.Add(new TranslationAnimation(this.attacker, tileCenter, tileCenter2, 0.25f));
			this.animations.Add(new FadeOutAnimation(this.captured, 0.25f));
			this.game.SoundSystem.PlaySound(SoundSystem.Sound.CapturePiece);
			this.state = CaptureAnimation.State.Capturing;
		}

		private void UpdateCapturing(float elapsed)
		{
			if (this.animations.IsIdle())
			{
				this.OnCaptureFinished();
			}
		}

		private void OnCaptureFinished()
		{
			this.Stop();
		}

		private void Stop()
		{
			this.animations.Clear();
			this.state = CaptureAnimation.State.Idle;
		}

		private const float CaptureDuration = 0.25f;

		private Game game;

		private LaserChess.Logic.Tile fromTile;

		private LaserChess.Logic.Tile toTile;

		private Piece attacker;

		private Piece captured;

		private CaptureAnimation.State state;

		private AnimationList animations = new AnimationList();

		private enum State
		{
			Idle,
			Capturing
		}
	}
}
