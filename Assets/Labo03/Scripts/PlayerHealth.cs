using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _pvs = 3;
    [SerializeField] private float frameDuration = 2f;
    [SerializeField] private Color damageColor = Color.red;
    private Color originalColor;
    private bool _peutRecevoirDommage = true;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    [SerializeField] private Volume globalVolume;

    private void Start()
    {
        Transform geometryTransform = transform.Find("Geometry");
        if (geometryTransform != null)
        {
            _skinnedMeshRenderer = geometryTransform.GetComponentInChildren<SkinnedMeshRenderer>();
            if (_skinnedMeshRenderer != null)
            {
                originalColor = _skinnedMeshRenderer.material.color;
            }
        }
    }

    public void PrendreDuDommage(int dommage)
    {
        if (!_peutRecevoirDommage) return;
        _peutRecevoirDommage = false;
        _pvs -= dommage;

        if (_pvs <= 0)
        {
            _pvs = 0;
            Mourir();
        }
        else
        {
            UiManager.Instance.UpdatePv(_pvs);
            StartCoroutine(AttendreIFrames());
            IncreaseDistortionAndColor();
        }
    }

    private void Mourir()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ennemi"))
        {
            PrendreDuDommage(1);
            for (int i = 0; i < _skinnedMeshRenderer.materials.Length; i++)
            {
                _skinnedMeshRenderer.materials[i].SetColor("_Color", damageColor);
            }
        }
    }

    private IEnumerator AttendreIFrames()
    {
        yield return new WaitForSeconds(frameDuration);
        _peutRecevoirDommage = true;
        for (int i = 0; i < _skinnedMeshRenderer.materials.Length; i++)
        {
            _skinnedMeshRenderer.materials[i].SetColor("_Color", originalColor);
        }
    }

    private void IncreaseDistortionAndColor()
    {
        if (globalVolume.profile.TryGet(out LensDistortion lensDistortion))
        {
            lensDistortion.intensity.Override(lensDistortion.intensity.value + 0.2f);
        }

        if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            Color currentColor = colorAdjustments.colorFilter.value;
            colorAdjustments.colorFilter.Override(new Color(
                currentColor.r,
                currentColor.g * 0.75f,
                currentColor.b * 0.75f
            ));
        }
    }
}
