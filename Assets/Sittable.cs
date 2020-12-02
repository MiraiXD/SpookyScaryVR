using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sittable : MonoBehaviour
{
    public Transform interactionTransform;
    public Transform bodyEffector, rightHandEffector, rightShoulderEffector, leftHandEffector, leftShoulderEffector;
    public bool isFree { get; set; } = true;
}
