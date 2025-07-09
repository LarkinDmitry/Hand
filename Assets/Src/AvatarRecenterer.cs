using UnityEngine;

/// <summary>
/// Отвечает за установку XR камеры в определённое положение в пространстве 
/// </summary>
public class AvatarRecenterer : MonoBehaviour
{
    [SerializeField] private Transform _avatarRoot;
    [SerializeField] private Transform _target;
    [Space]
    [SerializeField] private bool _yOnlyRecenter = true;

    private float _xFirstAngleTrashhpld = 30;
    private float _xSecondAngleTrashhpld = 60;
    private float _smoothSpeed = 2.0f;
    private Transform _cam;

    private void Start()
    {
        _cam = Camera.main.transform;
        ReCenterCamera();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReCenterCamera();
        }
    }

    public void ReCenterCamera()
    {
        float absCamAngleX = Mathf.Abs(NormalizeAngle(_cam.localEulerAngles.x));

        if (absCamAngleX < _xFirstAngleTrashhpld || _yOnlyRecenter)
        {
            // Угол подъема головы относительно горизонта менее _xFirstAngleTrashhpld (или более сложная рецентровка не требуется).
            // Самый простой вариант т.к. пациент или стоит или сидит с прямой спиной.
            // Делаем так что-бы игровой горизонт совпадал с реальным горизонтом.
            // Разворачиваем камеру, в направлении цели, только вокруг вертикальной оси.
            float angleY = NormalizeAngle(_target.rotation.eulerAngles.y) - NormalizeAngle(_cam.rotation.eulerAngles.y);
            _avatarRoot.rotation *= Quaternion.Euler(0, angleY, 0);
            _avatarRoot.rotation = Quaternion.Euler(0, _avatarRoot.rotation.eulerAngles.y, 0);
        }
        else
        {
            // Угол подъема головы относительно горизонта больше или равен _xFirstAngleTrashhpld.
            // Пациент вероятнее всего находится в полусидячем-полулежачем или лежачем положении.
            // Поворачиваем уровень игрового горизонта так, что-бы пациенту казалось что он находится в вертикальном положении,
            // а так же разворачиваем камеру, в направлении цели, вокруг вертикальной оси

            if (absCamAngleX < _xSecondAngleTrashhpld)
            {
                // Если угол подъема головы относительно горизонта менее _xSecondAngleTrashhpld, то пациент находится в
                // полулежачем положении. Угол поворота головы вокруг оси Z нужно игнорирровать т.к. пациент
                // мог просто случайно наклонить голову в бок.
                _cam.localRotation = Quaternion.Euler(_cam.localRotation.eulerAngles.x, _cam.localRotation.eulerAngles.y, 0);
            }

            System.Numerics.Quaternion target = new(_target.rotation.x, _target.rotation.y, _target.rotation.z, _target.rotation.w);
            System.Numerics.Quaternion cam = new(_cam.rotation.x, _cam.rotation.y, _cam.rotation.z, _cam.rotation.w);
            var delta = System.Numerics.Quaternion.Divide(target, cam);

            _avatarRoot.rotation = new Quaternion(delta.X, delta.Y, delta.Z, delta.W) * _avatarRoot.rotation;
        }

        _avatarRoot.position += _target.position - _cam.position;
    }

    /// <summary>
    /// return an angle between -180...180 degrees
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private float NormalizeAngle(float angle)
    {
        return angle > 180 ? angle - 360 : angle < -180 ? angle + 360 : angle;
    }
}