using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interfaces : MonoBehaviour
{
    public interface IDamageable
    {
        void Damage(int amount);
    }
}
