﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/10/21 1:28:25 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.OpenFramework.Content;
using AutumnBox.OpenFramework.Management.Impl;
using AutumnBox.OpenFramework.Open.ServiceImpl;
using AutumnBox.OpenFramework.Service;
using AutumnBox.OpenFramework.Service.Default;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AutumnBox.OpenFramework.Management
{
    /// <summary>
    /// 框架加载器
    /// </summary>
#if SDK
    internal
#else
    public
#endif
        static class FxLoader
    {
        [ContextPermission(CtxPer.High)]
        private class FxLoaderContext : Context { }
        private static Context fxLoaderCtx;

        /// <summary>
        /// 完全加载
        /// </summary>
        /// <param name="baseApi"></param>
        public static void LoadBase(IBaseApi baseApi)
        {
            IServicesManager serviceManager = Manager.ServicesManager;
            serviceManager.StartService<SMd5>();
            serviceManager.StartService<SBaseApiContainer>();
            serviceManager.StartService<SSoundManager>();
            var apiContainer = (SBaseApiContainer)serviceManager
                 .GetServiceByName(null,SBaseApiContainer.NAME);
            apiContainer.LoadApi(baseApi);
            fxLoaderCtx = new FxLoaderContext();
        }
        /// <summary>
        /// 加载拓展模块
        /// </summary>
        public static void LoadExtensions()
        {
            IServicesManager serviceManager = Manager.ServicesManager;
            serviceManager.StartService<InternalManagerImpl>();
            var internalManager = fxLoaderCtx
                .GetService<InternalManagerImpl>(InternalManagerImpl.SERVICE_NAME);
            internalManager.Reload();
        }

        /// <summary>
        /// 卸载拓展模块
        /// </summary>
        public static void UnloadExtensions()
        {
            foreach (var lib in Manager.InternalManager.Librarians)
            {
                try { lib.Destory(); } catch { }
            }
        }
        /// <summary>
        /// 停止并摧毁服务
        /// </summary>
        public static void UnloadServices()
        {
            ServicesManagerImpl serviceManager = (ServicesManagerImpl)Manager.ServicesManager;
            foreach (var serv in serviceManager._serviceCollection)
            {
                try { serv.Stop(); } catch { }
                try { serv.Destory(); } catch { }
            }
        }
    }
}