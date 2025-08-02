using System;
using System.Drawing;
using System.Windows.Forms;

namespace Geography_game
{
    public partial class MainForm : Form
    {
        private QuizManager quizManager;

        public MainForm()
        {
            quizManager = new QuizManager();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Use Montserrat, fallback to Segoe UI
            FontFamily fontFamily;
            try { fontFamily = new FontFamily("Montserrat"); }
            catch { fontFamily = new FontFamily("Segoe UI"); }
            var mainFont = new Font(fontFamily, 14, FontStyle.Bold);

            btnCreateMode = new Button();
            btnPlayMode = new Button();
            SuspendLayout();
            // 
            // btnCreateMode
            // 
            btnCreateMode.Location = new Point(100, 100);
            btnCreateMode.Name = "btnCreateMode";
            btnCreateMode.Size = new Size(200, 60);
            btnCreateMode.TabIndex = 0;
            btnCreateMode.Text = "Create Mode";
            btnCreateMode.Font = mainFont;
            btnCreateMode.Click += BtnCreateMode_Click;
            // 
            // btnPlayMode
            // 
            btnPlayMode.Location = new Point(100, 200);
            btnPlayMode.Name = "btnPlayMode";
            btnPlayMode.Size = new Size(200, 60);
            btnPlayMode.TabIndex = 1;
            btnPlayMode.Text = "Play Mode";
            btnPlayMode.Font = mainFont;
            btnPlayMode.Click += BtnPlayMode_Click;
            // 
            // MainForm
            // 
            BackColor = Color.FromArgb(60, 0, 120);
            ClientSize = new Size(400, 400);
            Controls.Add(btnCreateMode);
            Controls.Add(btnPlayMode);
            Font = mainFont;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Geography Quiz Game";
            ResumeLayout(false);
        }

        private void BtnCreateMode_Click(object sender, EventArgs e)
        {
            using (var createForm = new CreateModeForm(quizManager))
            {
                createForm.ShowDialog();
            }
        }

        private void BtnPlayMode_Click(object sender, EventArgs e)
        {
            if (quizManager.Questions.Count == 0)
            {
                MessageBox.Show("No questions available. Please add questions in Create Mode.", "No Questions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var playForm = new PlayModeForm(quizManager))
            {
                playForm.ShowDialog();
            }
        }
        private Button btnCreateMode;
        private Button btnPlayMode;
    }
}