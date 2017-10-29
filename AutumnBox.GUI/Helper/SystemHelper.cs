/* =============================================================================*\
*
* Filename: SystemHelper.cs
* Description: 
*
* Version: 1.0
* Created: 10/2/2017 05:04:40(UTC+8:00)
* Compiler: Visual Studio 2017
* 
* Author: zsh2401
* Company: I am free man
*
\* =============================================================================*/
using AutumnBox.Basic.Executer;
using AutumnBox.Shared.CstmDebug;
using System;
using System.Diagnostics;
using System.Threading;

namespace AutumnBox.GUI.Helper
{
    /// <summary>
    /// ϵͳ������,��ϵͳ��صľ�̬����
    /// </summary>
    public static class SystemHelper
    {
        /// <summary>
        /// ɱ��һ������
        /// </summary>
        /// <param name="processName"></param>
        public static void KillProcess(string processName)
        {
            var list = Process.GetProcessesByName(processName);
            foreach (Process p in list)
            {
                p.Kill();
            }
        }
        /// <summary>
        /// �ж��Ƿ���win10ϵͳ
        /// </summary>
        public static bool IsWin10
        {
            get
            {
                return Environment.OSVersion.Version.Major == 10;
            }
        }
        /// <summary>
        /// �˳�����App
        /// </summary>
        /// <param name="exitCode"></param>
        public static void AppExit(int exitCode = 0)
        {
            Logger.T("SystemHelper", "Exiting.....");
            App.DevicesListener.Stop();
            CommandExecuter.Kill();
            Environment.Exit(exitCode);
        }

        #region �ڴ���� http://www.cnblogs.com/xcsn/p/4678322.html
        static bool continueAutoGC = true;
        /// <summary>
        /// ��ʼ�Զ�GC
        /// </summary>
        public static void StartAutoGC()
        {
            continueAutoGC = true;
            new Thread(AutoGC) { Name = "Auto GC" }.Start();
        }
        /// <summary>
        /// �����Զ�GC
        /// </summary>
        public static void StopAutoGC()
        {
            continueAutoGC = false;
        }
        /// <summary>
        /// ����ѭ�����ڴ���շ���
        /// </summary>
        private static void AutoGC()
        {
            while (continueAutoGC)
            {
                ClearMemory();
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// �ͷ��ڴ�
        /// </summary>
        private static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Util.NativeMethods.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion
    }
}