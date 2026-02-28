using UnityEngine;

namespace SG
{
    public class WeaponModelInstantiationSlot : MonoBehaviour
    {
        //这是哪个槽位？左手还是右手？，或是腰间或背上
        public WeaponModelSlot weaponSlot;

        public GameObject currentWeaponModel;

        public void UnloadWeapon()
        {
            if(currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeapon(GameObject weaponModel)
        {
            currentWeaponModel = weaponModel;
            weaponModel.transform.parent = transform;

            //重置位置、旋转和缩放
            weaponModel.transform.localPosition = Vector3.zero;
            weaponModel.transform.localRotation = Quaternion.identity;
            weaponModel.transform.localScale = Vector3.one;
        }
    }
}
