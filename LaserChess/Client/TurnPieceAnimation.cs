using System;
using LaserChess.Logic;
using TG.Skin;

namespace LaserChess.Client
{
	public class TurnPieceAnimation : Animation
	{
		public TurnPieceAnimation(Game game, SkinningClass skin)
		{
			this.game = game;
			foreach (Piece piece in game.PieceList)
			{
				piece.PieceLogic.TurnPieceEvent += this.OnTurnPiece;
			}
		}

		private void OnTurnPiece(object sender, TurnPieceEventArgs e)
		{
			this.fromFacing = e.Facing1;
			this.toFacing = e.Facing2;
			this.piece = this.game.FindPiece(e.Piece);
			this.Start();
		}

		public override bool IsIdle()
		{
			return this.state == TurnPieceAnimation.State.Idle;
		}

		public override void Update(float elapsed)
		{
			this.animations.Update(elapsed);
			TurnPieceAnimation.State @enum = this.state;
			if (@enum == TurnPieceAnimation.State.Turning)
			{
				this.UpdateTurning(elapsed);
				return;
			}
			this.Stop();
		}

		private void Start()
		{
			this.animations.Clear();
			float num = Piece.FacingToAngle(this.fromFacing);
			float num2 = Piece.FacingToAngle(this.toFacing);
			float num3 = num2 - num;
			if (num3 < -180f)
			{
				num3 += 360f;
			}
			if (num3 > 180f)
			{
				num3 -= 360f;
			}
			num2 = num + num3;
			int frames = this.piece.Frames;
			int num4 = (int)(num * (float)frames / 360f);
			int num5 = (int)(num2 * (float)frames / 360f);
			this.animations.Add(new DisplayAnimation(this.piece, num4, num5, frames, 0.25f));
			this.game.SoundSystem.PlaySound(SoundSystem.Sound.TurnPiece);
			this.state = TurnPieceAnimation.State.Turning;
		}

		private void UpdateTurning(float elapsed)
		{
			if (this.animations.IsIdle())
			{
				this.OnTurnFinished();
			}
		}

		private void OnTurnFinished()
		{
			this.Stop();
		}

		private void Stop()
		{
			this.animations.Clear();
			this.state = TurnPieceAnimation.State.Idle;
		}

		private const float TurnDuration = 0.25f;

		private Game game;

		private Direction fromFacing;

		private Direction toFacing;

		private Piece piece;

		private TurnPieceAnimation.State state;

		private AnimationList animations = new AnimationList();

		private enum State
		{
			Idle,
			Turning
		}
	}
}
