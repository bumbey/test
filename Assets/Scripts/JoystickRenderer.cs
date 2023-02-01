using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickRenderer : MonoBehaviour {
    public Transform endpoint;
    public LineRenderer line;

    public void SetOriginPosition(Vector2 pos) {
        transform.position = pos;
        line.SetPosition(0, pos);
        SetEndpointPosition(pos);
    }

    public void SetEndpointPosition(Vector2 pos) {
        endpoint.position = pos;
        line.SetPosition(1, endpoint.position);
        endpoint.transform.up = endpoint.position - transform.position;
    }
}