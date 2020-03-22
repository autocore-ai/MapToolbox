#region License
/******************************************************************************
* Copyright 2018-2020 The AutoCore Authors. All Rights Reserved.
* 
* Licensed under the GNU Lesser General Public License, Version 3.0 (the "License"); 
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
* https://www.gnu.org/licenses/lgpl-3.0.html
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*****************************************************************************/
#endregion


using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AutoCore.MapToolbox.PCL
{
    public class NativePluginDebug
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void UnmanagedFunctionDelegate(IntPtr strPtr);
        [DllImport(Const.unity_debug_plugin)]
        static extern void SetFunctionDebugLog(IntPtr fp);
        [DllImport(Const.unity_debug_plugin)]
        static extern void SetFunctionDebugLogWarning(IntPtr fp);
        [DllImport(Const.unity_debug_plugin)]
        static extern void SetFunctionDebugLogError(IntPtr fp);
        static void Log(IntPtr strPtr) => Debug.Log($"{Const.unity_debug_plugin}:{Marshal.PtrToStringAnsi(strPtr)}");
        static void LogWarning(IntPtr strPtr) => Debug.LogWarning($"{Const.unity_debug_plugin}:{Marshal.PtrToStringAnsi(strPtr)}");
        static void LogError(IntPtr strPtr) => Debug.LogError($"{Const.unity_debug_plugin}:{Marshal.PtrToStringAnsi(strPtr)}");
        [RuntimeInitializeOnLoadMethod]
        public static void RegisterDebugFunctions()
        {
            UnmanagedFunctionDelegate delegate_log = Log;
            UnmanagedFunctionDelegate delegate_log_warning = LogWarning;
            UnmanagedFunctionDelegate delegate_log_error = LogError;
            SetFunctionDebugLog(Marshal.GetFunctionPointerForDelegate(delegate_log));
            SetFunctionDebugLogWarning(Marshal.GetFunctionPointerForDelegate(delegate_log_warning));
            SetFunctionDebugLogError(Marshal.GetFunctionPointerForDelegate(delegate_log_error));
        }
    }
}