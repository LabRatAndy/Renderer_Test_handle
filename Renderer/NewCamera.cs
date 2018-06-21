using OpenTK;
using System;
namespace Renderer
{
    public class NewCamera
    {
        private Vector3 position = Vector3.Zero;
        private Vector3 oreientation = new Vector3((float)Math.PI, 0f, 0f);
        private float moveSpeed = 0.2f;
        private float mouseSenitivity = 0.01f;
        public Vector3 Position
        {
            get { return position; }
        }
        public Vector3 Oreientation
        {
            get { return oreientation; }
        }
        public float MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }
        public float MouseSensitivity
        {
            get { return mouseSenitivity; }
            set { mouseSenitivity = value; }
        }
        public NewCamera()
        {
            position = Vector3.Zero;
            oreientation = new Vector3((float)Math.PI, 0.0f, 0.0f);
            moveSpeed = 0.2f;
            MouseSensitivity = 0.01f;
        }
        public NewCamera(Vector3 position, Vector3 oreientation, float moveSpeed, float mouseSensitivity)
        {
            this.position = position;
            this.oreientation = oreientation;
            this.moveSpeed = moveSpeed;
            this.mouseSenitivity = mouseSensitivity;
        }
        public Matrix4 GetViewMatrix()
        {
            Vector3 lookAt = new Vector3();
            lookAt.X = (float)(Math.Sin(oreientation.X) * Math.Cos(oreientation.Y));
            lookAt.Y = (float)(Math.Sin(oreientation.Y));
            lookAt.Z = (float)(Math.Cos(oreientation.X) * Math.Cos(oreientation.Y));
            return Matrix4.LookAt(position, position + lookAt, Vector3.UnitY);
        }
        public void Move(float x, float y, float z)
        {
            Vector3 offset = new Vector3();
            Vector3 forward = new Vector3((float)Math.Sin(oreientation.X), 0, (float)Math.Cos(oreientation.X));
            Vector3 right = new Vector3(-forward.Z, 0, -forward.X);
            offset += x * right;
            offset += y * forward;
            offset.Y += z;
            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, moveSpeed);
            position += offset;
        }
        public void Rotation(float x, float y)
        {
            x = x + mouseSenitivity;
            y = y + mouseSenitivity;
            oreientation.X = (oreientation.X + x) % ((float)Math.PI * 2.0f);
            oreientation.Y = Math.Max(Math.Min(oreientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
        }
    }
}