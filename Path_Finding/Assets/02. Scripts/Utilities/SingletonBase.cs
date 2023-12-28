
using UnityEngine;

public class SingletonBase : MonoBehaviour
{
    protected static bool _isDisabled;
    protected static readonly object Locked = new object();
}