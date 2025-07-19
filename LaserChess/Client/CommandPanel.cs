using System;
using System.Drawing;
using LaserChess.Logic;
using TG.DxGame;
using TG.Skin;

namespace LaserChess.Client
{
	// Token: 0x02000023 RID: 35
	public class CommandPanel
	{
		// Token: 0x060000BF RID: 191 RVA: 0x000056DC File Offset: 0x000038DC
		public void Init(GraphicsClass graphicsClass_0, Game game_1)
		{
			if (game_1 == null)
			{
				throw new ArgumentNullException("game");
			}
			this.game_0 = game_1;
			this.button_0 = new Button(graphicsClass_0, "ButtonMoveUp", game_1.SoundSystem);
			this.button_1 = new Button(graphicsClass_0, "ButtonMoveDown", game_1.SoundSystem);
			this.button_2 = new Button(graphicsClass_0, "ButtonMoveLeft", game_1.SoundSystem);
			this.button_3 = new Button(graphicsClass_0, "ButtonMoveRight", game_1.SoundSystem);
			this.button_4 = new Button(graphicsClass_0, "ButtonTurnLeft", game_1.SoundSystem);
			this.button_5 = new Button(graphicsClass_0, "ButtonTurnRight", game_1.SoundSystem);
			this.button_6 = new Button(graphicsClass_0, "ButtonFireLaser", game_1.SoundSystem);
			this.button_0.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_3);
			this.button_1.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_4);
			this.button_2.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_5);
			this.button_3.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_6);
			this.button_4.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_7);
			this.button_5.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_8);
			this.button_6.SkinItem.ClickRoutine = new SkinButtonClass.ClickRoutineCallback(this.method_9);
			this.Hide();
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00002766 File Offset: 0x00000966
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x0000276E File Offset: 0x0000096E
		public LaserChess.Logic.Piece PieceLogic
		{
			get
			{
				return this.piece_0;
			}
			set
			{
				this.piece_0 = value;
				if (this.piece_0 != null)
				{
					this.method_2(this.game_0.Board.GetTileCenter(this.piece_0.Tile));
					this.method_0();
					return;
				}
				this.Hide();
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000586C File Offset: 0x00003A6C
		public void Hide()
		{
			this.button_0.Hide();
			this.button_1.Hide();
			this.button_2.Hide();
			this.button_3.Hide();
			this.button_4.Hide();
			this.button_5.Hide();
			this.button_6.Hide();
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000058C8 File Offset: 0x00003AC8
		private void method_0()
		{
			this.button_0.Show();
			this.button_1.Show();
			this.button_2.Show();
			this.button_3.Show();
			this.button_4.Show();
			this.button_5.Show();
			this.button_6.Show();
			this.method_1();
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00005928 File Offset: 0x00003B28
		private void method_1()
		{
			if (!this.piece_0.CanMovePiece(Board.NormalizeDirection(Direction.Up)))
			{
				this.button_0.Hide();
			}
			if (!this.piece_0.CanMovePiece(Board.NormalizeDirection(Direction.Down)))
			{
				this.button_1.Hide();
			}
			if (!this.piece_0.CanMovePiece(Board.NormalizeDirection(Direction.Left)))
			{
				this.button_2.Hide();
			}
			if (!this.piece_0.CanMovePiece(Board.NormalizeDirection(Direction.Right)))
			{
				this.button_3.Hide();
			}
			if (!this.piece_0.CanTurnPiece(Direction.Left))
			{
				this.button_4.Hide();
			}
			if (!this.piece_0.CanTurnPiece(Direction.Right))
			{
				this.button_5.Hide();
			}
			if (!this.piece_0.CanFireLaser(this.piece_0.FrontFacing))
			{
				this.button_6.Hide();
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00005A04 File Offset: 0x00003C04
		private void method_2(PointF pointF_0)
		{
			this.button_0.Origin = pointF_0;
			this.button_1.Origin = pointF_0;
			this.button_2.Origin = pointF_0;
			this.button_3.Origin = pointF_0;
			this.button_4.Origin = pointF_0;
			this.button_5.Origin = pointF_0;
			this.button_6.Origin = pointF_0;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000027AD File Offset: 0x000009AD
		private bool method_3(SkinButtonClass skinButtonClass_0)
		{
			this.Hide();
			if (this.piece_0 != null)
			{
				this.game_0.RaiseMoveRequestEvent(this.piece_0.Player.PlayerNumber, this.piece_0.PieceNumber, MoveCommand.MovePiece, Board.NormalizeDirection(Direction.Up));
			}
			return true;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000027EB File Offset: 0x000009EB
		private bool method_4(SkinButtonClass skinButtonClass_0)
		{
			this.Hide();
			if (this.piece_0 != null)
			{
				this.game_0.RaiseMoveRequestEvent(this.piece_0.Player.PlayerNumber, this.piece_0.PieceNumber, MoveCommand.MovePiece, Board.NormalizeDirection(Direction.Down));
			}
			return true;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00002829 File Offset: 0x00000A29
		private bool method_5(SkinButtonClass skinButtonClass_0)
		{
			this.Hide();
			if (this.piece_0 != null)
			{
				this.game_0.RaiseMoveRequestEvent(this.piece_0.Player.PlayerNumber, this.piece_0.PieceNumber, MoveCommand.MovePiece, Board.NormalizeDirection(Direction.Left));
			}
			return true;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00002867 File Offset: 0x00000A67
		private bool method_6(SkinButtonClass skinButtonClass_0)
		{
			this.Hide();
			if (this.piece_0 != null)
			{
				this.game_0.RaiseMoveRequestEvent(this.piece_0.Player.PlayerNumber, this.piece_0.PieceNumber, MoveCommand.MovePiece, Board.NormalizeDirection(Direction.Right));
			}
			return true;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000028A5 File Offset: 0x00000AA5
		private bool method_7(SkinButtonClass skinButtonClass_0)
		{
			this.Hide();
			if (this.piece_0 != null)
			{
				this.game_0.RaiseMoveRequestEvent(this.piece_0.Player.PlayerNumber, this.piece_0.PieceNumber, MoveCommand.TurnPiece, Direction.Left);
			}
			return true;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000028DE File Offset: 0x00000ADE
		private bool method_8(SkinButtonClass skinButtonClass_0)
		{
			this.Hide();
			if (this.piece_0 != null)
			{
				this.game_0.RaiseMoveRequestEvent(this.piece_0.Player.PlayerNumber, this.piece_0.PieceNumber, MoveCommand.TurnPiece, Direction.Right);
			}
			return true;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005A68 File Offset: 0x00003C68
		private bool method_9(SkinButtonClass skinButtonClass_0)
		{
			this.Hide();
			if (this.piece_0 != null)
			{
				this.game_0.RaiseMoveRequestEvent(this.piece_0.Player.PlayerNumber, this.piece_0.PieceNumber, MoveCommand.FireLaser, this.piece_0.FrontFacing);
			}
			return true;
		}

		// Token: 0x040000BD RID: 189
		private Button button_0;

		// Token: 0x040000BE RID: 190
		private Button button_1;

		// Token: 0x040000BF RID: 191
		private Button button_2;

		// Token: 0x040000C0 RID: 192
		private Button button_3;

		// Token: 0x040000C1 RID: 193
		private Button button_4;

		// Token: 0x040000C2 RID: 194
		private Button button_5;

		// Token: 0x040000C3 RID: 195
		private Button button_6;

		// Token: 0x040000C4 RID: 196
		private Game game_0;

		// Token: 0x040000C5 RID: 197
		private LaserChess.Logic.Piece piece_0;
	}
}
