using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label; // Explicitly alias 'Label' to avoid ambiguity

namespace Geography_game
{
    public partial class CreateModeForm : Form
    {
        private QuizManager quizManager;
        private ComboBox cbQuestionType;
        private TextBox txtQuestionText;
        private TextBox txtAnswer;
        private ListBox lbAnswers;
        private ListBox lbQuestions;
        private Button btnAddAnswer;
        private Button btnAddQuestion;
        private Button btnEditQuestion;
        private Button btnDeleteQuestion;
        private Button btnSaveExit;
        private List<string> answers;
        private PictureBox pbQuestionImage;
        private string? imagePath; // FIX: Make nullable

        public CreateModeForm(QuizManager quizManager)
        {
            this.quizManager = quizManager;
            answers = new List<string>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Use Montserrat, fallback to Segoe UI
            FontFamily fontFamily;
            try { fontFamily = new FontFamily("Montserrat"); }
            catch { fontFamily = new FontFamily("Segoe UI"); }
            var mainFont = new Font(fontFamily, 16, FontStyle.Bold);
            var titleFont = new Font(fontFamily, 22, FontStyle.Bold);
            var labelFont = new Font(fontFamily, 14, FontStyle.Bold);

            Text = "Kahoot-style Geography Quiz - Create Mode";
            Size = new Size(950, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(60, 0, 120);
            Font = mainFont;

            // Title
            Label lblTitle = new Label
            {
                Text = "Geography Quiz - Create Mode",
                Location = new Point(0, 15),
                Size = new Size(950, 40),
                Font = titleFont,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White
            };

            // Left panel for question input
            Panel leftPanel = new Panel
            {
                Location = new Point(30, 70),
                Size = new Size(540, 350),
                BackColor = Color.FromArgb(80, 0, 80, 120)
            };

            Label lblQuestionType = new Label { Text = "Question Type:", Location = new Point(20, 20), Size = new Size(130, 28), ForeColor = Color.White, Font = labelFont };
            cbQuestionType = new ComboBox
            {
                Location = new Point(160, 20),
                Size = new Size(200, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font(fontFamily, 11)
            };
            cbQuestionType.Items.AddRange(new[] { "Multiple Choice", "Open Ended", "True/False" });
            cbQuestionType.SelectedIndex = 0;
            cbQuestionType.SelectedIndexChanged += CbQuestionType_SelectedIndexChanged;

            Label lblQuestionText = new Label { Text = "Question:", Location = new Point(20, 65), Size = new Size(130, 28), ForeColor = Color.White, Font = labelFont };
            txtQuestionText = new TextBox { Location = new Point(160, 65), Size = new Size(340, 28), Font = new Font(fontFamily, 11) };

            Label lblAnswer = new Label { Text = "Answer:", Location = new Point(20, 110), Size = new Size(130, 28), ForeColor = Color.White, Font = labelFont };
            txtAnswer = new TextBox { Location = new Point(160, 110), Size = new Size(200, 28), Font = new Font(fontFamily, 11) };
            btnAddAnswer = new Button
            {
                Text = "Add Answer",
                Location = new Point(370, 110),
                Size = new Size(130, 28),
                BackColor = Color.LightSteelBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(fontFamily, 10, FontStyle.Bold)
            };
            btnAddAnswer.Click += BtnAddAnswer_Click;

            lbAnswers = new ListBox { Location = new Point(160, 150), Size = new Size(340, 80), Font = new Font(fontFamily, 11) };

            // Image section
            pbQuestionImage = new PictureBox
            {
                Location = new Point(160, 240),
                Size = new Size(120, 80),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            Button btnLoadImage = new Button
            {
                Text = "Add Image",
                Location = new Point(290, 240),
                Size = new Size(100, 28),
                BackColor = Color.LightSteelBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(fontFamily, 10, FontStyle.Bold)
            };
            btnLoadImage.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        pbQuestionImage.Image = Image.FromFile(ofd.FileName);
                        imagePath = ofd.FileName;
                    }
                }
            };

            leftPanel.Controls.AddRange(new Control[] {
                lblQuestionType, cbQuestionType, lblQuestionText, txtQuestionText,
                lblAnswer, txtAnswer, btnAddAnswer, lbAnswers, pbQuestionImage, btnLoadImage
            });

            // Right panel for question list and actions
            Panel rightPanel = new Panel
            {
                Location = new Point(600, 70),
                Size = new Size(320, 350),
                BackColor = Color.FromArgb(80, 0, 80, 120)
            };

            Label lblQuestions = new Label
            {
                Text = "Questions List:",
                Location = new Point(10, 10),
                Size = new Size(300, 28),
                ForeColor = Color.White,
                Font = labelFont
            };
            lbQuestions = new ListBox
            {
                Location = new Point(10, 45),
                Size = new Size(300, 200),
                Font = new Font(fontFamily, 11)
            };
            lbQuestions.SelectedIndexChanged += LbQuestions_SelectedIndexChanged;

            btnAddQuestion = new Button
            {
                Text = "Add Question",
                Location = new Point(10, 260),
                Size = new Size(90, 35),
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                Font = new Font(fontFamily, 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnAddQuestion.Click += BtnAddQuestion_Click;

            btnEditQuestion = new Button
            {
                Text = "Edit",
                Location = new Point(110, 260),
                Size = new Size(90, 35),
                BackColor = Color.CornflowerBlue,
                ForeColor = Color.White,
                Font = new Font(fontFamily, 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnEditQuestion.Click += BtnEditQuestion_Click;

            btnDeleteQuestion = new Button
            {
                Text = "Delete",
                Location = new Point(210, 260),
                Size = new Size(90, 35),
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                Font = new Font(fontFamily, 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnDeleteQuestion.Click += BtnDeleteQuestion_Click;

            btnSaveExit = new Button
            {
                Text = "Save & Exit",
                Location = new Point(90, 310),
                Size = new Size(140, 35),
                BackColor = Color.DarkSlateBlue,
                ForeColor = Color.White,
                Font = new Font(fontFamily, 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnSaveExit.Click += BtnSaveExit_Click;

            rightPanel.Controls.AddRange(new Control[] {
                lblQuestions, lbQuestions, btnAddQuestion, btnEditQuestion, btnDeleteQuestion, btnSaveExit
            });

            Controls.AddRange(new Control[] { lblTitle, leftPanel, rightPanel });
            UpdateQuestionList();
        }

        private void CbQuestionType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            answers.Clear();
            lbAnswers.Items.Clear();
            txtAnswer.Text = "";
            bool isTrueFalse = cbQuestionType.SelectedItem?.ToString() == "True/False";
            txtAnswer.Enabled = !isTrueFalse;
            btnAddAnswer.Enabled = !isTrueFalse;
            lbAnswers.Enabled = true;
            lbAnswers.Items.Clear();
            if (isTrueFalse)
            {
                lbAnswers.Items.AddRange(new[] { "True", "False" });
            }
        }

        private void BtnAddAnswer_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                MessageBox.Show("Please enter an answer.");
                return;
            }
            if (cbQuestionType.SelectedItem?.ToString() == "Multiple Choice" && answers.Count >= 4)
            {
                MessageBox.Show("Multiple-choice questions can have only 4 options.");
                return;
            }
            answers.Add(txtAnswer.Text);
            lbAnswers.Items.Add(txtAnswer.Text);
            txtAnswer.Text = "";
        }

        private void BtnAddQuestion_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuestionText.Text))
            {
                MessageBox.Show("Please enter a question.");
                return;
            }
            try
            {
                Question question = CreateQuestion();
                quizManager.AddQuestion(question);
                UpdateQuestionList();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnEditQuestion_Click(object? sender, EventArgs e)
        {
            if (lbQuestions.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a question to edit.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtQuestionText.Text))
            {
                MessageBox.Show("Please enter a question.");
                return;
            }
            try
            {
                Question question = CreateQuestion();
                quizManager.EditQuestion(lbQuestions.SelectedIndex, question);
                UpdateQuestionList();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnDeleteQuestion_Click(object? sender, EventArgs e)
        {
            if (lbQuestions.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a question to delete.");
                return;
            }
            quizManager.DeleteQuestion(lbQuestions.SelectedIndex);
            UpdateQuestionList();
            ClearInputs();
        }

        private void BtnSaveExit_Click(object? sender, EventArgs e)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GeographyQuizQuestions.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(filePath, JsonSerializer.Serialize(quizManager.Questions, options));
                MessageBox.Show($"Questions saved to {filePath}", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving questions: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LbQuestions_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lbQuestions.SelectedIndex == -1) return;
            Question selected = quizManager.Questions[lbQuestions.SelectedIndex];
            txtQuestionText.Text = selected.Text;
            answers.Clear();
            lbAnswers.Items.Clear();
            imagePath = selected.ImagePath;
            pbQuestionImage.Image = null;
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                pbQuestionImage.Image = Image.FromFile(imagePath);
            }
            if (selected is MultipleChoiceQuestion mc)
            {
                cbQuestionType.SelectedItem = "Multiple Choice";
                answers.AddRange(mc.Options);
                lbAnswers.Items.AddRange(mc.Options.ToArray());
            }
            else if (selected is OpenEndedQuestion oe)
            {
                cbQuestionType.SelectedItem = "Open Ended";
                answers.AddRange(oe.CorrectAnswers);
                lbAnswers.Items.AddRange(oe.CorrectAnswers.ToArray());
            }
            else if (selected is TrueFalseQuestion)
            {
                cbQuestionType.SelectedItem = "True/False";
                lbAnswers.Items.AddRange(new[] { "True", "False" });
            }
        }

        private Question CreateQuestion()
        {
            string questionType = cbQuestionType.SelectedItem?.ToString() ?? "";
            if (questionType == "Multiple Choice")
            {
                if (answers.Count != 4)
                    throw new Exception("Multiple-choice questions must have exactly 4 options.");
                if (lbAnswers.SelectedIndex == -1)
                    throw new Exception("Please select the correct answer in the list.");
                return new MultipleChoiceQuestion(txtQuestionText.Text, "Geography", new List<string>(answers), answers[lbAnswers.SelectedIndex], imagePath);
            }
            else if (questionType == "Open Ended")
            {
                if (answers.Count == 0)
                    throw new Exception("Open-ended questions must have at least one answer.");
                return new OpenEndedQuestion(txtQuestionText.Text, "Geography", new List<string>(answers), imagePath);
            }
            else // True/False
            {
                if (lbAnswers.SelectedIndex == -1)
                    throw new Exception("Please select True or False.");
                bool correctAnswer = lbAnswers.SelectedIndex == 0;
                return new TrueFalseQuestion(txtQuestionText.Text, "Geography", correctAnswer, imagePath);
            }
        }

        private void UpdateQuestionList()
        {
            lbQuestions.Items.Clear();
            for (int i = 0; i < quizManager.Questions.Count; i++)
            {
                lbQuestions.Items.Add($"Q{i + 1}: {quizManager.Questions[i].Text}");
            }
        }

        private void ClearInputs()
        {
            txtQuestionText.Text = "";
            txtAnswer.Text = "";
            answers.Clear();
            lbAnswers.Items.Clear();
            cbQuestionType.SelectedIndex = 0;
            pbQuestionImage.Image = null;
            imagePath = null;
        }
    }
}