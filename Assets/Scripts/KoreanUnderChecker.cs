using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoreanUnderChecker : MonoBehaviour
{
    static public bool UnderCheck(string name) {
        if (name.Length == 0) return false;
        int last = name[name.Length-1];
        int criteria = ( last - 44032 ) % 28;
        return (criteria == 0) ? false : true;
    }
}
