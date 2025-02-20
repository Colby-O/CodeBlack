using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cure : MonoBehaviour
{
    public enum Type {
        Adrenaline,
        BetaBlockers,
        CalciumBlockers,
        Atropine,
        Digoxin,
        Ibuprofen,
        Furosemide,
        Defibrillator,
        Insulin,
        Food,
        Heat,
        Oxygen,
    }

    public Type cure;
}
