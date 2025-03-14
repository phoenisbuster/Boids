using UnityEngine;
using System.Collections;
namespace MasterStylizedProjectile
{
    [ExecuteInEditMode]
    public class BillBoardParticles : MonoBehaviour
    {

        public bool bTurnOver = false;

        void OnWillRenderObject()
        {
            if (UnityEngine.Camera.current)
            {
                if (bTurnOver)
                    transform.forward = UnityEngine.Camera.current.transform.forward;
                else
                    transform.forward = -UnityEngine.Camera.current.transform.forward;
            }
        }
    }
}