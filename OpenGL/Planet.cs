﻿using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace OpenGL
{
    internal class Planet : LinkableObject
    {
        private Accumulator _power;
        private Label _label;

        public bool Highlighted { get; private set; } = false;

        public readonly Color4 DefaultColour = new Color4(1f, 1f, 1f, 1f);

        public Planet(Vector3 position)
            : base(position)
        {
            Color = DefaultColour;
            Width = Height = 1f;
        }

        public override void Init()
        {
            LoadTexture("Textures/GasGiant.png");

            _power = new Accumulator { Rate = 1, Value = 0, Limit = 10, Enabled = true };
            _label = new Label(new Vector3(0f, 0f, 0f), "0");
            Children.Add(_label);

            foreach (var item in Children)
            {
                item.Init();
            }
        }

        public override void Draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            GL.PushMatrix();
            GL.Translate(Position);

            DrawPlanet();

            foreach (var item in Children)
            {
                item.Draw();
            }

            GL.PopMatrix();
        }

        private void DrawPlanet()
        {
            var slices = 50;
            var twicePi = 2f * Math.PI;

            GL.Color4(Color);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.TexCoord2(0.5f, 0.5f);
            GL.Vertex2(0f, 0f);
            for (int i = 0; i <= slices; i++)
            {
                GL.TexCoord2(
                    Math.Cos(i * twicePi / slices) * 0.5 + 0.5,
                    Math.Sin(i * twicePi / slices) * 0.5 + 0.5);
                GL.Vertex2(
                    Width * Math.Cos(i * twicePi / slices),
                    Height * Math.Sin(i * twicePi / slices));
            }

            GL.End();
        }

        public override void Update(double time)
        {
            _power.Update(time);
            var powerVal = (int)_power.Value;
            _label.UpdateText(powerVal.ToString());
        }

        public override bool Intersect(Ray ray)
        {
            Vector3 ray_dir = (ray.To - ray.From).Normalized();
            var t = Vector3.Dot((ray.From - Position), ray_dir);
            var distanceAlongRay = -t;
            var distanceToSphereOrigin = ((ray.From - Position) - t * ray_dir).Length;
            var isIntersect = distanceToSphereOrigin <= Width;
            return isIntersect;
        }

        public void Highlight()
        {
            Highlighted = true;
            Color = new Color4(1f, 1f, 0f, 1f);
        }
        public void UnHighlight()
        {
            Highlighted = false;
            Color = DefaultColour;
        }
    }
}
