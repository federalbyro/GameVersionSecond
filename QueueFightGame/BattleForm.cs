// BattleFormImproved.cs — FIXED VERSION (без Designer)
// ────────────────────────────────────────────────────────────────
// * Убрана лишняя строка InitializeComponent();
// * Добавлен Method SafeImageLoad кросс‑платформенный
// * Исправлена ширина _redPanel/_bluePanel при ресайзе
// * Других изменений логики нет — форма полностью автономна, Designer‑файл больше не нужен
// ----------------------------------------------------------------
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text;

namespace QueueFightGame.UI
{
    public class BattleForm : Form //  designer не нужен → класс не partial
    {
        private const int UnitDisplaySize = 96;
        private const int UnitDisplayMargin = 6;

        private readonly GameManager _gameManager;
        private readonly ILogger _battleLogger;

        private Panel _battleField;
        private FlowLayoutPanel _redPanel;
        private FlowLayoutPanel _bluePanel;

        private Label _roundLabel;
        private Label _turnLabel;
        private Label _statusLabel;
        private TextBox _logBox;
        private Button _nextBtn;
        private Button _undoBtn;
        private Button _exitBtn;
        private bool _redOnLeft;

        public BattleForm(Team redTeam, Team blueTeam)
        {
            _battleLogger = new MemoryLogger();
            _gameManager = new GameManager(_battleLogger);

            BuildUi();   // ← Designer больше не вызывается

            _gameManager.GameStateChanged += OnGameStateChanged;
            _gameManager.GameOver += (s, e) => { };
            _gameManager.StartGame(redTeam, blueTeam);
        }

        // ─────────────────── UI BUILD ───────────────────
        private void BuildUi()
        {
            Text = "Поле битвы";
            ClientSize = new Size(1280, 720);
            MinimumSize = new Size(960, 540);
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.Dpi;
            DoubleBuffered = true;

            _battleField = new Panel
            {
                Dock = DockStyle.Fill,
                BackgroundImage = SafeImageLoad("Resources/battle.png"),
                BackgroundImageLayout = ImageLayout.Stretch,
                Padding = new Padding(50, 60, 50, 140)
            };
            Controls.Add(_battleField);


         

            _redPanel = CreateTeamFlow(FlowDirection.LeftToRight);
            _bluePanel = CreateTeamFlow(FlowDirection.RightToLeft);
            _battleField.Controls.Add(_redPanel);
            _battleField.Controls.Add(_bluePanel);

            _roundLabel = MakeLabel("Раунд: 1");
            _turnLabel = MakeLabel("Ход:");
            _statusLabel = MakeLabel("Идёт игра…", Color.LimeGreen);

            var topBar = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                ColumnCount = 3,
                BackColor = Color.FromArgb(150, 0, 0, 0)
            };
            topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            topBar.Controls.Add(_roundLabel, 0, 0);
            topBar.Controls.Add(_turnLabel, 1, 0);
            topBar.Controls.Add(_statusLabel, 2, 0);
            _battleField.Controls.Add(topBar);

