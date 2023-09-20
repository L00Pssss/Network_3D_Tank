using System;
using UnityEngine;

public class TestDimension : MonoBehaviour
{
        [SerializeField] private VehicleDimensions vehicleDimensions;

        private void Update()
        {
                Debug.Log(vehicleDimensions.IsVisibleFromPoint(transform.root, transform.position, Color.red));
        }
}
