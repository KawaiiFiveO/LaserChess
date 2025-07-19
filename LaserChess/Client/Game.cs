using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using LaserChess.Logic;
using TG.DxGame;

namespace LaserChess.Client
{
	// Token: 0x02000011 RID: 17
	public class Game : IDisposable
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000023BC File Offset: 0x000005BC
		public SoundSystem SoundSystem
		{
			get
			{
				return this.soundSystem_0;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000063 RID: 99 RVA: 0x000023C4 File Offset: 0x000005C4
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00004340 File Offset: 0x00002540
		public int LocalPlayerNumber
		{
			get
			{
				return this.int_1;
			}
			set
			{
				this.int_1 = value;
				Board.Flip = this.int_1 == 1;
				for (int i = 0; i < 2; i++)
				{
					this.player_0[i].ShowAllPieces();
				}
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000065 RID: 101 RVA: 0x000023CC File Offset: 0x000005CC
		// (remove) Token: 0x06000066 RID: 102 RVA: 0x000023E5 File Offset: 0x000005E5
		public event EventHandler<MoveRequestEventArgs> MoveRequestEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.eventHandler_0 = (EventHandler<MoveRequestEventArgs>)Delegate.Combine(this.eventHandler_0, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.eventHandler_0 = (EventHandler<MoveRequestEventArgs>)Delegate.Remove(this.eventHandler_0, value);
			}
		}

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x06000067 RID: 103 RVA: 0x000023FE File Offset: 0x000005FE
        public Logic.Game GameLogic
        {
			get
			{
				return this.game_0;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00002406 File Offset: 0x00000606
		public Collection<Piece> PieceList
		{
			get
			{
				return this.collection_0;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000069 RID: 105 RVA: 0x0000240E File Offset: 0x0000060E
		public Board Board
		{
			get
			{
				return this.board_0;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000437C File Offset: 0x0000257C
		public Game(Logic.Game game_1, Control control_0)
		{
			this.soundSystem_0 = new SoundSystem(control_0);
			this.game_0 = game_1;
			this.board_0 = new Board(this.game_0.Board);
			for (int i = 0; i < 2; i++)
			{
				this.player_0[i] = new Player(this, this.game_0.GetPlayer(i));
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000440C File Offset: 0x0000260C
		public void Init(GraphicsClass graphicsClass_0)
		{
			if (graphicsClass_0 == null)
			{
				return;
			}
			this.soundSystem_0.Init(graphicsClass_0);
			this.board_0.Init(graphicsClass_0);
			this.player_0[0].Init(graphicsClass_0, "White");
			this.player_0[1].Init(graphicsClass_0, "Black");
			this.animationList_0.Add(new MovePieceAnimation(this, graphicsClass_0.Skin));
			this.animationList_0.Add(new TurnPieceAnimation(this, graphicsClass_0.Skin));
			this.animationList_0.Add(new FireLaserAnimation(this, graphicsClass_0.Skin));
			this.animationList_0.Add(new CaptureAnimation(this, graphicsClass_0.Skin));
			this.animationList_0.Add(new TeleportAnimation(this, graphicsClass_0.Skin));
		}

        // Token: 0x0600006C RID: 108 RVA: 0x00002416 File Offset: 0x00000616
        public void RegisterPiece(Piece piece_0, LaserChess.Logic.Piece piece_1)
        {
			this.collection_0.Add(piece_0);
			this.hashtable_0[piece_1] = piece_0;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002431 File Offset: 0x00000631
		public Piece FindPiece(LaserChess.Logic.Piece piece_0)
		{
			return (Piece)this.hashtable_0[piece_0];
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000044D0 File Offset: 0x000026D0
		public void Animate(float float_0)
		{
			try
			{
				this.animationList_0.Update(float_0);
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Concat(new string[]
				{
					"Exception occurred in ",
					ex.Source,
					Environment.NewLine,
					ex.Message,
					Environment.NewLine,
					ex.StackTrace
				}), "TrayGames Alert", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00002444 File Offset: 0x00000644
		public AnimationList AnimationList
		{
			get
			{
				return this.animationList_0;
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000244C File Offset: 0x0000064C
		public void RaiseMoveRequestEvent(int int_2, int int_3, MoveCommand moveCommand_0, Direction direction_0)
		{
			if (this.eventHandler_0 != null)
			{
				this.eventHandler_0(this, new MoveRequestEventArgs(int_2, int_3, moveCommand_0, direction_0));
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000454C File Offset: 0x0000274C
		protected virtual void Dispose(bool bool_1)
		{
			lock (this)
			{
				if (!this.bool_0)
				{
					if (bool_1 && this.soundSystem_0 != null)
					{
						this.soundSystem_0.Dispose();
						this.soundSystem_0 = null;
					}
					this.bool_0 = true;
				}
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000246C File Offset: 0x0000066C
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000060 RID: 96
		private const int int_0 = 2;

		// Token: 0x04000061 RID: 97
		private bool bool_0;

		// Token: 0x04000062 RID: 98
		private SoundSystem soundSystem_0;

		// Token: 0x04000063 RID: 99
		private int int_1;

		// Token: 0x04000064 RID: 100
		private AnimationList animationList_0 = new AnimationList();

		// Token: 0x04000065 RID: 101
		private EventHandler<MoveRequestEventArgs> eventHandler_0;

		// Token: 0x04000066 RID: 102
		private Logic.Game game_0;

		// Token: 0x04000067 RID: 103
		private Board board_0;

		// Token: 0x04000068 RID: 104
		private Player[] player_0 = new Player[2];

		// Token: 0x04000069 RID: 105
		private Hashtable hashtable_0 = new Hashtable();

		// Token: 0x0400006A RID: 106
		private Collection<Piece> collection_0 = new Collection<Piece>();
	}
}
