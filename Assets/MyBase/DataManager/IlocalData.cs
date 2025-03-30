using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBase.DataManager
{
    /// <summary>
    /// Store data that serialized to JSON.
    /// </summary>
    public interface ILocalData
    {
        void InitAfterLoadData();
    }
}
