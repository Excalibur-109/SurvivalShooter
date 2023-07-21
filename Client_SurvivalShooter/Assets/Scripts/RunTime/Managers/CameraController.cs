using Excalibur;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    private Vector3[] _border;
    private Transform _camTransfom;
    private PositionComponent _position;

    public bool UpdateToTarget { get; set; } = true;

    public void SetTarget(PositionComponent followTarget)
    {
        _position = followTarget;
    }

    public void SetCamera(Transform camera)
    {
        _camTransfom = camera;
    }

    public void SetBorder(Vector3[] border)
    {
        _border = border;
    }

    public void UpdatePosition()
    {
        if (UpdateToTarget)
        {
            _UpdatePositon(_position.pos);
        }
    }

    public void UpdatePosition(Vector3 position)
    {
        _UpdatePositon(position);
    }

    public void MoveTo(Vector3 position, int time = 1)
    {
        if (_camTransfom.position != position)
        {
            CharacterManager.Instance.Player.DisablePlayerInput();
            Vector3 startPosition = _camTransfom.position;
            Timing.Instance.Tick(1, (elapsed) =>
            {
                float factor = (float)elapsed / (time * 1000);
                _UpdatePositon(Vector3.Lerp(startPosition, position, factor));
            }, () =>
            {
                _UpdatePositon(position);
                CharacterManager.Instance.Player.EnablePlayerInput();
            });
        }
    }

    private void _UpdatePositon(Vector3 position)
    {
        position.z = _camTransfom.transform.position.z;
        _camTransfom.transform.position = position;
    }
}
