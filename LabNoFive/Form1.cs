using System.Numerics;
using LabNoFive.Objects;
namespace LabNoFive
{
    public partial class Form1 : Form
    {
        List<BaseObject> objects = new();
        Player player;
        Marker? marker;
        Random rand = new Random();
        DarkArea darkArea;
        List<Circle> circles = new List<Circle>();
        int score = 0;

        public Form1()
        {
            InitializeComponent();

            player = new Player(pbMain.Width / 2, pbMain.Height / 2, 0);

            player.OnOverlap += (p, obj) =>
            {
                txtLog.Text = $"[{DateTime.Now:HH:mm:ss:ff}] Игрок пересекся с {obj}\n" + txtLog.Text;
            };

            player.OnMarkerOverlap += (m) =>
            {
                objects.Remove(m);
                marker = null;
            };
            darkArea = new DarkArea(-75, pbMain.Height / 2, 0);

            darkArea.OnObjectEntered += (obj) =>
            {
                if (obj is Circle circle)
                {
                    circle.Color = Color.White;
                }
                else if (obj is Player player)
                {
                    player.Color = Color.White;
                }
                else if (obj is Marker marker)
                {
                    marker.Color = Color.White;
                }
            };

            darkArea.OnObjectExited += (obj) =>
            {
                if (obj is Circle circle)
                {
                    circle.Color = Color.LimeGreen;
                }
                else if (obj is Player player)
                {
                    player.Color = Color.DeepSkyBlue;
                }
                else if (obj is Marker marker)
                {
                    marker.Color = Color.Red;
                }
            };

            objects.Add(darkArea);
            marker = new Marker(pbMain.Width / 2 + 50, pbMain.Height / 2 + 50, 0);

            objects.Add(player);
            objects.Add(marker);

            for (int i = 0; i < 2; i++)
            {
                var circle = new Circle(0, 0, 0);
                circle.Respawn(rand, pbMain.Width, pbMain.Height);
                circle.OnOverlap += (c, obj) =>
                {
                    if (obj is Player)
                    {
                        ((Circle)c).Respawn(rand, pbMain.Width, pbMain.Height);
                        score++;
                        UpdateScoreDisplay();
                    }
                };

                circle.OnCountdownEnded += (c) =>
                {
                    c.Respawn(rand, pbMain.Width, pbMain.Height);
                };

                objects.Add(circle);
                circles.Add(circle);
            }
        }

        private void UpdateScoreDisplay()
        {
            labelScore.Text = $"Очки: {score}";
        }

        private void pbMain_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            updatePlayer();
            darkArea.Move(pbMain.Width);
            darkArea.CheckOverlaps(objects, g);

            foreach (var obj in objects.ToList())
            {
                if (obj != player && player.Overlaps(obj, g))
                {
                    player.Overlap(obj);
                    obj.Overlap(player);
                }
            }

            foreach (var obj in objects)
            {
                g.Transform = obj.GetTransform();
                obj.Render(g);
            }
        }

        private void updatePlayer()
        {
            if (marker != null)
            {
                float dx = marker.X - player.X;
                float dy = marker.Y - player.Y;
                float length = MathF.Sqrt(dx * dx + dy * dy);
                dx /= length;
                dy /= length;

                player.vX += dx * 0.5f;
                player.vY += dy * 0.5f;

                player.Angle = 90 - MathF.Atan2(player.vX, player.vY) * 180 / MathF.PI;
            }

            player.vX += -player.vX * 0.1f;
            player.vY += -player.vY * 0.1f;

            player.X += player.vX;
            player.Y += player.vY;
        }

        private void pbMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (marker == null)
            {
                marker = new Marker(0, 0, 0);
                objects.Add(marker);
            }
            marker.X = e.X;
            marker.Y = e.Y;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var circle in circles)
            {
                circle.UpdateCountdown();
            }

            pbMain.Invalidate();
        }

    }
}
