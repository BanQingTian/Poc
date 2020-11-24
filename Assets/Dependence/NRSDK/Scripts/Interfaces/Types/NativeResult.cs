﻿/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    internal enum NativeResult
    {
        Success = 0,
        Failure = 1,
        InvalidArgument = 2,
        NotEnoughMemory = 3,
        UnSupported = 4,
        GlassesDisconnect = 5,
        SdkVersionMismatch = 6,
        SdcardPermissionDeny = 7,
        RGBCameraDeviceNotFind = 8,
        DPDeviceNotFind = 9,
        TrackingNotRunning = 10,
        GetDisplayFailure = 11,
        GetDisplayModeMismatch = 12,
    }
}
