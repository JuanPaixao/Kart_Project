using UnityEngine;

[CreateAssetMenu(fileName = "KartData", menuName = "KartProject/KartData", order = 0)]
public class KartData : ScriptableObject
{
    public float turnSpeed;
    public float maxSpeed;
    public float acceleration;
    public float deacceleration;
    public float brakeForce;
    public float driftSpeed;
}

