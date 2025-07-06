using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LabNoFive.Objects
{
    class Circle : BaseObject
    {
        public double Countdown { get; private set; } = 100;
        public event Action<Circle>? OnCountdownEnded;

        public Circle(float x, float y, float angle) : base(x, y, angle)
        {
            Color = Color.LimeGreen;
        }

        public override void Render(Graphics g)
        {
            // Используем свойство Color для заливки
            g.FillEllipse(new SolidBrush(Color), -15, -15, 30, 30);

            // Текст счетчика рисуем черным для лучшей читаемости
            var font = new Font("Verdana", 8);
            var brush = new SolidBrush(Color); // Изменено на черный цвет
            g.DrawString(
                Countdown.ToString(), // Форматируем без дробной части
                font,
                brush,
                10, 10
            );
        }

        public override GraphicsPath GetGraphicsPath()
        {
            var path = base.GetGraphicsPath();
            path.AddEllipse(-15, -15, 30, 30);
            return path;
        }

        public void Respawn(Random rand, int width, int height)
        {
            X = rand.Next(30, width - 30);
            Y = rand.Next(30, height - 30);
            Countdown = 100;
        }

        public void UpdateCountdown()
        {
            --Countdown;
            if (Countdown <= 0)
            {
                OnCountdownEnded?.Invoke(this);
            }
        }
    }
}