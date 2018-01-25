using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Kolo> Snake = new List<Kolo>();
        private Kolo food = new Kolo();

        public Form1()
        {
            InitializeComponent();

            new Settings();

            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            StartGame();
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;

            new Settings();

            Snake.Clear();
            Kolo head = new Kolo {x = 10, y = 5};
            Snake.Add(head);


            lblScore.Text = Settings.Score.ToString();
            GenerateFood();

        }

        private void GenerateFood()
        {
            int maxXPos = playground.Size.Width / Settings.Width;
            int maxYPos = playground.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Kolo {x = random.Next(0, maxXPos), y = random.Next(0, maxYPos)};
        }


        private void UpdateScreen(object sender, EventArgs e)
        {
            if (Settings.GameOver)
            {
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Kierunek.Lewo)
                    Settings.direction = Kierunek.Prawo;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Kierunek.Prawo)
                    Settings.direction = Kierunek.Lewo;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Kierunek.Dol)
                    Settings.direction = Kierunek.Gora;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Kierunek.Gora)
                    Settings.direction = Kierunek.Dol;

                MovePlayer();
            }

            playground.Invalidate();

        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColour;
                    if (i == 0)
                        snakeColour = Brushes.Black;
                    else
                        snakeColour = Brushes.Green;

                    canvas.FillEllipse(snakeColour,
                        new Rectangle(Snake[i].x * Settings.Width,
                                      Snake[i].y * Settings.Height,
                                      Settings.Width, Settings.Height));


                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.x * Settings.Width,
                             food.y * Settings.Height, Settings.Width, Settings.Height));

                }
            }
            else
            {
                string gameOver = "Game over \nTwój wynik to: " + Settings.Score + "\nNaciśnij Enter aby spróbować ponownie";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
            }
        }


        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Kierunek.Prawo:
                            Snake[i].x++;
                            break;
                        case Kierunek.Lewo:
                            Snake[i].x--;
                            break;
                        case Kierunek.Gora:
                            Snake[i].y--;
                            break;
                        case Kierunek.Dol:
                            Snake[i].y++;
                            break;
                    }


                    int maxXPos = playground.Size.Width / Settings.Width;
                    int maxYPos = playground.Size.Height / Settings.Height;

                    if (Snake[i].x < 0 || Snake[i].y < 0
                        || Snake[i].x >= maxXPos || Snake[i].y >= maxYPos)
                    {
                        Die();
                    }

                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].x == Snake[j].x &&
                           Snake[i].y == Snake[j].y)
                        {
                            Die();
                        }
                    }

                    if (Snake[0].x == food.x && Snake[0].y == food.y)
                    {
                        Eat();
                    }

                }
                else
                {
                    Snake[i].x = Snake[i - 1].x;
                    Snake[i].y = Snake[i - 1].y;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void Eat()
        {
            Kolo circle = new Kolo
            {
                x = Snake[Snake.Count - 1].x,
                y = Snake[Snake.Count - 1].y
            };
            Snake.Add(circle);

            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();

            GenerateFood();
        }

        private void Die()
        {
            Settings.GameOver = true;
        }
    }
}
