using System;
using System.Collections.Generic;
namespace Renderer
{
    /// <summary>
    /// Singleton to manage cameras for the renderer
    /// </summary>
    internal sealed class CameraManager
    {
        /// <summary>
        /// class to hold meta data for the camera
        /// </summary>
        private class CameraMetaData
        {
            private readonly Camera theCamera;
            private readonly string cameraName;
            internal CameraMetaData(Camera theCamera, string cameraName)
            {
                this.theCamera = theCamera;
                this.cameraName = cameraName;
            }
            internal Camera TheCamera
            {
                get { return theCamera; }
            }
            internal string CameraName
            {
                get { return cameraName; }
            }
        }
        private List<CameraMetaData> cameraList = null;
        private static readonly Lazy<CameraManager> lazy = new Lazy<CameraManager>(() => new CameraManager());
        private int activeCamera;
        /// <summary>
        /// creates or returns the camera manager object
        /// </summary>
        internal static CameraManager Instance
        {
            get { return lazy.Value; }
        }
        private CameraManager()
        {
            cameraList = new List<CameraMetaData>();
        }
        /// <summary>
        /// Adds a camera to the manager.
        /// throws a argument exception if name is null, empty, or white space
        /// throws a argument null exception if camera object is null
        /// </summary>
        /// <param name="newCamera">Camera object to be added</param>
        /// <param name="name">string wiht the name of the camera</param>
        /// <returns>index to refer to the camera</returns>
        internal int  AddCamera(Camera newCamera, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name cannot be null, empty, or whitespace");
            if (newCamera == null) throw new ArgumentNullException("newCamera");
            CameraMetaData cameraData = new CameraMetaData(newCamera, name);
            cameraList.Add(cameraData);
            return cameraList.IndexOf(cameraData);
        }
        /// <summary>
        /// Returns any camera in the manager
        /// </summary>
        /// <param name="index">index of the camera as given by the addcamera method</param>
        /// <returns>The requested camera object</returns>
        internal Camera GetCamera(int index)
        {
            return cameraList[index].TheCamera;
        }
        /// <summary>
        /// Returns any camera in the manager
        /// throws argument exception if string is null, empty or whitespace
        /// throws argumwnt exception if name isn't found
        /// </summary>
        /// <param name="name">string containing the camera name</param>
        /// <returns>The requested camera object</returns>
        internal Camera GetCamera(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("camera name string is null or white space");
            }
            foreach (var item in cameraList)
            {
                if (item.CameraName == name) return item.TheCamera;
            }
            throw new ArgumentException("camera name not found");
        }
        /// <summary>
        /// Removes a camera from the manager
        /// </summary>
        /// <param name="index">index of the camera as returned by addcamera method</param>
        internal void RemoveCamera(int index)
        {
            cameraList.RemoveAt(index);
        }
        /// <summary>
        /// removes as camera from the manager
        /// throws argument exception if name is null,empty,or whitespace
        /// </summary>
        /// <param name="name">string containing camera name</param>
        internal void RemoveCamera(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Camera name is null, empty or whitespace");
            foreach(var item in cameraList)
            {
                if (item.CameraName == name) cameraList.Remove(item);
            }
        }
        /// <summary>
        /// Gets the camera marked as active
        /// </summary>
        /// <returns>Camera object that is marked as active</returns>
        internal Camera GetActiveCamera()
        {
            return cameraList[activeCamera].TheCamera;
        }
        /// <summary>
        /// Sets the camera that will be returned by get active camera
        /// </summary>
        /// <param name="index">the index of the camera as returned by add camera</param>
        internal void SetActiveCamera(int index)
        {
            activeCamera = index;
        }
        /// <summary>
        /// Sets the camera that will be returned by get active camera
        /// throws argument exception if string is null, empty or whitespace
        /// throws argumwnt exception if name isn't found
        /// </summary>
        /// <param name="name">string with the name </param>
        internal void SetActiveCamera(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Camera name is null, empty, or whitespace");
            for (int n = 0; n < cameraList.Count; n++)
            {
                if (cameraList[n].CameraName == name)
                {
                    activeCamera = n;
                    return;
                }
            }
            throw new ArgumentException("Camera name not found");
        }
    }
}