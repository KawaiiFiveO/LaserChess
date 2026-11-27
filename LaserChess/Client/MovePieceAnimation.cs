using System;
using System.Drawing;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	public class MovePieceAnimation : Animation
	{
		public MovePieceAnimation(Game game, SkinningClass skin)
		{
			this.game = game;
			foreach (Piece piece in game.PieceList)
			{
				piece.PieceLogic.MovePieceEvent += this.OnMovePiece;
			}
		}

		private void OnMovePiece(object sender, MovePieceEventArgs e)
		{
			this.fromTile = e.Tile1;
			this.toTile = e.Tile2;
			this.piece = this.game.FindPiece(e.Piece);
			this.Start();
		}

		public override bool IsIdle()
		{
			return this.state == MovePieceAnimation.State.Idle;
		}

		public override void Update(float elapsed)
		{
			this.animations.Update(elapsed);
			MovePieceAnimation.State @enum = this.state;
			if (@enum == MovePieceAnimation.State.Moving)
			{
				this.UpdateMoving(elapsed);
				return;
			}
			this.Stop();
		}

		private void Start()
		{
			this.animations.Clear();
			PointF tileCenter = this.game.Board.GetTileCenter(this.fromTile);
			PointF tileCenter2 = this.game.Board.GetTileCenter(this.toTile);
			this.animations.Add(new TranslationAnimation(this.piece, tileCenter, tileCenter2, 0.25f));
			this.game.SoundSystem.PlaySound(SoundSystem.Sound.MovePiece);
			this.state = MovePieceAnimation.State.Moving;
		}

		private void UpdateMoving(float elapsed)
		{
			if (this.animations.IsIdle())
			{
				this.OnMoveFinished();
			}
		}

		private void OnMoveFinished()
		{
			this.Stop();
		}

		private void Stop()
		{
			this.animations.Clear();
			this.state = MovePieceAnimation.State.Idle;
		}

		private const float MoveDuration = 0.25f;

		private Game game;

		private Tile fromTile;

		private Tile toTile;

		private Piece piece;

		private MovePieceAnimation.State state;

		private AnimationList animations = new AnimationList();

		private enum State
		{
			Idle,
			Moving
		}
	}
}
