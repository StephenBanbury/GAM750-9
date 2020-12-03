using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class Move : MonoBehaviour
    {
        private float _moveSpeed;
        private float _rotateSpeed;
        private Vector3 _origPosition;
        private Quaternion _origRotation;

        void Start()
        {
            _moveSpeed = 6f;
            _rotateSpeed = 20f;

            _origPosition =
                new Vector3(transform.position.x, transform.position.y, transform.position.z);
            _origRotation =
                new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        }

        void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.E))
            {
                transform.position += transform.forward * Time.deltaTime * _moveSpeed;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                transform.position -= transform.forward * Time.deltaTime * _moveSpeed;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * Time.deltaTime * _moveSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * Time.deltaTime * _moveSpeed;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.up * Time.deltaTime * _moveSpeed;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.up * Time.deltaTime * _moveSpeed;
            }


            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(-Vector3.up * _rotateSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Rotate(Vector3.right * _rotateSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Rotate(-Vector3.right * _rotateSpeed * Time.deltaTime);
            }


            if (Input.GetKey(KeyCode.Space))
            {
                transform.position = _origPosition;
                transform.rotation = _origRotation;
            }
        }
    }
}