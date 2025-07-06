using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace LabNoFive.Objects
{
    class DarkArea(float x, float y, float angle) : BaseObject(x, y, angle)
    {
        public event Action<BaseObject> OnObjectEntered;
        public event Action<BaseObject> OnObjectExited;

        private HashSet<BaseObject> objectsInside = new();
        private readonly int width = 200;
        private readonly int height = 500;
        private float speed = 3f; // Скорость движения

        public override void Render(Graphics g)
        {
            var brush = new SolidBrush(Color.Black);
            g.FillRectangle(brush, -width / 2, -height / 2, width, height);
            g.DrawRectangle(new Pen(Color.Black, 2), -width / 2, -height / 2, width, height);
        }

        public override GraphicsPath GetGraphicsPath()
        {
            var path = base.GetGraphicsPath();
            path.AddRectangle(new Rectangle(-width / 2, -height / 2, width, height));
            return path;
        }

        public void Move(int boundsWidth)
        {
            X += speed;

            if (X > boundsWidth + width / 2)
            {
                X = -width / 2; // Появляется слева
            }
        }

        public void CheckOverlaps(IEnumerable<BaseObject> objects, Graphics g)
        {
            var currentInside = new HashSet<BaseObject>();

            // Проверяем пересечения со всеми объектами
            foreach (var obj in objects)
            {
                if (obj != this && this.Overlaps(obj, g))
                {
                    currentInside.Add(obj);

                    // Если объект только что вошел в область
                    if (!objectsInside.Contains(obj))
                    {
                        OnObjectEntered?.Invoke(obj);
                    }
                }
            }

            // Проверяем объекты, которые вышли из области
            foreach (var obj in objectsInside)
            {
                if (!currentInside.Contains(obj))
                {
                    OnObjectExited?.Invoke(obj);
                }
            }

            objectsInside = currentInside;
        }
    }
}