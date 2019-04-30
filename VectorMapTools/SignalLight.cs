#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.UnityTools.VectorMapTools
{
    [ExecuteInEditMode]
    public class SignalLight : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, 30, 10, 0.2f, 1);
        }
    }
}