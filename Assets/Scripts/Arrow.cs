using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f;
    public Transform tip;
    private Rigidbody _rigidBody;
    private bool _inAir = false;
    private Vector3 _lastPosition = Vector3.zero;
    private bool hasHit = false;
    private AudioSource[] audioSources;
    private void Awake()
    {
        audioSources = GetComponents<AudioSource>();
        _rigidBody = GetComponent<Rigidbody>();
        PullInteraction.PullActionReleased += Release;
        Stop();
    }
    private void OnDestroy()
    {
        PullInteraction.PullActionReleased -= Release;
    }
    private void Release(float value)
    {
        PullInteraction.PullActionReleased -= Release;
        gameObject.transform.parent = null;
        _inAir = true;
        SetPhysics(true);

        Vector3 force = transform.forward * value * speed;
        _rigidBody.AddForce(force, ForceMode.Impulse);
        StartCoroutine(RotateWithVelocity());
        _lastPosition = tip.position;
        audioSources[1].Play();
    }
    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (_inAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(_rigidBody.velocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }
    void FixedUpdate()
    {
        if (_inAir)
        {
            CheckCollision();
            _lastPosition = tip.position;
        }
    }
    private void CheckCollision()
    {
        if (Physics.Linecast(_lastPosition, tip.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject.TryGetComponent<ArrowSpawner>(out ArrowSpawner component)) return;
            if (hitInfo.transform.TryGetComponent(out Rigidbody body))
            {
                _rigidBody.interpolation = RigidbodyInterpolation.None;
                transform.parent = hitInfo.transform;
                body.AddForce(_rigidBody.velocity, ForceMode.Impulse);
            }
            Stop();

        }
    }
    private void Stop()
    {
        _inAir = false;
        SetPhysics(false);
    }
    private void SetPhysics(bool usePhysics)
    {
        _rigidBody.isKinematic = !usePhysics;
        _rigidBody.useGravity = usePhysics;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Target>(out Target target))
        {
            if (!hasHit)
            {
                audioSources[0].Play();
                hasHit = true;
                target.Hit();
            }
        }
    }
}

