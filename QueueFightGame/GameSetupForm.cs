using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq; // For random team generation
using System.Collections.Generic; // For List

namespace QueueFightGame.UI
{
    public partial class GameSetupForm : Form
    {
        private Button randomButton;
        private Button purchaseButton;
        private Button backButton;
        private ILogger uiLogger; // For potential logging during setup

        private const string BgPath = "Resources/zastavka.png"; // ваш PNG-фон
        private readonly Color BtnBack = Color.FromArgb(40, 40, 40);   // тёмный графит
        private readonly Color BtnBorder = Color.FromArgb(80, 0, 0);     // бордовый кант
        private readonly Color BtnHover = Color.FromArgb(70, 70, 70);

        public GameSetupForm()
        {
            InitializeComponent();
            SetupCustomComponents();
            // Use MemoryLogger even here if setup actions need logging visible later
            uiLogger = new MemoryLogger();
        }

        private void SetupCustomComponents()
        {
            Text = "Настройка Игры";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(400, 300);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            // ── фон
            try
            {
                BackgroundImage = Image.FromFile(BgPath);
                BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { /* если файла нет – просто остаётся сплошной цвет */ }

            // ── создаём кнопки
            randomButton = MakeDungeonButton("Случайный Бой", 140, 50);
            purchaseButton = MakeDungeonButton("Собрать Команды", 140, 110);
            backButton = MakeDungeonButton("Назад", 140, 170);

            randomButton.Click += RandomButton_Click;
            purchaseButton.Click += PurchaseButton_Click;
            backButton.Click += BackButton_Click;

            Controls.AddRange(new Control[] { randomButton, purchaseButton, backButton });
        }

        // ── фабричный метод для «подземельной» кнопки
        private Button MakeDungeonButton(string text, int x, int y)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.Gainsboro,
                BackColor = BtnBack,
                FlatStyle = FlatStyle.Flat
            };

            // оформляем рамку и эффекты
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = BtnBorder;
            btn.FlatAppearance.MouseDownBackColor = BtnBorder;
            btn.FlatAppearance.MouseOverBackColor = BtnHover;

            return btn;
        }

        private void RandomButton_Click(object sender, EventArgs e)
        {
            float budget = 100; // Default budget for random teams
            Team redTeam = GenerateRandomTeam("Красные", budget);
            Team blueTeam = GenerateRandomTeam("Синие", budget);

            if (redTeam == null || blueTeam == null || !redTeam.HasFighters() || !blueTeam.HasFighters())
            {
                MessageBox.Show("Не удалось сгенерировать случайные команды. Возможно, недостаточно бюджета или нет доступных юнитов.", "Ошибка генерации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            StartBattle(redTeam, blueTeam);
        }

        private Team GenerateRandomTeam(string name, float budget)
        {
            Team team = new Team(name, budget);
            var availableUnits = UnitConfig.Stats.Keys.ToList();
            Random random = new Random();
            int attempts = 0; // Prevent infinite loop if budget too low

            // Simple random generation: pick random units until budget runs out
            while (team.CurrentMoney > 0 && availableUnits.Count > 0 && attempts < 50)
            {
                string randomUnitType = availableUnits[random.Next(availableUnits.Count)];
                UnitConfig.UnitData unitData = UnitConfig.Stats[randomUnitType];

                if (team.CanAfford(unitData.Cost))
                {
                    IUnit unit = UnitFactory.CreateUnit(randomUnitType);
                    team.AddFighter(unit, null); // No logger needed for random generation display
                }
                else
                {
                    // Remove unit type if too expensive to avoid repeatedly trying it
                    availableUnits.Remove(randomUnitType);
                }
                attempts++;
            }

            // Ensure team is not empty if possible (add cheapest unit if empty and affordable)
            if (!team.Fighters.Any())
            {
                var cheapestUnit = UnitConfig.Stats.OrderBy(kv => kv.Value.Cost).FirstOrDefault();
                if (cheapestUnit.Value != null && team.CanAfford(cheapestUnit.Value.Cost))
                {
                    IUnit unit = UnitFactory.CreateUnit(cheapestUnit.Key);
                    team.AddFighter(unit, null);
                }
            }


            return team;
        }

        private void PurchaseButton_Click(object sender, EventArgs e)
        {
            // Open the purchase form
            // Pass budget, maybe team names
            float budget = 100; // Or get from settings
            TeamPurchaseForm purchaseForm = new TeamPurchaseForm(budget);
            purchaseForm.Show();
            this.Hide(); // Hide setup form

            purchaseForm.FormClosed += (s, args) => this.Close(); // Close setup when purchase is done/closed
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            // Need a way to show the main menu again if it was hidden
            // This requires passing a reference or using a singleton/static approach (less ideal)
            // Simple way: just close this form. The MainMenuForm's FormClosed handler should exit app.
            // Better way: MainMenuForm listens for GameSetupForm closing and shows itself.
            this.Close();
            // Find and show MainMenuForm if it exists? Risky.
            // Let's assume closing this form triggers closure of the hidden MainMenuForm via the event handler set there.
        }

        private void StartBattle(Team redTeam, Team blueTeam)
        {
            BattleForm battleForm = new BattleForm(redTeam, blueTeam);
            battleForm.Show();
            this.Hide(); // Hide this form

            battleForm.FormClosed += (s, args) => this.Close(); // Ensure app closes when battle ends
        }

        // Required by designer
        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // GameSetupForm
            //
            this.Name = "GameSetupForm";
            this.ResumeLayout(false);
        }
    }
}