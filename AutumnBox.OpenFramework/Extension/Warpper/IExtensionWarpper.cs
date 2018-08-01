﻿using AutumnBox.Basic.Device;
using System;

namespace AutumnBox.OpenFramework.Extension
{
    /// <summary>
    /// 拓展模块包装器
    /// </summary>
    public interface IExtensionWarpper 
    {
        /// <summary>
        /// 模块名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 模块说明
        /// </summary>
        string Desc { get; }
        /// <summary>
        /// 运行前检查
        /// </summary>
        /// <returns></returns>
        ForerunCheckResult ForerunCheck();
        /// <summary>
        /// 开始运行
        /// </summary>
        /// <param name="device"></param>
        void Run(DeviceBasicInfo device);
        /// <summary>
        /// 停止运行
        /// </summary>
        /// <returns></returns>
        bool Stop();
        /// <summary>
        /// 当该包装类被要求摧毁时调用
        /// </summary>
        void Destory();
    }
}