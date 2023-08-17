using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseFieldUI : MonoBehaviour
{
    protected ObjectMemoryPool<BaseFieldUI>.del_Register del_Register;
    public virtual BaseFieldUI Init(ObjectMemoryPool<BaseFieldUI>.del_Register register)
    {
        del_Register = register;
        return this;
    }
}
