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
using System.Linq.Expressions;

namespace AutoCore.MapToolbox.Editor
{
    static class Utils
    {
        public static string GetMethodName<T, TResult>(Func<T,TResult> method) => method.Method.Name;
        public static string GetMethodName<TResult>(Func<TResult> method) => method.Method.Name;
        public static string GetMethodName(Action method) => method.Method.Name;
        public static string GetMethodName<T>(Action<T> method) => method.Method.Name;
        public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess) => ((MemberExpression)memberAccess.Body).Member.Name;
    }
}