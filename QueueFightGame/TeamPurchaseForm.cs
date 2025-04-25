using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QueueFightGame.UI
{
    public partial class TeamPurchaseForm : Form
    {
        private readonly float _initialBudget;
        private Team _redTeam;
        private Team _blueTeam;
        private Team _currentlyEditingTeam;

        // UI Elements (Declare fields)
        private ListBox availableUnitsListBox;
        private ListBox redTeamListBox;
        private ListBox blueTeamListBox;
        private Label availableUnitsLabel;
        private Label redTeamLabel;
        private Label blueTeamLabel;
        private Label redMoneyLabel;
        private Label blueMoneyLabel;
        private Label unitInfoLabel; // To display stats of selected unit
        private PictureBox unitPictureBox; // To display icon
        private Button addUnitButton;
        private Button removeUnitButton;
        private Button switchTeamButton; // Button to switch between editing Red and Blue
        private Button startBattleButton;
        private Button backButton; // Go back to setup menu
        private const int HeroPreviewSize = 128;

        public TeamPurchaseForm(float budget)
        {
            _initialBudget = budget;
            _redTeam = new Team("Красные", _initialBudget);
            _blueTeam = new Team("Синие", _initialBudget);
            _currentlyEditingTeam = _redTeam; // Start editing Red team

            InitializeComponent();
            SetupCustomComponents();
            LoadAvailableUnits();
            UpdateTeamDisplay();
        }

        private void SetupCustomComponents()
        {

            this.Text = "Сбор Команд";
            this.ClientSize = new Size(1024, 624); // Adjust size as needed
            this.StartPosition = FormStartPosition.CenterScreen;

            this.BackgroundImage = Image.FromFile("Resources/zastavka.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Available Units Section
            availableUnitsLabel = new Label { Text = "Доступные Бойцы:", Location = new Point(10, 10), AutoSize = true };
            availableUnitsListBox = new ListBox { Location = new Point(10, 30), Size = new Size(200, 200) };
            availableUnitsListBox.SelectedIndexChanged += AvailableUnitsListBox_SelectedIndexChanged;

            // Unit Info Section
            unitPictureBox = new PictureBox { 
                Location = new Point(10, 240),
                Size = new Size(64, 64), 
                BorderStyle = BorderStyle.FixedSingle, 
                SizeMode = PictureBoxSizeMode.Zoom 
            };

           


            unitInfoLabel = new Label { Location = new Point(80, 240), Size = new Size(130, 100), BorderStyle = BorderStyle.Fixed3D };

            // Red Team Section
            redTeamLabel = new Label { Text = "Команда Красных:", Location = new Point(250, 10), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            redMoneyLabel = new Label { Text = $"Бюджет: {_redTeam.CurrentMoney:F0}", Location = new Point(400, 10), AutoSize = true };
            redTeamListBox = new ListBox { Location = new Point(250, 30), Size = new Size(200, 300) };

            // Blue Team Section
            blueTeamLabel = new Label { Text = "Команда Синих:", Location = new Point(480, 10), AutoSize = true };
            blueMoneyLabel = new Label { Text = $"Бюджет: {_blueTeam.CurrentMoney:F0}", Location = new Point(630, 10), AutoSize = true };
            blueTeamListBox = new ListBox { Location = new Point(480, 30), Size = new Size(200, 300) };

            // Buttons Section
            addUnitButton = new Button { Text = "Добавить ->", Location = new Point(80, 350), Size = new Size(100, 30) };
            removeUnitButton = new Button { Text = "<- Убрать", Location = new Point(80, 390), Size = new Size(100, 30) };
            switchTeamButton = new Button { Text = "Ред. Синюю", Location = new Point(300, 350), Size = new Size(130, 30) }; // Initial text assumes Red is active
            startBattleButton = new Button { Text = "Начать Битву", Location = new Point(550, 350), Size = new Size(130, 30), Font = new Font(this.Font, FontStyle.Bold), BackColor = Color.LightGreen };
            backButton = new Button { Text = "Назад", Location = new Point(550, 390), Size = new Size(130, 30), BackColor = Color.LightCoral };


            addUnitButton.Click += AddUnitButton_Click;
            removeUnitButton.Click += RemoveUnitButton_Click;
            switchTeamButton.Click += SwitchTeamButton_Click;
            startBattleButton.Click += StartBattleButton_Click;
            backButton.Click += BackButton_Click;


            // Add Controls
            this.Controls.Add(availableUnitsLabel);
            this.Controls.Add(availableUnitsListBox);
            this.Controls.Add(unitPictureBox);
            this.Controls.Add(unitInfoLabel);
            this.Controls.Add(redTeamLabel);
            this.Controls.Add(redMoneyLabel);
            this.Controls.Add(redTeamListBox);
            this.Controls.Add(blueTeamLabel);
            this.Controls.Add(blueMoneyLabel);
            this.Controls.Add(blueTeamListBox);
            this.Controls.Add(addUnitButton);
            this.Controls.Add(removeUnitButton);
            this.Controls.Add(switchTeamButton);
            this.Controls.Add(startBattleButton);
            this.Controls.Add(backButton);

            UpdateEditingTeamHighlight();
        }

        private void LoadAvailableUnits()
        {
            availableUnitsListBox.Items.Clear();
            foreach (var unitData in UnitConfig.Stats.Values.OrderBy(u => u.Cost))
            {
                // Store the type name in the item's Tag for later use
                var item = new ListBoxItem { Text = $"{unitData.DisplayName} ({unitData.Cost})", Value = unitData.TypeName };
                availableUnitsListBox.Items.Add(item);
            }
        }

        private void AvailableUnitsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (availableUnitsListBox.SelectedItem is ListBoxItem selectedItem)
            {
                // Value здесь - это string (TypeName)
                string typeName = selectedItem.Value as string; // Используем 'as' для безопасного приведения
                if (!string.IsNullOrEmpty(typeName) && UnitConfig.Stats.TryGetValue(typeName, out var data))
                {
                    unitInfoLabel.Text = $"Имя: {data.DisplayName}\nHP: {data.Health}\nЗащ: {data.Protection:P0}\nУрон: {data.Damage}\nСтоим: {data.Cost}\n----------\n{data.Description}"; // Форматируем защиту как %
                    try { unitPictureBox.Image = Image.FromFile(data.IconPath); }
                    catch { unitPictureBox.Image = null; }
                }
                else // Если приведение к string не удалось или нет данных в конфиге
                {
                    unitInfoLabel.Text = "";
                    unitPictureBox.Image = null;
                }
            }
            else
            {
                unitInfoLabel.Text = "";
                unitPictureBox.Image = null;
            }
        }

        private void SwitchTeamButton_Click(object sender, EventArgs e)
        {
            if (_currentlyEditingTeam == _redTeam)
            {
                _currentlyEditingTeam = _blueTeam;
                switchTeamButton.Text = "Ред. Красную";
            }
            else
            {
                _currentlyEditingTeam = _redTeam;
                switchTeamButton.Text = "Ред. Синюю";
            }
            UpdateEditingTeamHighlight();
        }

        private void UpdateEditingTeamHighlight()
        {
            if (_currentlyEditingTeam == _redTeam)
            {
                redTeamLabel.Font = new Font(this.Font, FontStyle.Bold);
                redTeamListBox.BackColor = SystemColors.Info; // Highlight red list
                blueTeamLabel.Font = new Font(this.Font, FontStyle.Regular);
                blueTeamListBox.BackColor = SystemColors.Window;
            }
            else
            {
                redTeamLabel.Font = new Font(this.Font, FontStyle.Regular);
                redTeamListBox.BackColor = SystemColors.Window;
                blueTeamLabel.Font = new Font(this.Font, FontStyle.Bold);
                blueTeamListBox.BackColor = SystemColors.Info; // Highlight blue list
            }
        }

        private void AddUnitButton_Click(object sender, EventArgs e)
        {
            if (availableUnitsListBox.SelectedItem is ListBoxItem selectedItem)
            {
                // Value здесь - это string (TypeName)
                string typeName = selectedItem.Value as string;
                if (!string.IsNullOrEmpty(typeName))
                {
                    UnitConfig.UnitData unitData = UnitFactory.GetUnitData(typeName);
                    if (_currentlyEditingTeam.CanAfford(typeName))
                    {
                        IUnit newUnit = UnitFactory.CreateUnit(typeName);
                        _currentlyEditingTeam.AddFighter(newUnit, null);
                        UpdateTeamDisplay();
                    }
                    else { MessageBox.Show($"Недостаточно денег!"); }
                }
                else { /* Логика если Value не string - не должно произойти */ }
            }
        }

        private void RemoveUnitButton_Click(object sender, EventArgs e)
        {
            ListBox activeListBox = (_currentlyEditingTeam == _redTeam) ? redTeamListBox : blueTeamListBox;

            if (activeListBox.SelectedItem is ListBoxItem selectedTeamItem)
            {
                // Find the unit in the actual team list based on the Tag (which holds the IUnit instance)
                IUnit unitToRemove = selectedTeamItem.Value as IUnit;
                if (unitToRemove != null)
                {
                    _currentlyEditingTeam.RemoveFighter(unitToRemove, null, true); // Refund money
                    UpdateTeamDisplay();
                }
            }
        }

        private void UpdateTeamDisplay()
        {
            redTeamListBox.Items.Clear();
            foreach (var unit in _redTeam.Fighters)
            {
                // Store the actual unit instance in the Tag for removal later
                var item = new ListBoxItem { Text = unit.Name, Value = unit };
                redTeamListBox.Items.Add(item);
            }
            redMoneyLabel.Text = $"Бюджет: {_redTeam.CurrentMoney:F0}";

            blueTeamListBox.Items.Clear();
            foreach (var unit in _blueTeam.Fighters)
            {
                var item = new ListBoxItem { Text = unit.Name, Value = unit };
                blueTeamListBox.Items.Add(item);
            }
            blueMoneyLabel.Text = $"Бюджет: {_blueTeam.CurrentMoney:F0}";

            // Enable Start button only if both teams have at least one fighter
            startBattleButton.Enabled = _redTeam.Fighters.Any() && _blueTeam.Fighters.Any();
        }

        private void StartBattleButton_Click(object sender, EventArgs e)
        {
            if (!_redTeam.Fighters.Any() || !_blueTeam.Fighters.Any())
            {
                MessageBox.Show("Обе команды должны иметь хотя бы одного бойца!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Start the battle
            BattleForm battleForm = new BattleForm(_redTeam, _blueTeam);
            battleForm.Show();
            this.Hide();

            battleForm.FormClosed += (s, args) => this.Close();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
            // Пытаемся найти и показать предыдущую форму
            var setupForm = System.Windows.Forms.Application.OpenForms.OfType<GameSetupForm>().FirstOrDefault();
            setupForm?.Show();
        }


        // Required by designer
        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // TeamPurchaseForm
            //
            this.Name = "TeamPurchaseForm";
            this.ResumeLayout(false);

        }

        // Helper class for ListBox items storing values
        private class ListBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; } // Can store TypeName string or IUnit instance

            public override string ToString()
            {
                return Text;
            }
        }
    }
}