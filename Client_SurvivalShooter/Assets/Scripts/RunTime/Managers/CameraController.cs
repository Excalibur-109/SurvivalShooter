using Excalibur;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    private Vector3[] _border;
    private Transform _camTransfom;
    private Transform _followTarget;

    public void SetTarget(Transform followTarget)
    {
        _followTarget = followTarget;
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
        _UpdatePositon(_followTarget.position);
    }

    public void MoveTo(Vector3 position, int time = 1)
    {
        if (_camTransfom.position != position)
        {
            CharacterManager.Instance.Player.DisablePlayerInput();
            Vector3 startPosition = _camTransfom.position;
            Timing.Instance.ScheduleOnce(time, () =>
            {
                _UpdatePositon(position);
                CharacterManager.Instance.Player.EnablePlayerInput();
            }, (elapsed) =>
            {
                _UpdatePositon(Vector3.Lerp(startPosition, position, (float)elapsed / time));
            });
        }
    }

    private void _UpdatePositon(Vector3 position)
    {
        position.z = _camTransfom.transform.position.z;
        _camTransfom.transform.position = position;
    }
}
