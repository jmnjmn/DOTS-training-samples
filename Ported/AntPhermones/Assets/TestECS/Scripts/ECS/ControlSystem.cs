// using Unity.Entities;
// using Unity.Transforms;
// using UnityEngine;
//
// public class ControlSystem : ComponentSystem
// {
//     protected override void OnUpdate()
//     {
//         if (!GameRoot.Instance.useJobSystem)
//         {
//             Entities.ForEach((ref Rotation rotate, ref Translation translation,ref RotateSpeed rotateSpeed) =>
//             {
//                 Quaternion q = rotate.Value;
//                 var ang = q.eulerAngles;
//                 ang.y = ang.y + Time.DeltaTime * rotateSpeed.Speed;
//                 rotate.Value = Quaternion.Euler(ang);
//             
//                 Vector3 pos = Random.insideUnitSphere * 40;
//                 translation.Value = pos;
//             });
//         }
//         
//     }
// }