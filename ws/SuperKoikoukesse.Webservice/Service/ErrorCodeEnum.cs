﻿
namespace SuperKoikoukesse.Webservice.Service
{
    /// <summary>
    /// Available error codes for webservice
    /// </summary>
    public enum ErrorCodeEnum : int
    {
        Ok  = 0,
        InvalidRequest = 100,
        ServiceError = 500,
    }
}