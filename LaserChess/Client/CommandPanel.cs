using System;
using System.Drawing;
using LaserChess.Logic;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	public class CommandPanel
	{
		public void Init(GraphicsClass graphicsClass, Game game)
		{
			if (game == null)
			{
				throw new ArgumentNullException("game");
			}
			this.game = game;
			this.moveUpButton = new Button(graphicsClass, "ButtonMoveUp", game.SoundSystem);
			this.moveDownButton = new Button(graphicsClass, "ButtonMoveDown", game.SoundSystem);
			this.moveLeftButton = new Button(graphicsClass, "ButtonMoveLeft", game.SoundSystem);
			this.moveRightButton = new Button(graphicsClass, "ButtonMoveRight", game.SoundSystem);
			this.turnLeftButton = new Button(graphicsClass, "ButtonTurnLeft", game.SoundSystem);
			this.turnRightButton = new Button(graphicsClass, "ButtonTurnRight", game.SoundSystem);
			this.fireLaserButton = new Button(graphicsClass, "ButtonFireLaser", game.SoundSystem);
			this.moveUpButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.MoveUpClicked);
			this.moveDownButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.MoveDownClicked);
			this.moveLeftButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.MoveLeftClicked);
			this.moveRightButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.MoveRightClicked);
			this.turnLeftButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.TurnLeftClicked);
			this.turnRightButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.TurnRightClicked);
			this.fireLaserButton.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.FireLaserClicked);
			this.Hide();
		}

		public LaserChess.Logic.Piece PieceLogic
		{
			get
			{
				return this.pieceLogic;
			}
			set
			{
				this.pieceLogic = value;
				if (this.pieceLogic != null)
				{
					this.SetOrigin(this.game.Board.GetTileCenter(this.pieceLogic.Tile));
					this.ShowButtons();
					return;
				}
				this.Hide();
			}
		}

		public void Hide()
		{
			this.moveUpButton.Hide();
			this.moveDownButton.Hide();
			this.moveLeftButton.Hide();
			this.moveRightButton.Hide();
			this.turnLeftButton.Hide();
			this.turnRightButton.Hide();
			this.fireLaserButton.Hide();
		}

		private void ShowButtons()
		{
			this.moveUpButton.Show();
			this.moveDownButton.Show();
			this.moveLeftButton.Show();
			this.moveRightButton.Show();
			this.turnLeftButton.Show();
			this.turnRightButton.Show();
			this.fireLaserButton.Show();
			this.HideInvalidButtons();
		}

		private void HideInvalidButtons()
		{
			if (!this.pieceLogic.CanMovePiece(Board.NormalizeDirection(Direction.Up)))
			{
				this.moveUpButton.Hide();
			}
			if (!this.pieceLogic.CanMovePiece(Board.NormalizeDirection(Direction.Down)))
			{
				this.moveDownButton.Hide();
			}
			if (!this.pieceLogic.CanMovePiece(Board.NormalizeDirection(Direction.Left)))
			{
				this.moveLeftButton.Hide();
			}
			if (!this.pieceLogic.CanMovePiece(Board.NormalizeDirection(Direction.Right)))
			{
				this.moveRightButton.Hide();
			}
			if (!this.pieceLogic.CanTurnPiece(Direction.Left))
			{
				this.turnLeftButton.Hide();
			}
			if (!this.pieceLogic.CanTurnPiece(Direction.Right))
			{
				this.turnRightButton.Hide();
			}
			if (!this.pieceLogic.CanFireLaser(this.pieceLogic.FrontFacing))
			{
				this.fireLaserButton.Hide();
			}
		}

		private void SetOrigin(PointF origin)
		{
			this.moveUpButton.Origin = origin;
			this.moveDownButton.Origin = origin;
			this.moveLeftButton.Origin = origin;
			this.moveRightButton.Origin = origin;
			this.turnLeftButton.Origin = origin;
			this.turnRightButton.Origin = origin;
			this.fireLaserButton.Origin = origin;
		}

		private bool MoveUpClicked(SkinButtonClass button)
		{
			this.Hide();
			if (this.pieceLogic != null)
			{
				this.game.RaiseMoveRequestEvent(this.pieceLogic.Player.PlayerNumber, this.pieceLogic.PieceNumber, MoveCommand.MovePiece, Board.NormalizeDirection(Direction.Up));
			}
			return true;
		}

		private bool MoveDownClicked(SkinButtonClass button)
		{
			this.Hide();
			if (this.pieceLogic != null)
			{
				this.game.RaiseMoveRequestEvent(this.pieceLogic.Player.PlayerNumber, this.pieceLogic.PieceNumber, MoveCommand.MovePiece, Board.NormalizeDirection(Direction.Down));
			}
			return true;
		}

		private bool MoveLeftClicked(SkinButtonClass button)
		{
			this.Hide();
			if (this.pieceLogic != null)
			{
				this.game.RaiseMoveRequestEvent(this.pieceLogic.Player.PlayerNumber, this.pieceLogic.PieceNumber, MoveCommand.MovePiece, Board.NormalizeDirection(Direction.Left));
			}
			return true;
		}

		private bool MoveRightClicked(SkinButtonClass button)
		{
			this.Hide();
			if (this.pieceLogic != null)
			{
				this.game.RaiseMoveRequestEvent(this.pieceLogic.Player.PlayerNumber, this.pieceLogic.PieceNumber, MoveCommand.MovePiece, Board.NormalizeDirection(Direction.Right));
			}
			return true;
		}

		private bool TurnLeftClicked(SkinButtonClass button)
		{
			this.Hide();
			if (this.pieceLogic != null)
			{
				this.game.RaiseMoveRequestEvent(this.pieceLogic.Player.PlayerNumber, this.pieceLogic.PieceNumber, MoveCommand.TurnPiece, Direction.Left);
			}
			return true;
		}

		private bool TurnRightClicked(SkinButtonClass button)
		{
			this.Hide();
			if (this.pieceLogic != null)
			{
				this.game.RaiseMoveRequestEvent(this.pieceLogic.Player.PlayerNumber, this.pieceLogic.PieceNumber, MoveCommand.TurnPiece, Direction.Right);
			}
			return true;
		}

		private bool FireLaserClicked(SkinButtonClass button)
		{
			this.Hide();
			if (this.pieceLogic != null)
			{
				this.game.RaiseMoveRequestEvent(this.pieceLogic.Player.PlayerNumber, this.pieceLogic.PieceNumber, MoveCommand.FireLaser, this.pieceLogic.FrontFacing);
			}
			return true;
		}

		private Button moveUpButton;

		private Button moveDownButton;

		private Button moveLeftButton;

		private Button moveRightButton;

		private Button turnLeftButton;

		private Button turnRightButton;

		private Button fireLaserButton;

		private Game game;

		private LaserChess.Logic.Piece pieceLogic;
	}
}
