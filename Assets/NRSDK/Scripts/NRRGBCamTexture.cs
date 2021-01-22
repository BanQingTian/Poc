/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using UnityEngine;

    /// <summary> Create a rgb camera texture. </summary>
    public class NRRGBCamTexture : CameraModelView
    {
        /// <summary> The on update. </summary>
        public Action<CameraTextureFrame> OnUpdate;
        /// <summary> The current frame. </summary>
        public CameraTextureFrame CurrentFrame;
        /// <summary> The texture. </summary>
        private Texture2D m_Texture;

        /// <summary> Default constructor. </summary>
        public NRRGBCamTexture():base(CameraImageFormat.RGB_888)
        {
            this.m_Texture = CreateTexture();
        }

        /// <summary> Creates the texture. </summary>
        /// <returns> The new texture. </returns>
        private Texture2D CreateTexture()
        {
            return new Texture2D(Width, Height, TextureFormat.RGB24, false);
        }

        /// <summary> Gets the texture. </summary>
        /// <returns> The texture. </returns>
        public Texture2D GetTexture()
        {
            if (m_Texture == null)
            {
                m_Texture = CreateTexture();
            }
            return m_Texture;
        }

        /// <summary> Load raw texture data. </summary>
        /// <param name="rgbRawDataFrame"> .</param>
        protected override void OnRawDataUpdate(FrameRawData rgbRawDataFrame)
        {
            m_Texture.LoadRawTextureData(rgbRawDataFrame.data);
            m_Texture.Apply();

            CurrentFrame.timeStamp = rgbRawDataFrame.timeStamp;
            CurrentFrame.texture = m_Texture;

            OnUpdate?.Invoke(CurrentFrame);
        }

        /// <summary> On texture stopped. </summary>
        protected override void OnStopped()
        {
            GameObject.Destroy(m_Texture);
            m_Texture = null;
        }
    }
}
