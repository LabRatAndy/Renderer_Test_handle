using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Renderer
{
    /// <summary>
    /// enums for camera movements 
    /// </summary>
    public enum CameraMovement
    {
        CameraForward,
        CameraBackward,
        CameraLeft,
        CameraRight,
        CameraUp,
        CameraDown,
        CameraPanLeft,
        CameraPanRight,
        CameraTiltUp,
        CameraTiltDown
    }
    /// <summary>
    /// class to abstract camera functions
    /// </summary>
    public class Camera
    {
        private Vector3 position;
        private Vector3 front;
        private Vector3 up;
        private Vector3 right;
        private Vector3 worldup;
        private float yaw;
        private float pitch;
        private float movementspeed;
        private float mousesensitivity;
        private float zoom;

        public Camera(Vector3 position, Vector3 up, float yaw = -90.0f, float pitch = 0.0f, float movementspeed = 3.0f,
            float mousesensitvity = 0.25f,float zoom = 45.0f)
        {
            this.position = position;
            this.worldup = up;
            this.yaw = yaw;
            this.pitch = pitch;
            this.mousesensitivity = mousesensitvity;
            this.movementspeed = movementspeed;
            this.zoom = zoom;
            this.front = new Vector3(0.0f, 0.0f, -1.0f);
            UpdateCameraVectors();
        }
        public Camera(float px, float py, float pz, float ux,float uy, float uz,float yaw = -90.0f,float pitch = 0.0f,float movementspeed = 3.0f,
            float mousesensitivity = 0.25f, float zoom = 45.0f)
        {
            this.position = new Vector3(px, py, pz);
            this.worldup = new Vector3(ux, uy, uz);
            this.yaw = yaw;
            this.pitch = pitch;
            this.mousesensitivity = mousesensitivity;
            this.movementspeed = movementspeed;
            this.zoom = zoom;
            this.front = new Vector3(0.0f, 0.0f, -1.0f);
            UpdateCameraVectors();
        }
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float MovementSpeed
        {
            get { return movementspeed; }
            set { movementspeed = value; }
        }
        public float MouseSensitivity
        {
            get { return mousesensitivity; }
            set { mousesensitivity = value; }
        }
        public float Zoom
        {
            get { return Zoom; }
            set { zoom = value; }
        }
        public float Yaw
        {
            get { return yaw; }
        }
        public float Pitch
        {
            get { return pitch; }
        }
        /// <summary>
        /// calculates the view matrix transform
        /// </summary>
        /// <returns>matrix4 containing transform</returns>
        public Matrix4 GetVeiwMatrix()
        {
            return Matrix4.LookAt(position, front, up);
        }
        /// <summary>
        /// moves camera with the keyboard
        /// </summary>
        /// <param name="cameraMovement">type of camera movement based on cameramovement enum</param>
        /// <param name="timeDelta">time key pressed for </param>
        public void ProcessKeyboard(CameraMovement cameraMovement, float timeDelta)
        {
            float velocity = movementspeed * timeDelta;
            if (cameraMovement == CameraMovement.CameraForward) position += front * velocity;
            if (cameraMovement == CameraMovement.CameraBackward) position -= front * velocity;
            if (cameraMovement == CameraMovement.CameraLeft) position -= right * velocity;
            if (cameraMovement == CameraMovement.CameraRight) position += right * velocity;
            if (cameraMovement == CameraMovement.CameraUp) position += worldup * velocity;
            if (cameraMovement == CameraMovement.CameraDown) position -= worldup * velocity;
            if (cameraMovement == CameraMovement.CameraPanLeft)
            {
                yaw -= velocity;
                UpdateCameraVectors();
            }
            if(cameraMovement == CameraMovement.CameraPanRight)
            {
                yaw += velocity;
                UpdateCameraVectors();
            }
            if(cameraMovement == CameraMovement.CameraTiltUp)
            {
                pitch += velocity;
                UpdateCameraVectors();
            }
            if(cameraMovement == CameraMovement.CameraTiltDown)
            {
                pitch -= velocity;
                UpdateCameraVectors();
            }
        }
        /// <summary>
        /// processes movemen of the camera via the mouse
        /// </summary>
        /// <param name="xOffset">change in the X axis</param>
        /// <param name="yOffset">change in the Y axis</param>
        /// <param name="constrainPitch">constrains the pitch</param>
        public void ProcessMouseMovement(float xOffset, float yOffset, bool constrainPitch = true)
        {
            xOffset *= mousesensitivity;
            yOffset *= mousesensitivity;
            yaw += xOffset;
            pitch += yOffset;
            if(constrainPitch)
            {
                if (pitch > 89.0f) pitch = 89.0f;
                if (pitch < -89.0f) pitch = -89.0f;
            }
            UpdateCameraVectors();
        }
        /// <summary>
        /// processes scroll wheel zoom chnages 
        /// </summary>
        /// <param name="offset">change in the zoom</param>
        public void ProcessMouseScroll(float offset)
        {
            if (zoom >= 1.0f && zoom <= 45.0f) zoom -= offset;
            if (zoom <= 1.0f) zoom = 1.0f;
            if (zoom >= 45.0f) zoom = 45.0f;
        }
        /// <summary>
        /// updates the camera front, up, and right vectors
        /// </summary>
        private void UpdateCameraVectors()
        {
            Vector3 front;
            front.X = (float)(System.Math.Cos(DegreesToRadians((double)yaw)) * System.Math.Cos(DegreesToRadians((double)pitch)));
            front.Y = (float)System.Math.Sin(DegreesToRadians((double)pitch));
            front.Z = (float)(System.Math.Sin(DegreesToRadians((double)yaw)) * System.Math.Cos(DegreesToRadians((double)pitch)));
            this.front = Vector3.Normalize(front);
            this.right = Vector3.Normalize(Vector3.Cross(front, worldup));
            this.up = Vector3.Normalize(Vector3.Cross(right, front));
        }
        /// <summary>
        /// converts degrees to radians 
        /// </summary>
        /// <param name="degrees">value in degrees</param>
        /// <returns>value in radians</returns>
        private double DegreesToRadians(double degrees)
        {
            return degrees * (System.Math.PI / 180);
        }
    }
    public class newCamera
    {
        private Vector3 position;
        private Vector3 lookDirection;
        private Vector3 upDirection;
        private float nearPlaneDistance;
        private float farPlaneDistance;
        private double fieldOfView;
        /// <summary>
        /// sets the cameras position in the world
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        /// <summary>
        /// normalised vector to represent the direction in which we are looking
        /// </summary>
        public Vector3 LookDirection
        {
            get { return lookDirection; }
            set { lookDirection = value; }
        }
        /// <summary>
        /// normalised vector to represent the up direction
        /// </summary>
        public Vector3 UpDirection
        {
            get { return upDirection; }
            set { upDirection = value; }
        }
        /// <summary>
        /// returns vector indicating the left direction 
        /// </summary>
        public Vector3 LeftDirection
        {
            get { return Vector3.Cross(lookDirection, upDirection); }
     
        }
        public float NearPlaneDistance
        {
            get { return nearPlaneDistance; }
            set { nearPlaneDistance = value; }
        }
        public float FarPlaneDistance
        {
            get { return farPlaneDistance; }
            set { farPlaneDistance = value; }
        }
        public double FieldOfView
        {
            get { return fieldOfView; }
            set { fieldOfView = value; }
        }
        public Matrix4 GetViewMatrix
        {
            get { return Matrix4.LookAt(position, lookDirection, upDirection); }
            
        }
        public newCamera(Vector3 position,Vector3 lookDirection,Vector3 upDirection)
        {
            this.position = position;
            this.lookDirection = lookDirection;
            this.upDirection = upDirection;
            nearPlaneDistance = float.Epsilon;
            farPlaneDistance = 600.0f;
            fieldOfView = 45.0f;
        }
        public newCamera(Vector3 position, Vector3 lookDirection, Vector3 upDirection, float nearPlaneDistance, float farPlaneDistance, double fieldOfView)
        {
            this.position = position;
            this.lookDirection = lookDirection;
            this.upDirection = upDirection;
            this.nearPlaneDistance = nearPlaneDistance;
            this.farPlaneDistance = farPlaneDistance;
            this.fieldOfView = fieldOfView;
        }
        public void Move(Vector3 direction, float amount)
        {
            position += direction * amount;
        }
        public void SetYaw(float angle)
        {
            lookDirection = Rotate(lookDirection, upDirection, angle);
        }
        public void SetPitch(float angle)
        {

        }
        public void SetRoll(float angle)
        {

        }
        private static Vector3 Rotate(this Vector3 input, Vector3 rotationAxis, float rotationAngle)
        {
            Quaternion rotation = Quaternion.FromAxisAngle(rotationAxis, rotationAngle);
            return Vector3.Transform(input, rotation);
        }
    }
}