            _logBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 180,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9f)
            };
            _battleField.Controls.Add(_logBox);

            _nextBtn = MakeButton("Следующий ход", NextTurn);
            _undoBtn = MakeButton("Отменить ход", UndoTurn);
            _exitBtn = MakeButton("Выход", (s, e) => Close());

            var buttonBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 10),
                BackColor = Color.FromArgb(150, 0, 0, 0)
            };
            buttonBar.Controls.AddRange(new Control[] { _nextBtn, _undoBtn, _exitBtn });
            _battleField.Controls.Add(buttonBar);
        }

        private FlowLayoutPanel CreateTeamFlow(FlowDirection dir)
        {
            var pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = ClientSize.Width / 2 - 60,
                BackColor = Color.FromArgb(80, dir == FlowDirection.LeftToRight ? Color.Red : Color.Blue),
                FlowDirection = dir,
                WrapContents = true,
                AutoScroll = true,
                Margin = new Padding(0),
                Padding = new Padding(UnitDisplayMargin)
            };
            // при ресайзе держим половину ширины
            Resize += (s, e) => pnl.Width = ClientSize.Width / 2 - 60;
            return pnl;
        }

        private Label MakeLabel(string text, Color? color = null) => new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = color ?? Color.White
        };

        private Button MakeButton(string text, EventHandler click)
        {
            var btn = new Button
            {
                Text = text,
                Width = 160,
                Height = 40,
                Font = new Font(Font, FontStyle.Bold),
                Margin = new Padding(30, 0, 30, 0)
            };
            btn.Click += click;
            return btn;
        }

        // ─────────────────── GAME CALLBACKS ───────────────────
        private void OnGameStateChanged(object sender, GameStateChangedEventArgs e)
            => (InvokeRequired ? (Action)(() => ApplyState(e)) : () => ApplyState(e))();

        private void ApplyState(GameStateChangedEventArgs e)
        {
            _roundLabel.Text = $"Раунд: {_gameManager.Round}";
            _turnLabel.Text = $"Ход: {_gameManager.CurrentAttacker?.TeamName ?? "-"}";

            if (e.CurrentState == GameState.WaitingForPlayer)
            {
                _statusLabel.Text = "Ваш ход"; _statusLabel.ForeColor = Color.LimeGreen;
            }
            else if (e.CurrentState == GameState.TurnInProgress)
            {
                _statusLabel.Text = "Обработка…"; _statusLabel.ForeColor = Color.Orange;
            }
            else // GameOver
            {
                _statusLabel.Text = "Финал"; _statusLabel.ForeColor = Color.Red;
                _nextBtn.Enabled = _undoBtn.Enabled = false; _exitBtn.Text = "Закрыть";
            }

            _nextBtn.Enabled = e.CurrentState == GameState.WaitingForPlayer;
            _undoBtn.Enabled = _gameManager.CommandManager.CanUndo && e.CurrentState == GameState.WaitingForPlayer;

       

            DrawTeam(_bluePanel, e.BlueTeamSnapshot,  /*flip=*/false); // синие слева – лица вправо
            DrawTeam(_redPanel, e.RedTeamSnapshot,   /*flip=*/true);

            _logBox.Lines = e.LogMessages.ToArray();
            _logBox.SelectionStart = _logBox.Text.Length;
            _logBox.ScrollToCaret();
        }

        private void DrawTeam(FlowLayoutPanel panel, Team team, bool flip)
        {
            panel.SuspendLayout();
            panel.Controls.Clear();
            foreach (var unit in team.GetLivingFighters())
            {
                var cont = new Panel { Width = UnitDisplaySize, Height = UnitDisplaySize + 34, Margin = new Padding(UnitDisplayMargin) };
                var pic = new PictureBox { Width = UnitDisplaySize, Height = UnitDisplaySize, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
                pic.Image = SafeImageLoad(unit.IconPath, flip);
                var lbl = new Label
                {
                    Dock = DockStyle.Bottom,
                    Height = 34,
                    Text = $"{unit.Name}\nHP: {unit.Health:F0}/{unit.MaxHealth:F0}",
                    Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                    TextAlign = ContentAlignment.TopCenter,
                    BackColor = Color.FromArgb(180, 255, 255, 255)
                };
                cont.Controls.Add(pic); cont.Controls.Add(lbl); panel.Controls.Add(cont);
            }
            panel.ResumeLayout();
        }

        // ─────────────────── BUTTONS ───────────────────
        private void NextTurn(object s, EventArgs e) => _gameManager.RequestNextTurn();
        private void UndoTurn(object s, EventArgs e) => _gameManager.RequestUndoTurn();

        // ─────────────────── UTILS ───────────────────
        private static Image SafeImageLoad(string path, bool flipX = false)
        {
            try
            {
                var img = Image.FromFile(path);
                if (flipX) img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                return img;
            }
            catch { return new Bitmap(UnitDisplaySize, UnitDisplaySize); } // fallback
        }
    }
}
