using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace Geography_game
{
    public enum AnswerShape { Rectangle, Circle, Triangle, Hexagon }

    public class ShapeButton : Button
    {
        public AnswerShape Shape { get; set; } = AnswerShape.Rectangle;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            GraphicsPath path = new GraphicsPath();
            switch (Shape)
            {
                case AnswerShape.Circle:
                    path.AddEllipse(ClientRectangle);
                    break;
                case AnswerShape.Triangle:
                    path.AddPolygon(new[] {
                        new Point(Width / 2, 5),
                        new Point(Width - 5, Height - 5),
                        new Point(5, Height - 5)
                    });
                    break;
                case AnswerShape.Hexagon:
                    float w = Width - 10, h = Height - 10;
                    path.AddPolygon(new[] {
                        new Point((int)(w / 4) + 5, 5),
                        new Point((int)(3 * w / 4) + 5, 5),
                        new Point((int)w + 5, (int)(h / 2) + 5),
                        new Point((int)(3 * w / 4) + 5, (int)h + 5),
                        new Point((int)(w / 4) + 5, (int)h + 5),
                        new Point(5, (int)(h / 2) + 5)
                    });
                    break;
                default:
                    path.AddRectangle(ClientRectangle);
                    break;
            }
            this.Region = new Region(path);
            using (var pen = new Pen(Color.White, 3))
                pevent.Graphics.DrawPath(pen, path);
        }
    }

    public partial class PlayModeForm : Form
    {
        private QuizManager quizManager;
        private int currentQuestionIndex;
        private Label lblQuestion;
        private Label lblFeedback;
        private Button[] answerButtons;
        private TextBox txtOpenAnswer;
        private Button btnSubmit;
        private PictureBox pbQuestionImage;
        private Label lblQuestionCount;
        private int selectedAnswerIndex = -1;

        public PlayModeForm(QuizManager quizManager)
        {
            this.quizManager = quizManager;
            currentQuestionIndex = 0;
            quizManager.StartQuiz();
            InitializeComponent();
            LoadQuestion();
        }

        private void InitializeComponent()
        {
            // Use Montserrat, fallback to Segoe UI
            FontFamily fontFamily;
            try { fontFamily = new FontFamily("Montserrat"); }
            catch { fontFamily = new FontFamily("Segoe UI"); }
            var mainFont = new Font(fontFamily, 16, FontStyle.Bold);
            var titleFont = new Font(fontFamily, 26, FontStyle.Bold);
            var labelFont = new Font(fontFamily, 14, FontStyle.Bold);

            Text = "Kahoot-style Geography Quiz - Play Mode";
            Size = new Size(1100, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(60, 0, 120);
            Font = mainFont;

            Label lblTitle = new Label
            {
                Text = "Geography Quiz - Play Mode",
                Location = new Point(0, 15),
                Size = new Size(1100, 50),
                Font = titleFont,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White
            };

            Panel leftPanel = new Panel
            {
                Location = new Point(40, 80),
                Size = new Size(600, 600),
                BackColor = Color.FromArgb(90, 0, 80, 120)
            };

            lblQuestion = new Label
            {
                Location = new Point(30, 20),
                Size = new Size(540, 70),
                Font = new Font(fontFamily, 20, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };

            pbQuestionImage = new PictureBox
            {
                Location = new Point(30, 100),
                Size = new Size(200, 140),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            answerButtons = new Button[4];
            int btnWidth = 420, btnHeight = 60, btnSpacing = 25, btnLeft = 180, btnTop = 260;
            for (int i = 0; i < 4; i++)
            {
                answerButtons[i] = new Button
                {
                    Text = "",
                    Location = new Point(btnLeft, btnTop + i * (btnHeight + btnSpacing)),
                    Size = new Size(btnWidth, btnHeight),
                    BackColor = i switch
                    {
                        0 => Color.FromArgb(255, 61, 113),
                        1 => Color.FromArgb(0, 180, 216),
                        2 => Color.FromArgb(255, 221, 51),
                        3 => Color.FromArgb(0, 204, 102),
                        _ => Color.Gray
                    },
                    ForeColor = Color.White,
                    Font = new Font(fontFamily, 16, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Visible = false,
                    Tag = i
                };
                answerButtons[i].Paint += AnswerButton_Paint;
                int idx = i;
                answerButtons[i].Click += (s, e) => AnswerButton_Click(idx);
                leftPanel.Controls.Add(answerButtons[i]);
            }

            txtOpenAnswer = new TextBox
            {
                Location = new Point(260, 320),
                Size = new Size(300, 40),
                Font = new Font(fontFamily, 16),
                Visible = false
            };

            btnSubmit = new Button
            {
                Text = "Submit",
                Location = new Point(480, 370),
                Size = new Size(80, 40),
                BackColor = Color.MediumPurple,
                ForeColor = Color.White,
                Font = new Font(fontFamily, 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            btnSubmit.Click += BtnSubmit_Click;

            lblFeedback = new Label
            {
                Location = new Point(30, 420),
                Size = new Size(540, 50),
                ForeColor = Color.Yellow,
                Font = new Font(fontFamily, 15, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            leftPanel.Controls.Add(lblQuestion);
            leftPanel.Controls.Add(pbQuestionImage);
            leftPanel.Controls.Add(txtOpenAnswer);
            leftPanel.Controls.Add(btnSubmit);
            leftPanel.Controls.Add(lblFeedback);

            Panel rightPanel = new Panel
            {
                Location = new Point(670, 80),
                Size = new Size(370, 600),
                BackColor = Color.FromArgb(90, 0, 80, 120)
            };

            lblQuestionCount = new Label
            {
                Text = "Question 1 / 1",
                Location = new Point(20, 30),
                Size = new Size(330, 40),
                ForeColor = Color.White,
                Font = new Font(fontFamily, 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblInstructions = new Label
            {
                Text = "Select or enter your answer below.\nFeedback will appear after each question.",
                Location = new Point(20, 90),
                Size = new Size(330, 60),
                ForeColor = Color.White,
                Font = new Font(fontFamily, 13)
            };

            rightPanel.Controls.Add(lblQuestionCount);
            rightPanel.Controls.Add(lblInstructions);

            Controls.Add(lblTitle);
            Controls.Add(leftPanel);
            Controls.Add(rightPanel);
        }

        private void LoadQuestion()
        {
            // Example implementation: load the current question and update UI
            if (currentQuestionIndex < 0 || currentQuestionIndex >= quizManager.Questions.Count)
                return;

            var question = quizManager.Questions[currentQuestionIndex];
            lblQuestion.Text = question.Text;
            pbQuestionImage.Image = null;
            if (!string.IsNullOrEmpty(question.ImagePath) && File.Exists(question.ImagePath))
                pbQuestionImage.Image = Image.FromFile(question.ImagePath);

            lblFeedback.Text = "";
            lblQuestionCount.Text = $"Question {currentQuestionIndex + 1} / {quizManager.Questions.Count}";

            // Show/hide controls based on question type
            if (question is MultipleChoiceQuestion mcq)
            {
                for (int i = 0; i < answerButtons.Length; i++)
                {
                    answerButtons[i].Visible = true;
                    answerButtons[i].Text = mcq.Options[i];
                    answerButtons[i].BackColor = i switch
                    {
                        0 => Color.FromArgb(255, 61, 113),
                        1 => Color.FromArgb(0, 180, 216),
                        2 => Color.FromArgb(255, 221, 51),
                        3 => Color.FromArgb(0, 204, 102),
                        _ => Color.Gray
                    };
                }
                txtOpenAnswer.Visible = false;
                btnSubmit.Visible = false;
            }
            else if (question is OpenEndedQuestion)
            {
                foreach (var btn in answerButtons) btn.Visible = false;
                txtOpenAnswer.Visible = true;
                btnSubmit.Visible = true;
                txtOpenAnswer.Text = "";
            }
            else if (question is TrueFalseQuestion)
            {
                for (int i = 0; i < answerButtons.Length; i++)
                {
                    answerButtons[i].Visible = i < 2;
                    if (i == 0) answerButtons[i].Text = "True";
                    if (i == 1) answerButtons[i].Text = "False";
                }
                txtOpenAnswer.Visible = false;
                btnSubmit.Visible = false;
            }
        }

        private void ShowNextQuestion()
        {
            currentQuestionIndex++;
            if (currentQuestionIndex < quizManager.Questions.Count)
            {
                LoadQuestion();
            }
            else
            {
                MessageBox.Show($"Quiz complete! Your score: {quizManager.Score}/{quizManager.Questions.Count}", "Quiz Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }

        private void BtnSubmit_Click(object? sender, EventArgs e)
        {
            var question = quizManager.Questions[currentQuestionIndex];
            bool correct = question.CheckAnswer(txtOpenAnswer.Text);
            lblFeedback.Text = correct ? "Correct!" : $"Incorrect! Correct: {question.GetCorrectAnswer()}";
            quizManager.EvaluateAnswer(currentQuestionIndex, txtOpenAnswer.Text);
            Timer timer = new Timer { Interval = 1200 };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                ShowNextQuestion();
            };
            timer.Start();
        }

        private void AnswerButton_Paint(object? sender, PaintEventArgs e)
        {
            // Optional: custom drawing logic for answer buttons
        }

        private void AnswerButton_Click(int idx)
        {
            var question = quizManager.Questions[currentQuestionIndex];
            bool correct = false;
            string userAnswer = "";
            if (question is MultipleChoiceQuestion mcq)
            {
                userAnswer = mcq.Options[idx];
                correct = mcq.CheckAnswer(userAnswer);
            }
            else if (question is TrueFalseQuestion tfq)
            {
                userAnswer = idx == 0 ? "True" : "False";
                correct = tfq.CheckAnswer(userAnswer);
            }
            lblFeedback.Text = correct ? "Correct!" : $"Incorrect! Correct: {question.GetCorrectAnswer()}";
            quizManager.EvaluateAnswer(currentQuestionIndex, userAnswer);
            Timer timer = new Timer { Interval = 1200 };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                ShowNextQuestion();
            };
            timer.Start();
        }
    }
}
